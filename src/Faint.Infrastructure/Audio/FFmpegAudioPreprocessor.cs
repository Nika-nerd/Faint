using System.Diagnostics;
using Faint.Core.Interfaces;

namespace Faint.Infrastructure.Audio;

public class FFmpegAudioPreprocessor : IAudioPreprocessor
{
    public async Task<string> PrepareAudioAsync(string inputFilePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(inputFilePath))
        {
            throw new FileNotFoundException("Входной аудиофайл не найден.", inputFilePath);
        }

        // Формируем путь для выходного файла в той же папке
        var directory = Path.GetDirectoryName(inputFilePath) ?? string.Empty;
        var fileName = Path.GetFileNameWithoutExtension(inputFilePath);
        var outputFilePath = Path.Combine(directory, $"{fileName}_16khz.wav");

        // Параметры FFmpeg:
        // -y : перезаписывать файл, если существует
        // -i : входной файл
        // -ar 16000 : частота дискретизации 16 kHz
        // -ac 1 : один канал (моно)
        // -c:a pcm_s16le : аудио кодек PCM 16-bit little-endian (стандартный WAV)
        var processInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-y -i \"{inputFilePath}\" -ar 16000 -ac 1 -c:a pcm_s16le \"{outputFilePath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();

        // Ожидаем завершения конвертации
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            throw new Exception($"Ошибка при конвертации аудио (FFmpeg): {error}");
        }

        return outputFilePath;
    }
}

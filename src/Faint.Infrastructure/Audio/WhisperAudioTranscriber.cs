using Faint.Core.Interfaces;
using Faint.Core.Models;
using Whisper.net;
using Whisper.net.Ggml;

namespace Faint.Infrastructure.Audio;

public class WhisperAudioTranscriber : IAudioTranscriber
{
    private readonly string _modelPath;

    public WhisperAudioTranscriber(string modelPath = "ggml-small.bin")
    {
        _modelPath = modelPath;
    }

    public async Task<TextContent> TranscribeAsync(string audioFilePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(audioFilePath))
        {
            throw new FileNotFoundException("Аудиофайл не найден", audioFilePath);
        }

        GgmlType type = _modelPath.Contains("small", StringComparison.OrdinalIgnoreCase) ? GgmlType.Small : GgmlType.Base;

        await EnsureModelDownloadedAsync(_modelPath, type, cancellationToken);

        using var whisperFactory = WhisperFactory.FromPath(_modelPath);
        using var processor = whisperFactory.CreateBuilder()
            .WithLanguage("auto")
            .Build();

        await using var fileStream = File.OpenRead(audioFilePath);
        var segments = new List<string>();

        await foreach (var segment in processor.ProcessAsync(fileStream, cancellationToken))
        {
            if (!string.IsNullOrWhiteSpace(segment.Text))
            {
                segments.Add(segment.Text.Trim());
            }
        }

        return new TextContent
        {
            Content = string.Join(" ", segments)
        };
    }

    private static async Task EnsureModelDownloadedAsync(string modelPath, GgmlType modelType = GgmlType.Base, CancellationToken cancellationToken = default)
    {
        if (File.Exists(modelPath))
        {
            return;
        }

        Console.WriteLine("Модель Whisper не найдена. Начинаем скачивание...");

        var directory = Path.GetDirectoryName(modelPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var httpClient = new HttpClient();
        var downloader = new WhisperGgmlDownloader(httpClient);
        await using var modelStream = await downloader.GetGgmlModelAsync(modelType, cancellationToken: cancellationToken);

        await using var fileWriter = File.Create(modelPath);
        await modelStream.CopyToAsync(fileWriter, cancellationToken);

        Console.WriteLine("Модель успешно загружена!");
    }
}


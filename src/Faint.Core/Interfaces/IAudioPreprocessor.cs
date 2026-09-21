namespace Faint.Core.Interfaces;

public interface IAudioPreprocessor
{
    /// <summary>
    /// Конвертирует любой аудиофайл в формат, пригодный для Whisper (WAV, 16kHz, Mono).
    /// </summary>
    /// <returns>Путь к сконвертированному WAV-файлу.</returns>
    Task<string> PrepareAudioAsync(string inputFilePath, CancellationToken cancellationToken = default);
}
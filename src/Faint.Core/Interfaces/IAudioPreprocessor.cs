namespace Faint.Core.Interfaces;

public interface IAudioPreprocessor
{
   
    Task<string> PrepareAudioAsync(string inputFilePath, CancellationToken cancellationToken = default);
}
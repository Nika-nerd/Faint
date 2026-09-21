using Faint.Core.Models;

namespace Faint.Core.Interfaces;

public interface IAudioTranscriber
{
    Task<TextContent> TranscribeAsync(string audioFilePath, CancellationToken cancellationToken = default);
}

using Faint.Core.Models;

namespace Faint.Core.Interfaces;

public interface ILlmDocumentGenerator
{
    /// <summary>
    /// Генерирует структурированную документацию на основе расшифрованного текста.
    /// </summary>
    Task<string> GenerateDocumentationAsync(TextContent transcribedText, string systemPrompt, CancellationToken cancellationToken = default);
}
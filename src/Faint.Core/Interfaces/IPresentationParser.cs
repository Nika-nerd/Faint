namespace Faint.Core.Interfaces;

public interface IPresentationParser
{
    Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default);
}
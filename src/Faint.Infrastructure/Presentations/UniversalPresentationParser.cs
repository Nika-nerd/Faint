using Faint.Core.Interfaces;

namespace Faint.Infrastructure.Presentations;

public class UniversalPresentationParser : IPresentationParser
{
    private readonly PdfPresentationParser _pdfParser;
    private readonly PptxPresentationParser _pptxParser;

    public UniversalPresentationParser()
    {
        _pdfParser = new PdfPresentationParser();
        _pptxParser = new PptxPresentationParser();
    }

    public async Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        
        return extension switch
        {
            ".pdf" => await _pdfParser.ExtractTextAsync(filePath, cancellationToken),
            ".pptx" => await _pptxParser.ExtractTextAsync(filePath, cancellationToken),
            _ => throw new NotSupportedException($"Формат презентации '{extension}' не поддерживается. Используйте .pdf или .pptx")
        };
    }
}
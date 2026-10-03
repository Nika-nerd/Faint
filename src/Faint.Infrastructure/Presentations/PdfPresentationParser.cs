using System.Text;
using Faint.Core.Interfaces;
using UglyToad.PdfPig;


namespace Faint.Infrastructure.Presentations;

public class PdfPresentationParser : IPresentationParser
{
    public async Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var sb = new StringBuilder();
            using var document = PdfDocument.Open(filePath);
            
            foreach (var page in document.GetPages())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var text = page.Text;
                sb.AppendLine(text);
            }
            
            return sb.ToString();
        }, cancellationToken);
    }
}
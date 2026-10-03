using System.Text;
using DocumentFormat.OpenXml.Packaging;
using Faint.Core.Interfaces;

namespace Faint.Infrastructure.Presentations;

public class PptxPresentationParser : IPresentationParser
{
    public async Task<string> ExtractTextAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            var sb = new StringBuilder();
            using PresentationDocument presentationDocument = PresentationDocument.Open(filePath, false);
            var presentationPart = presentationDocument.PresentationPart;
            
            if (presentationPart?.SlideParts != null)
            {
                foreach (var slidePart in presentationPart.SlideParts)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (slidePart.Slide != null)
                    {
                        // Извлекаем все текстовые узлы со слайда
                        foreach (var text in slidePart.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            sb.Append(text.Text + " ");
                        }
                        sb.AppendLine();
                    }
                }
            }
            
            return sb.ToString();
        }, cancellationToken);
    }
}
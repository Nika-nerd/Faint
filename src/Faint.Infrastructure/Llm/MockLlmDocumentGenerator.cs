using Faint.Core.Interfaces;
using Faint.Core.Models;

namespace Faint.Infrastructure.Llm;

public class MockLlmDocumentGenerator : ILlmDocumentGenerator
{
    public Task<string> GenerateDocumentationAsync(TextContent transcribedText, string systemPrompt, CancellationToken cancellationToken = default)
    {
        // Создаем фейковый Markdown-документ, чтобы убедиться, что транскрибация сработала
        var fakeMarkdown = $@"
# 📝 Тестовый отчет (Режим без API)

**Система успешно отработала следующие шаги:**
1. ✅ Конвертация аудио (FFmpeg)
2. ✅ Транскрибация речи в текст (Whisper)

---

### 🎙 Распознанный текст:
> {transcribedText.Content}

---
*Примечание: Вы используете Mock-генератор. Для генерации реального summary подключите OpenAI API или локальную Ollama.*
";
        
        return Task.FromResult(fakeMarkdown);
    }
}
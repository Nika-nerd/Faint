using Faint.Core.Interfaces;

namespace Faint.Core.Services;

public class MeetingDocumentationService
{
    private readonly IAudioPreprocessor _audioPreprocessor;
    private readonly IAudioTranscriber _audioTranscriber;
    private readonly ILlmDocumentGenerator _llmDocumentGenerator;
    private readonly IPresentationParser? _presentationParser; // Не забываем парсер презентаций

    // Мощный системный промпт на английском языке
        private const string DefaultSystemPromptTemplate = @"You are an expert technical writer and academic analyst. 
You are provided with a raw transcript of an extended meeting or lecture. 
Your task is to create an exhaustive, highly detailed, and well-structured documentation in {0} using Markdown.

CRITICAL INSTRUCTIONS FOR COVERAGE:
- The AUDIO TRANSCRIPT is your primary source of truth. The lecture's meaning comes from what was spoken.
- DO NOT summarize or compress the content excessively. Retain all key concepts, technical terms, explanations, and analogies.
- Cover the transcript thoroughly from start to finish. Ensure the second half receives the same depth as the first half.
- ABSOLUTE RULE: You MUST use the exact 4-part DOCUMENT STRUCTURE below. Do not deviate from this format.

DOCUMENT STRUCTURE:
1. Executive Summary - A concise overview of the core purpose and high-level takeaways of the session.
2. Key Topics & Detailed Insights - An exhaustive breakdown of all topics discussed. Use sub-headings (###) for distinct themes. Include core concepts, processes, examples, and Q&A.
3. Core Takeaways & Conclusions - Key principles, conclusions, or consensus reached.
4. Action Items & Student Takeaways - Actionable tasks, recommendations, study advice, or next steps.

Formatting guidelines:
- Use clean Markdown with bolding, sub-headings, and bullet points.
- Maintain professional, precise, and grammatically perfect {0}.";

    // Конструктор только принимает зависимости
    public MeetingDocumentationService(
        IAudioPreprocessor audioPreprocessor,
        IAudioTranscriber audioTranscriber,
        ILlmDocumentGenerator llmDocumentGenerator,
        IPresentationParser? presentationParser = null)
    {
        _audioPreprocessor = audioPreprocessor;
        _audioTranscriber = audioTranscriber;
        _llmDocumentGenerator = llmDocumentGenerator;
        _presentationParser = presentationParser;
    }

    public async Task<string> ProcessMeetingAudioAsync(string inputAudioPath, string? presentationPath = null, string targetLanguage = "English", string? customPrompt = null, CancellationToken cancellationToken = default)
    {
        string? tempWavFilePath = null;
        try
        {
            // 1. Конвертация аудио
            tempWavFilePath = await _audioPreprocessor.PrepareAudioAsync(inputAudioPath, cancellationToken);
            
            // 2. Транскрибация аудио
            var textContent = await _audioTranscriber.TranscribeAsync(tempWavFilePath, cancellationToken);

            if (string.IsNullOrWhiteSpace(textContent.Content))
            {
                return "The audio file does not contain any recognizable speech.";
            }

            string presentationText = string.Empty;
            if(!string.IsNullOrEmpty(presentationPath) && _presentationParser != null)
            {
                presentationText = await _presentationParser.ExtractTextAsync(presentationPath, cancellationToken);
            }

            // Подставляем язык в промпт
            var prompt = string.IsNullOrWhiteSpace(customPrompt) 
                ? string.Format(DefaultSystemPromptTemplate, targetLanguage) 
                : customPrompt;

            if (!string.IsNullOrWhiteSpace(presentationText))
            {
                prompt += @"

CRITICAL RULE REGARDING PRESENTATION SLIDES:
- You are provided with extracted text from presentation slides as supplementary material.
- IGNORE ALL metadata, design instructions, layout notes, colors (e.g., 'Dark Theme', 'Background: Clean Off-White'), and slide numbers.
- DO NOT format your output slide-by-slide. DO NOT describe the visual appearance of the slides.
- Use the presentation text ONLY as a reference dictionary to correctly spell technical terms, understand the overarching logical flow, and fill in missing technical formulas/definitions.
- YOU MUST strictly follow the 4-part DOCUMENT STRUCTURE (Executive Summary, etc.) requested above.";

                textContent.Content = $"--- AUDIO TRANSCRIPT (PRIMARY SOURCE) ---\n{textContent.Content}\n\n--- PRESENTATION SLIDES TEXT (REFERENCE ONLY) ---\n{presentationText}";
            }

            return await _llmDocumentGenerator.GenerateDocumentationAsync(textContent, prompt, cancellationToken);
        }
        finally
        {
            if (!string.IsNullOrEmpty(tempWavFilePath) && File.Exists(tempWavFilePath))
            {
                File.Delete(tempWavFilePath);
            }
        }
    }
}
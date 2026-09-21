using Faint.Core.Interfaces;

namespace Faint.Core.Services;

public class MeetingDocumentationService
{
    private readonly IAudioPreprocessor _audioPreprocessor;
    private readonly IAudioTranscriber _audioTranscriber;
    private readonly ILlmDocumentGenerator _llmDocumentGenerator;

    public MeetingDocumentationService(
        IAudioPreprocessor audioPreprocessor,
        IAudioTranscriber audioTranscriber,
        ILlmDocumentGenerator llmDocumentGenerator)
    {
        _audioPreprocessor = audioPreprocessor;
        _audioTranscriber = audioTranscriber;
        _llmDocumentGenerator = llmDocumentGenerator;
    }

    public async Task<string> ProcessMeetingAudioAsync(string inputAudioPath, string? customPrompt = null, CancellationToken cancellationToken = default)
    {
        string? tempWavFilePath = null;
        try
        {
            tempWavFilePath = await _audioPreprocessor.PrepareAudioAsync(inputAudioPath, cancellationToken);
            var textContent = await _audioTranscriber.TranscribeAsync(tempWavFilePath, cancellationToken);

            if (string.IsNullOrWhiteSpace(textContent.Content))
            {
                return "Аудиофайл не содержит распознаваемой речи.";
            }

            string systemPrompt = customPrompt ?? "Создай краткое резюме (Markdown) из этого текста.";
            return await _llmDocumentGenerator.GenerateDocumentationAsync(textContent, systemPrompt, cancellationToken);
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
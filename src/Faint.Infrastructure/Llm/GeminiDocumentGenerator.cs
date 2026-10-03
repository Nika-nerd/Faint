using System.Text;
using System.Text.Json;
using Faint.Core.Interfaces;
using Faint.Core.Models;

namespace Faint.Infrastructure.Llm;

public class GeminiDocumentGenerator : ILlmDocumentGenerator
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly HttpClient _httpClient;

    
    public GeminiDocumentGenerator(string apiKey, string model = "gemini-3.5-flash")
    {
        _apiKey = apiKey;
        _model = model;
        _httpClient = new HttpClient();
    }

    public async Task<string> GenerateDocumentationAsync(TextContent transcribedText, string systemPrompt, CancellationToken cancellationToken = default)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";

        var requestBody = new
        {
            system_instruction = new { parts = new[] { new { text = systemPrompt } } },
            contents = new[]
            {
                new { parts = new[] { new { text = transcribedText.Content } } }
            },
            generationConfig = new { temperature = 0.1 } 
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(url, jsonContent, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Ошибка Gemini API: {response.StatusCode} - {error}");
        }

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        using var jsonDoc = JsonDocument.Parse(responseString);
        
        var resultText = jsonDoc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return resultText ?? string.Empty;
    }
}
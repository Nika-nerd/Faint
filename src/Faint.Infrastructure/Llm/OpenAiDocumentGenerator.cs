using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Faint.Core.Interfaces;
using Faint.Core.Models;

namespace Faint.Infrastructure.Llm;

public class OpenAiDocumentGenerator : ILlmDocumentGenerator
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly HttpClient _httpClient;

    // Используем gpt-4o или gpt-4-turbo, так как у них большое окно контекста
    public OpenAiDocumentGenerator(string apiKey, string model = "gpt-4o")
    {
        _apiKey = apiKey;
        _model = model;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> GenerateDocumentationAsync(TextContent transcribedText, string systemPrompt, CancellationToken cancellationToken = default)
    {
        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = transcribedText.Content }
            },
            temperature = 0.3 // Низкая температура для точности и строгости фактов
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        
        // Обращение к официальному API OpenAI
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Ошибка API LLM: {response.StatusCode} - {error}");
        }

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        using var jsonDoc = JsonDocument.Parse(responseString);
        
        // Парсим ответ от OpenAI
        var resultText = jsonDoc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return resultText ?? string.Empty;
    }
}

using Faint.Core.Interfaces;
using Faint.Infrastructure.Audio;
using Faint.Infrastructure.Llm;
using Faint.Core.Services;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== Faint Audio Transcriber ===");

// Теперь можно использовать любой формат, например .mp3 или .m4a
string audioPath = "lecture.mp3"; 

if (!File.Exists(audioPath))
{
    Console.WriteLine($"[!] Положите файл {audioPath} в папку с запущенной приложением.");
    return;
}

// Настройка Dependency Injection
var services = new ServiceCollection();

services.AddTransient<IAudioPreprocessor, FFmpegAudioPreprocessor>();
services.AddTransient<IAudioTranscriber>(sp => new WhisperAudioTranscriber("ggml-base.bin"));

// --- ОТКЛЮЧАЕМ ПЛАТНЫЙ OPENAI ---
// string openAiApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "ВАШ_OPENAI_API_КЛЮЧ";
// services.AddTransient<ILlmDocumentGenerator>(sp => new OpenAiDocumentGenerator(openAiApiKey));

// --- ВКЛЮЧАЕМ БЕСПЛАТНУЮ ЗАГЛУШКУ ---
services.AddTransient<ILlmDocumentGenerator, MockLlmDocumentGenerator>();

services.AddTransient<MeetingDocumentationService>();

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("Запуск процесса (Конвертация -> Транскрибация -> Генерация документации)...");

// Получаем наш главный сервис-оркестратор из DI
var docService = serviceProvider.GetRequiredService<MeetingDocumentationService>();

// Запускаем весь процесс
var resultMarkdown = await docService.ProcessMeetingAudioAsync(audioPath);

Console.WriteLine("\n--- Итоговая документация (Markdown) ---");
Console.WriteLine(resultMarkdown);
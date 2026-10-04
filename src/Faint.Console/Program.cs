using System;
using System.IO;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using Faint.Core.Interfaces;
using Faint.Core.Services;
using Faint.Infrastructure.Audio;
using Faint.Infrastructure.Llm;
using Faint.Infrastructure.Presentations;

Console.WriteLine("=== Faint Audio Transcriber ===");

Env.Load();

string audioPath = "lecture.mp3"; 
string? presentationPath = "presentation.pptx";
string targetLanguage = "Russian";

if (!File.Exists(audioPath))
{
    Console.WriteLine($"[!] Положите файл {audioPath} в папку с запущенной приложением.");
    return;
}

var services = new ServiceCollection();

services.AddTransient<IAudioPreprocessor, FFmpegAudioPreprocessor>();
services.AddTransient<IAudioTranscriber>(sp => new WhisperAudioTranscriber("ggml-small.bin"));

string geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") 
    ?? throw new InvalidOperationException("API ключ Gemini не найден! Укажите GEMINI_API_KEY в файле .env");

services.AddTransient<ILlmDocumentGenerator>(sp => new GeminiDocumentGenerator(geminiApiKey));

services.AddTransient<IPresentationParser, UniversalPresentationParser>();

services.AddTransient<MeetingDocumentationService>();

var serviceProvider = services.BuildServiceProvider();

Console.WriteLine("Запуск процесса (Конвертация -> Транскрибация -> Генерация документации)...");

var docService = serviceProvider.GetRequiredService<MeetingDocumentationService>();
var resultMarkdown = await docService.ProcessMeetingAudioAsync(audioPath, presentationPath, targetLanguage);

Console.WriteLine("\n--- Итоговая документация (Markdown) ---");
Console.WriteLine(resultMarkdown);
# Faint 🎙️⚡

**Faint** — автоматизированный CLI-инструмент на .NET 9 / C# для обработки аудио-лекций в структурированные Markdown-документы и базы знаний.

## 🚀 Возможности
- **Speech-to-Text:** Транскрибация аудиофайлов лекций.
- **AI Gap Analysis:** Определение пробелов в контексте и терминологии лекции.
- **External Search Integration:** Автоматическое обогащение материала внешними источниками.
- **Knowledge Synthesis:** Генерация конспекта с заголовками, выжимками и сносками с помощью моделей с большим контекстным окном (Gemini / Claude).
- **Markdown Export:** Готовый формат для Obsidian / Notion.

## 🏗️ Архитектура
Проект построен по принципам Clean Architecture:
- `Faint.Core` — доменные модели, интерфейсы и DTO.
- `Faint.Infrastructure` — работы с API (Whisper, Gemini, Search Services).
- `Faint.Services` — бизнес-логика и пайплайны обработки.
- `Faint.Console` — точка входа и CLI интерфейс.

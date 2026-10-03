<div align="center">

# 🎓 F.A.I.N.T

### **F**ast **A**cademic **I**nsight & **N**ote **T**ranscription

*Turn lecture audio and slides into clean, structured study notes, automatically.*

![C#](https://img.shields.io/badge/C%23-.NET%2010-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Whisper](https://img.shields.io/badge/Whisper-Speech--to--Text-412991?style=for-the-badge&logo=openai&logoColor=white)
![OpenXml](https://img.shields.io/badge/OpenXML-PPTX-D24726?style=for-the-badge&logo=microsoftpowerpoint&logoColor=white)
![PdfPig](https://img.shields.io/badge/PdfPig-PDF-E34F26?style=for-the-badge&logo=adobeacrobatreader&logoColor=white)
![Status](https://img.shields.io/badge/status-in%20development-yellow?style=for-the-badge)

</div>

---

## 📖 About

**F.A.I.N.T** takes the two things every lecture produces, **the recording** and **the slides**, and merges them into ready-to-use documentation.

Students spend hours rewatching lectures and rewriting notes. F.A.I.N.T automates that routine so you can spend your time **understanding** the material instead of transcribing it, and have solid notes ready when exam season arrives.

## 🎯 Goals

- ⏱️ **Save time** by replacing manual note-taking with automatic transcription
- 🧠 **Reduce routine** so students can focus on learning
- 📝 **Prepare for exams** with structured, searchable lecture documentation

## ✨ Features

### ✅ Implemented

| Feature | Description |
|---|---|
| 🎙️ **Audio transcription** | Converts lecture recordings to text using **Whisper** |
| 📊 **Presentation parsing** | Extracts slide content from **PPTX** (`DocumentFormat.OpenXml`) and **PDF** (`PdfPig`) |
| 📄 **Documentation generation** | Combines transcript and slides into a single structured document |

### 🗺️ Roadmap

- [ ] 🌍 **Multi-language documentation**: generate notes in different languages
- [ ] 🗄️ **Database integration**: detect duplicate lectures submitted by different students, so nothing is processed twice
- [ ] ⚡ **Asynchronous processing**: non-blocking pipeline for long recordings and large batches

## 🛠️ Tech Stack

| Technology | Purpose |
|---|---|
| **C# / .NET 10** | Core language and runtime |
| **Whisper** | Speech-to-text transcription |
| **DocumentFormat.OpenXml** | Reading and writing Office documents (PPTX) |
| **PdfPig** | Extracting text from PDF presentations |

## 🏗️ Architecture

The solution is split into three projects with clear responsibilities:

```
FAINT
├── 📦 Faint.Core         # Business logic: transcription, parsing, document generation
├── 🔌 Faint.Interfaces   # Abstractions and contracts shared across the solution
└── 💻 Faint.Console      # Console application, the entry point for users
```

| Project | Responsibility |
|---|---|
| **Faint.Core** | Implements the transcription pipeline and slide parsing |
| **Faint.Interfaces** | Defines the interfaces that keep components decoupled and testable |
| **Faint.Console** | Console front-end that wires everything together |

## 🔄 How It Works

```
 ┌──────────────┐      ┌──────────────┐
 │ Lecture audio│      │ Presentation │
 │   (record)   │      │ (PPTX / PDF) │
 └──────┬───────┘      └──────┬───────┘
        │                     │
        ▼                     ▼
 ┌──────────────┐      ┌──────────────┐
 │   Whisper    │      │ OpenXml /    │
 │ transcription│      │ PdfPig parse │
 └──────┬───────┘      └──────┬───────┘
        └──────────┬──────────┘
                   ▼
          ┌─────────────────┐
          │  Structured     │
          │  documentation  │
          └─────────────────┘
```

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A Whisper model (see the Whisper documentation for setup)

### Installation

```bash
git clone https://github.com/Nika-nerd/FAINT.git
cd FAINT
dotnet restore
dotnet build
```

### Usage

```bash
dotnet run --project Faint.Console -- --audio "lecture.mp3" --slides "presentation.pptx"
```

> ⚠️ The project is under active development, so the CLI options may change.

## 🤝 Contributing

Contributions, ideas and feature requests are welcome. Feel free to open an issue or submit a pull request.

## 📜 License

Distributed under the MIT License. See `LICENSE` for more information.

---

<div align="center">

**Made with ☕ for students who have better things to do than retype lectures.**

</div>

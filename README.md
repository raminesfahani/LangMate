# LangMate

[![Build & Publish NuGet Packages](https://github.com/raminesfahani/LangMate/actions/workflows/nuget-packages.yml/badge.svg)](https://github.com/raminesfahani/LangMate/actions/workflows/nuget-packages.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Language](https://img.shields.io/github/languages/top/raminesfahani/LangMate)](https://github.com/raminesfahani/LangMate/search?l=c%23)
![GitHub Repo stars](https://img.shields.io/github/stars/raminesfahani/LangMate?style=social)

**LangMate** is a modular and extensible AI chat application and SDK platform built with .NET 9 and fully compatible with .NET Aspire.  
It provides a Blazor-powered Web UI, Ollama model integrations, persistent chat history via MongoDB, and a flexible SDK for .NET developers to integrate and use local LLMs (like Gemma, LLaMA2, Mistral) easily and securely.

![LangMate Logo](https://github.com/raminesfahani/LangMate/raw/main/logo.png)
---

## 🌟 Key Features

- ⚙️ **Fully compatible with .NET Aspire**
- 💬 **Chat UI** 
    - built with Blazor Server (interactive, reactive experience)
- 🧠 **Ollama Integration** 
  - LLM-based completion and conversation
- 🗃 **MongoDB Chat History and caching** 
  - for persistent and fast retrieval
- 🔧 **LangMate.Core SDK** 
  - use Ollama easily in your own .NET apps
- 🧩 **Pluggable Middleware** 
  - Polly-powered **Resiliency**, Circuit Breakers, and Retry logic
- 🚀 **File Upload Support** 
  - with base64 image preview support (for image input models)
- 🌐 **API Endpoints** 
  - Implemented a backend-driven project for using in every client app
- 🧰 **Developer-Friendly** architecture — clean, testable and maintainable

---

## 🧠 Architecture

```
LangMate/
├── Apps/
├──── LangMate.AppHost.AppHost        → .NET Aspire for orchestrating and deploying the apps on Docker, Kubernetes, or any other cloud platform.
├──── LangMate.AppHost.BlazorUI        → Blazor ChatBot UI sample project using LangMate SDK
├──── LangMate.AppHost.ApiService        → Web API sample project using LangMate SDK
├── SDK/
├──── LangMate.Core               → Reusable .NET SDK to interact with Ollama
├──── LangMate.Middleware         → Polly-based Resiliency Middleware (retry, timeout, circuit breaker)
├──── LangMate.Extensions         → Utilities, helpers and extension methods
├──── LangMate.Persistence        → MongoDB chat history, caching layer, repositories and configuration
```

---

## 📦 Installation

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Ollama](https://ollama.com/) installed and running locally (for model inference)
- [MongoDB](https://www.mongodb.com/) (locally or cloud instance)

### Run the App

```bash
git clone https://github.com/raminesfahani/LangMate.git
cd LangMate/src/apps/LangMate.AppHost.AppHost

# Restore dependencies and build
dotnet restore
dotnet build

# Run the .NET Aspire Dashboard
dotnet run
```

Then open `https://localhost:17198/` in your browser to see the .NET Aspire dashboard. You can launch Blazor ChatBot or Web API apps over there by their own links.

---

# LangMate.Core SDK

**LangMate.Core** is a lightweight, extensible .NET SDK designed to make working with Ollama-powered local AI models seamless and developer-friendly. It abstracts away the complexity of managing conversations, interacting with Ollama endpoints, and persisting chat history — all while offering resiliency, caching, and extensibility.

## 📦 Installation

To use `LangMate.Core`, install the required NuGet package [![NuGet](https://img.shields.io/nuget/v/LangMate.Core)](https://www.nuget.org/packages/LangMate.Core), or include the project reference in your solution.

```bash
dotnet add package LangMate.Core
```

## ✅ Sample Usage

You can see [Full Documentation](src/sdk/LangMate.Core/README.md) and sample usage in this link as well.

---

## 🧠 LangMate Blazor Chat UI

The **LangMate Blazor App** is an intelligent, real-time chat UI built with Blazor Server and integrated with the powerful local AI models provided by Ollama using ***LangMate.Core SDK***.

It provides a complete frontend experience for interacting with AI models, managing chat history, uploading files, and dynamically updating chat state.

### ✨ Features

- 🔁 Chat with Ollama Models: Seamlessly send and stream messages from local Ollama instances.

- 💬 Persistent Conversations: Every chat session is stored in MongoDB and can be resumed anytime.

- 📂 File Uploads: Upload image files and pass them to models like llava for multimodal interactions.

- 🧭 Sidebar Navigation: Access previous chats and start new ones from a clean sidebar UI.

- 📦 Model Switching: Easily switch between available Ollama models.

- 🔃 Streaming Responses: Uses async streaming to display tokens as they’re generated.

- ☁️ Resilient Middleware: Protected with timeout, retry, and circuit breaker policies.

- 🔔 Global Error Toasts: All unhandled exceptions surface as toast notifications.

## Screenshots

<div><img src="assets/chat-blazor-ui/home.png" alt="LangMate Blazor Chat UI - Home Page"/></div>
<br>
<div><img src="assets/chat-blazor-ui/chat-ui.jpg" alt="LangMate Blazor Chat UI - Chat user interface"/></div>
<br>
<div><img src="assets/chat-blazor-ui/sample-conversation.jpg" alt="LangMate Blazor Chat UI - Sample chat"/></div>

---

## 📡 LangMate WebAPI

The **LangMate API Service** is the backend layer of the LangMate system, exposing RESTful HTTP APIs for external integration, orchestration, and automation.

It serves as a stateless gateway for interacting with the LangMate core functionalities — such as chat sessions, file uploads, model management, and streaming chat completions — powered by the ***LangMate.Core SDK*** and ***Ollama***.

### ✨ Features

- 🔗 Chat Completion API: Start or continue chat sessions supporting stream mode with local Ollama models.

- 🧠 Model Discovery: Query available and pulled models from the Ollama runtime.

- 💬 Conversation APIs: Read, delete, and manage persistent chat history.

- 🖼️ File Upload: Upload image files to be used with multimodal models (e.g., gemma).

- 🔐 Middleware-Enhanced Resilience: Protected by retry, timeout, and circuit breaker policies via LangMate.Middleware.

- ⚙️ Scalar Integration: Auto-generated OpenAPI documentation (easily added).

<br>
<div><img src="assets/web-api/api-doc.png" alt="LangMate Web API documentation"/></div>

---

## 🛠️ Build and Test

```bash
dotnet build --configuration Release
dotnet test
```

---

## 📬 Contributing

Contributions are welcome!

1. Fork the repo and create your branch
2. Implement your feature or fix
3. Submit a PR with proper context

---

## 📄 License

Licensed under the [MIT License](LICENSE).

---

## 📣 Contact

Created and maintained by [@raminesfahani](https://github.com/raminesfahani).  
For issues and features, open a [GitHub Issue](https://github.com/raminesfahani/LangMate/issues).

---
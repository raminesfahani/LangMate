# LangMate

**LangMate** — a modern local-first AI assistant and developer SDK powered by **Ollama** and **Blazor**.  
Chat with local LLMs, manage conversations, and integrate advanced AI features into your own .NET apps — all securely on your machine.

---

## Key features

- Local LLM support via **Ollama** (run models locally for privacy and latency).
- Blazor-based UI for fast, cross-platform frontends.
- Conversation management, caching and persistence primitives.
- Extensible SDK (Abstractions, Extensions, Middleware) for building custom assistant experiences.
- Test projects demonstrating unit and integration tests.
- Designed for offline-first and self-hosted workflows.

---

## Quickstart

### Requirements
- .NET SDK 8.0 or later installed.
- [Ollama](https://ollama.com) or compatible local LLM runtime available and running (models pulled locally).
- A modern browser for the Blazor UI.

### Run locally (example)
1. Start your Ollama server and ensure a model is installed and accessible (see Ollama docs).
2. From the repo root:
```bash
# Build solution
dotnet build

# Run AppHost (Blazor UI + API)
cd src/LangMate.AppHost/LangMate.AppHost.AppHost
dotnet run

# In another terminal you can run API service or tests as needed:
cd ../../LangMate.AppHost.ApiService
dotnet run
```

3. Open the Blazor UI at the URL reported by `dotnet run` (usually https://localhost:5001).

---

## Project structure & short descriptions

This repository is organized into several projects. Briefly:

- **src/LangMate.Abstractions** — (project folder)
- **src/LangMate.AppHost/LangMate.AppHost.ApiService** — (project folder)
- **src/LangMate.AppHost/LangMate.AppHost.AppHost** — (project folder)
- **src/LangMate.AppHost/LangMate.AppHost.BlazorUI** — (project folder)
- **src/LangMate.AppHost/LangMate.AppHost.ServiceDefaults** — (project folder)
- **src/LangMate.AppHost/LangMate.AppHost.Tests** — (project folder)
- **src/LangMate.Cache** — (project folder)
- **src/LangMate.Core** — (project folder)
- **src/LangMate.Extensions** — (project folder)
- **src/LangMate.Middleware** — (project folder)
- **tests/LangMate.Cache.Tests** — (project folder)
- **tests/LangMate.Core.Tests** — (project folder)
- **tests/LangMate.Middleware.Tests** — (project folder)

**Highlights**
- `src/LangMate.Abstractions` — Shared interfaces and DTOs used across the SDK.
- `src/LangMate.Core` — Core runtime, conversation handling, model adapters and orchestration.
- `src/LangMate.Cache` — Persistence and caching layers (local disk, optional DB adapters).
- `src/LangMate.Extensions` — Optional extensions and helper utilities for integrating 3rd-party tools.
- `src/LangMate.Middleware` — Request/response middleware for augmenting prompts, logging, and safety checks.
- `src/LangMate.AppHost/*` — Blazor UI, API service, host configurations and defaults.
- `tests/*` — Unit and integration tests for core modules and middleware.

---

## Configuration & environment

- Environment variables used by the projects (examples):
  - `OLLAMA_URL` — URL to the local Ollama API (e.g. `http://localhost:11434`).
  - `LANGMATE__DB_PATH` — Path to local persistence files.
  - Standard ASP.NET Core environment variables (ASPNETCORE_ENVIRONMENT, etc.)

Check project `appsettings.json` files under `src/LangMate.AppHost/*` for concrete configuration keys.

---

## Contributing

See `CONTRIBUTING.md` for guidelines on development, testing, style and PR process.

---

## License

Include project license here if present (add `LICENSE` file at repo root if missing).

---

_Last updated: 2025-09-29 16:59 UTC_

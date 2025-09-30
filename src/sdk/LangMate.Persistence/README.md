# LangMate.Persistence

> Part of the [LangMate.Core](https://github.com/raminesfahani/LangMate) SDK

## 📦 NuGet

[![NuGet](https://img.shields.io/nuget/v/LangMate.Persistence)](https://www.nuget.org/packages/LangMate.Persistence)

**LangMate.Persistence** provides a clean and extensible MongoDB-based persistence layer for the LangMate ecosystem. It powers features like chat history, conversation storage, and data caching in LangMate-based applications.

## ✨ Features

- MongoDB support for chat conversations and messages
- Repository pattern abstraction for testability and clean architecture
- Pluggable via DI using LangMate.Core
- Optional caching support
- Built for high-performance and scalable data access

## 📦 Installation

To use `LangMate.Persistence`, install the required NuGet package, or include the project reference in your solution.

```bash
dotnet add package LangMate.Extensions
```

## 🔧 Service Registration

In your `Program.cs` or inside a service registration method (already is done in LangMate.Core):

```csharp
services.AddLangMateMemoryCache();
services.AddLangMateMongoDb(configuration);
```

## 📄 License

MIT License
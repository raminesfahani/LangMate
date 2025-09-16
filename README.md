# Netrilo Infrastructure

[![Build & Publish NuGet Packages](https://github.com/raminesfahani/Netrilo_Infrastructure/actions/workflows/nuget-packages.yml/badge.svg)](https://github.com/raminesfahani/Netrilo_Infrastructure/actions/workflows/nuget-packages.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Language](https://img.shields.io/github/languages/top/raminesfahani/Netrilo_Infrastructure)](https://github.com/raminesfahani/Netrilo_Infrastructure/search?l=c%23)
![GitHub Repo stars](https://img.shields.io/github/stars/raminesfahani/Netrilo_Infrastructure?style=social)

**Netrilo** is a modular infrastructure SDK for modern .NET applications.  
It provides plug-and-play building blocks for common concerns like logging, persistence, web layers, messaging, and extensions — all in a clean, testable, and highly maintainable architectures, including Domain-Driven-Design, Event-Driven and so on.

![Netrilo Logo](https://github.com/raminesfahani/Netrilo_Infrastructure/raw/main/logo.png)

---

## 📖 About the Project

This repository is the backbone of the `Netrilo` infrastructure layer, designed to be:

- 🔌 **Modular**: Use only what you need.
- 🧪 **Testable**: Fully decoupled, allowing unit testing and mocking.
- ♻️ **Reusable**: Drop into any .NET Core / .NET 8+ project and / Fully compatible with .Net Aspire.
- 🔍 **Discoverable**: Each module has its own documentation and NuGet package.
- 📦 **Ready Deployment**: Easy, customizable and flexible Azure and Github artifacts deployment.

---

## 🧱 Architecture Overview

Each module targets a specific infrastructure concern:

| Module | Purpose | NuGet Package | Documentation |
|--------|---------|---------------|---------------|
| **Abstractions** | Core contracts and DI service definitions | [![Abstractions](https://img.shields.io/nuget/v/Netrilo.Infrastructure.Common.Abstractions?color=green)](https://www.nuget.org/packages/Netrilo.Infrastructure.Common.Abstractions) | [![Full Documentation](https://img.shields.io/badge/Full%20Documentation-orange)](Src/Netrilo.Infrastructure.Common.Abstractions) |
| **Bus**         | Event bus & messaging pattern implementation | [![Bus](https://img.shields.io/nuget/v/Netrilo.Infrastructure.Common.Bus?color=green)](https://www.nuget.org/packages/Netrilo.Infrastructure.Common.Bus) | [![Full Documentation](https://img.shields.io/badge/Full%20Documentation-orange)](Src/Netrilo.Infrastructure.Common.Bus) |
| **Extensions**  | Common helper utilities and extension methods | [![Extensions](https://img.shields.io/nuget/v/Netrilo.Infrastructure.Common.Extensions?color=green)](https://www.nuget.org/packages/Netrilo.Infrastructure.Common.Extensions) | [![Full Documentation](https://img.shields.io/badge/Full%20Documentation-orange)](Src/Netrilo.Infrastructure.Common.Extensions) |
| **Logging**     | Structured logging setup using Serilog | [![Logging](https://img.shields.io/nuget/v/Netrilo.Infrastructure.Common.Logging?color=green)](https://www.nuget.org/packages/Netrilo.Infrastructure.Common.Logging) | [![Full Documentation](https://img.shields.io/badge/Full%20Documentation-orange)](Src/Netrilo.Infrastructure.Common.Logging) |
| **Persistence** | EF Core integration, repository pattern, migrations supporting SQL and NoSQL databases.| [![Persistence](https://img.shields.io/nuget/v/Netrilo.Infrastructure.Common.Persistence?color=green)](https://www.nuget.org/packages/Netrilo.Infrastructure.Common.Persistence) | [![Full Documentation](https://img.shields.io/badge/Full%20Documentation-orange)](Src/Netrilo.Infrastructure.Common.Persistence) |
| **Web**         | Implementing API explorers and filters, Validation and Exception handling, and Service Discovery| [![Web](https://img.shields.io/nuget/v/Netrilo.Infrastructure.Common.Web?color=green)](https://www.nuget.org/packages/Netrilo.Infrastructure.Common.Web) | [![Full Documentation](https://img.shields.io/badge/Full%20Documentation-orange)](Src/Netrilo.Infrastructure.Common.Web) |


These modules can be installed independently and used in isolation or together. You can read the full installation and usage guideline for each project package in the above documentation links.

---

## 🚀 Installation

You can install any package using the NuGet CLI or .NET CLI and you can choose one from the above list and install from the NuGet website or Package Manager. Here is the command you can use to install manually via `.NET CLI`:

```bash
dotnet add package Netrilo.Infrastructure.Common.Abstractions
dotnet add package Netrilo.Infrastructure.Common.Bus
dotnet add package Netrilo.Infrastructure.Common.Extensions
dotnet add package Netrilo.Infrastructure.Common.Logging
dotnet add package Netrilo.Infrastructure.Common.Persistence
dotnet add package Netrilo.Infrastructure.Common.Web
```

---

## Azure Artifact Deployment

You can easily configure and customize this [YAML Pipeline](Deployment/Azure-Pipeline.yml) for deploying on `Azure Artifact` by setting your environment variables and repo supporting `package versioning`.

---

## 🛠️ Build Instructions

To build all projects and generate NuGet packages:

```bash
dotnet restore
dotnet build --configuration Release
```

---

## 🧪 Unit Tests

All unit tests for the repository are located under the `Tests/` folder.

### Run all tests

Use the following `dotnet` command to discover and run all tests in the repository:

```bash
dotnet test --configuration Release
```

## 🗂️ Repository Structure

```
Main Projects/
├── Netrilo.Infrastructure.Common.Abstractions/
├── Netrilo.Infrastructure.Common.Bus/
├── Netrilo.Infrastructure.Common.Extensions/
├── Netrilo.Infrastructure.Common.Logging/
├── Netrilo.Infrastructure.Common.Persistence/
├── Netrilo.Infrastructure.Common.Web/

Test Projects/
├── Netrilo.Infrastructure.Common.Abstractions.UnitTests/
├── Netrilo.Infrastructure.Common.Bus.UnitTests/
├── Netrilo.Infrastructure.Common.Extensions.UnitTests/

.github/
└── workflows/
    └── nuget-packages.yml     → CI/CD pipeline

Deployment/
└── Azure Artifacts/
    └── Azure-Pipeline.yml     → CI/CD pipeline

artifacts/              → Generated NuGet packages
```

Each main project has its own README and is published as a standalone NuGet package.

---

## 🤝 Contribution Guide

We welcome contributions to improve and extend the SDK.  
To contribute:

1. Fork the repo and create your feature branch.
2. Commit your changes and push.
3. Open a pull request.

Please make sure that your changes follow the coding standards and are thoroughly tested.

---

## 🔒 License

This project is licensed under the [MIT License](LICENSE).

---

## 📬 Contact

Maintained by [@raminesfahani](https://github.com/raminesfahani).  
For feature requests or bug reports, please [open an issue](https://github.com/raminesfahani/Netrilo_Infrastructure/issues).

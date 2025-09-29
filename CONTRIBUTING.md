# Contributing to LangMate

Thank you for wanting to contribute! This document explains how to get started.

## How to contribute
1. Fork the repository and create a feature branch.
2. Follow the coding conventions used across the repo (C# .NET style).
3. Include or update unit tests where appropriate.
4. Open a pull request with a clear description of your changes.

## Development workflow
- Use `dotnet build` and `dotnet test` to ensure code compiles and tests pass.
- Keep changes isolated and make PRs small and focused.
- Add XML docs for public APIs and keep README updated for any public surface changes.

## Code style & formatting
- Use `dotnet format` / EditorConfig to keep consistent formatting.
- Prefer clear naming, small methods, and dependency injection for testability.

## Running tests
```bash
dotnet test
```

## CI / Checks
- Ensure all tests pass and static analysis checks succeed before merging.

Thank you — we appreciate your time and contributions!

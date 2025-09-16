# 🙌 Contributing to Netrilo Infrastructure

Thanks for taking the time to contribute to **Netrilo**, a modular infrastructure SDK for modern .NET applications!

We welcome contributions of **any kind** — bug reports, feature suggestions, code improvements, documentation fixes, and more.

---

## 📦 Repository Overview

**Netrilo** aims to provide plug-and-play building blocks for common infrastructure concerns such as:
- Logging
- Persistence
- Messaging
- Web Layering
- Testing
- Caching
- And more...

Each module lives in its own project folder and is built independently.

---

## 🧰 How to Get Started

### 1. Fork the Repository

Click the **"Fork"** button at the top-right of this repo and clone your fork:

```bash
git clone https://github.com/YOUR_USERNAME/Netrilo_Infrastructure.git
cd Netrilo_Infrastructure
```

### 2. Create a Feature Branch

```bash
git checkout -b feature/my-awesome-feature
```

### 3. Install .NET SDK

Ensure you're using the correct .NET version:

```bash
dotnet --version
# Recommended: .NET 7.0+ or as defined in global.json
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run Tests

```bash
dotnet test
```

---

## 📋 Guidelines

### ✔️ Code Style
- Use **.NET naming conventions**.
- Keep each module **isolated and single-responsibility**.
- Write **unit tests** for any non-trivial logic.

### 📦 New Modules
When adding a new module:
- Place it in a separate folder: `src/Netrilo.Infrastructure.YourModuleName`
- Add a matching test project: `tests/Netrilo.Infrastructure.YourModuleName.Tests`
- Register any DI services via extension methods.

### 🧪 Testing
- We use xUnit or MSTest — follow the existing test structure.
- Tests should be **isolated**, **fast**, and **deterministic**.

---

## 📝 Commit Messages

Use clear, descriptive commit messages:

```
feat(logging): add file logging with rolling strategy
fix(persistence): resolve EF Core tracking bug
docs(readme): update badge and module links
```

---

## 🚀 Pull Requests

- Push your branch and open a PR against the `main` branch.
- Fill in the PR template (if available).
- Make sure the CI passes and tests run.
- Be kind and open to code review feedback 😊

---

## 💬 Need Help?

Feel free to [open an issue](https://github.com/raminesfahani/Netrilo_Infrastructure/issues) with any questions or feature suggestions.

---

## 💖 Thank You!

Your contributions make **Netrilo** better — thank you for being part of the community! 🌟

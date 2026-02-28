# AGENTS.md

Guidelines for AI agents (Copilot, Claude, etc.) contributing to Netler.NET.

---

## Project Structure & Module Organization

```
Netler.sln                        — Solution root
src/
  Netler/
    Server.cs                     — TCP server with fluent builder API
    Client/                       — Client-side connection logic
    Messages/                     — MessagePack-serialized message types
    Request.cs                    — Request model
    Response.cs                   — Response model
    StreamExtensions.cs           — Stream helpers (4-byte length-prefixed framing)
tests/
  UnitTests/
    RequestTests.cs               — Protocol parsing tests (isolated)
    ResponseTests.cs              — Protocol parsing tests (isolated)
  IntegrationTests/
    ServerTests.cs                — Full server↔client TCP round-trip tests
    sleep.sh / sleep.cmd          — Helper scripts used by integration tests
DOCS.md                           — Auto-generated from XML doc comments
```

---

## Build, Test, and Development Commands

```sh
# Restore dependencies
dotnet restore Netler.sln

# Build entire solution
dotnet build Netler.sln

# Run all tests
dotnet test Netler.sln

# Run only unit tests
dotnet test tests/UnitTests/UnitTests.csproj

# Run only integration tests
dotnet test tests/IntegrationTests/IntegrationTests.csproj

# Pack NuGet package (Release configuration)
dotnet pack src/Netler/Netler.csproj -c Release
```

---

## Coding Style & Naming Conventions

- Follow standard C# conventions:
  - PascalCase for public members, types, and namespaces
  - camelCase for local variables and parameters
  - Braces on their own lines (Allman style)
- All TCP message payloads are serialized with **MessagePack** — do not introduce other serialization formats
- The server API uses a fluent builder style: `Server.Create(config => ...)` — preserve this pattern
- Messages are length-prefixed with a **4-byte header** describing the content length — maintain this framing
- Avoid compiler warnings; fix or suppress with justification if unavoidable

---

## Testing Guidelines

- Both test projects use the **xUnit** framework
- **UnitTests**: test protocol logic (request/response parsing) in isolation — no network I/O
- **IntegrationTests**: test full server↔client TCP round-trips; rely on `sleep.sh`/`sleep.cmd` helpers
- Run `dotnet test Netler.sln` locally before opening a PR
- Name test methods using the pattern: `Scenario_UnderTest_ExpectedOutcome`

---

## Commit & PR Guidelines

- Use present-tense imperative voice: `Add route parameter validation`, `Fix null reference in response parser`
- Use `[skip ci]` only for documentation-only commits (e.g. README/DOCS updates)
- PR descriptions should explain the change and reference which tests cover it

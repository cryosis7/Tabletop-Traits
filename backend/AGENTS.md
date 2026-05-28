# Backend - Board Game Rankings

## Overview

ASP.NET Core Web API (.NET 10, C#) that syncs board game collection data from the BoardGameGeek XML API, caches it in memory, and provides endpoints for collection browsing and mechanism-based analysis.

## Architecture

Domain-Driven Design with strict layered dependencies:

```
Api → Application → Domain
Api → Infrastructure → Application → Domain
```

| Layer | Path | Responsibility |
|-------|------|----------------|
| Domain | `src/BoardGameRankings.Domain/` | Entities, Value Objects, repository interfaces. Zero external dependencies. |
| Application | `src/BoardGameRankings.Application/` | Services, DTOs, use-case orchestration. Depends only on Domain. |
| Infrastructure | `src/BoardGameRankings.Infrastructure/` | BGG XML API client, in-memory cached repositories, DI registration. Depends on Domain + Application. |
| Api | `src/BoardGameRankings.Api/` | Controllers, middleware, DI composition root. Depends on Application + Infrastructure + DevTools (dev only). |
| DevTools | `src/BoardGameRankings.DevTools/` | WireMock-based BGG mock server and HTML fixtures. Used by Api in Development mode and by tests. |

### Key Conventions

- **Dependency direction flows inward.** Domain has no project references. Infrastructure implements Domain interfaces.
- **Repository pattern** abstracts persistence. Repositories are defined as interfaces in Domain and implemented in Infrastructure.
- **Services** live in Application and contain business logic orchestration. They depend on Domain interfaces, never on Infrastructure directly.
- **DTOs** are defined in Application and returned from service methods. Controllers map between HTTP and DTOs only.
- **DI registration** for Infrastructure lives in `DependencyInjection.cs` within that project.

## Coding Standards

### Naming

- PascalCase for public members, types, namespaces, and methods.
- camelCase for private fields prefixed with underscore (`_collectionRepository`).
- Suffix interfaces with their purpose (`ICollectionRepository`, `IBggApiClient`).
- Suffix services with `Service`, DTOs with `Dto`.

### Patterns to Follow

- Constructor injection for all dependencies.
- `async/await` throughout; return `Task` or `Task<T>` from async methods.
- Use `CancellationToken` propagation in async methods.
- Prefer `IReadOnlyList<T>` or `IEnumerable<T>` for return types over mutable collections.
- Keep controllers thin; delegate logic to Application services.
- Use strongly-typed configuration via `IOptions<T>` where applicable.
- Use primary constructors when possible

### Anti-Patterns to Avoid

- Do not reference Infrastructure from Domain or Application projects.
- Do not put business logic in controllers.
- Do not use `HttpClient` directly; use typed clients or the BGG API client abstraction.
- Do not catch exceptions silently; log and rethrow or return meaningful error responses.
- Do not use static state or service locator patterns.

## Testing

- Framework: xUnit
- Mocking: WireMock.Net (HTTP-level mocking via DevTools fixtures)
- Test projects mirror source structure:
  - `tests/BoardGameRankings.Domain.Tests/` - Domain entity and value object tests
  - `tests/BoardGameRankings.Application.Tests/` - Integration tests against WireMock fixtures

### Conventions

- One test class per production class.
- Test method naming: `MethodName_Scenario_ExpectedResult`.
- Arrange/Act/Assert structure in every test.
- Use the `BggMockFixture` class to stand up WireMock with HTML fixtures from DevTools.
- Test edge cases: null inputs, empty collections, invalid data.

## Error Handling

- Use exceptions for exceptional/unexpected conditions, not for control flow.
- Validate inputs at the API boundary (controllers) before passing to services.
- Return appropriate HTTP status codes (400 for bad input, 404 for not found, 500 for unhandled errors).
- Log errors with structured logging (ILogger<T>).
- BGG API calls must include retry logic with exponential backoff and respect rate limits.

## Key Commands

```powershell
# Build
dotnet build BoardGameRankings.slnx

# Run API (with hot reload)
dotnet watch run --project src/BoardGameRankings.Api

# Run all tests
dotnet test BoardGameRankings.slnx

# Run specific test project
dotnet test tests/BoardGameRankings.Domain.Tests
dotnet test tests/BoardGameRankings.Application.Tests
```

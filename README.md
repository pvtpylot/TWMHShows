# Tradewinds Model Horse Shows (TWMH Shows)
A .NET 9 solution that delivers a shared UI and identity experience across:
- .NET MAUI Blazor Hybrid (mobile/desktop)
- Blazor Web App + minimal Web APIs (server)

The solution provides authenticated management of shows, divisions/classes, and participant collections, with a consistent component model and DTOs shared between client and server. It is designed for live show operations with a clear roadmap toward event-driven messaging (Kafka/Azure Event Hubs) for real-time ringside workflows.

## Solution overview

Projects
- MauiBlazorWeb.Web (Server)
  - Blazor Web App hosting the UI for desktop browsers.
  - ASP.NET Core Identity for local accounts and account management.
  - EF Core (code-first) with migrations to persist shows and related entities.
  - REST endpoints to support the Hybrid client and Web app (for example, `api/userModelObjects`, `api/shows`, `api/weather`).
  - Swagger/OpenAPI enabled for API exploration at `/swagger`.
- MauiBlazorWeb (Hybrid Client)
  - .NET MAUI Blazor Hybrid app that reuses the same Blazor components and DTOs.
  - Authenticated `HttpClient` via an `IHttpClientFactory` helper (`CreateAuthenticatedClientAsync`) with access tokens stored in device secure storage.
  - Cross-platform targets: Windows and Android out of the box (iOS/MacCatalyst with platform provisioning).
- MauiBlazorWeb.Shared (Shared)
  - DTOs (for example, `ShowDto`, `UserModelObjectDto`) and service contracts (for example, `IDataService`, `IShowService`) to keep client/server contracts consistent.

Primary features
- Unified Identity: ASP.NET Core Identity endpoints are exposed for both the Web app and the MAUI Hybrid client. The Hybrid client logs in/out and refreshes tokens, persisting them to secure device storage.
- Role-aware UI: Navigation and pages are gated by authentication/roles, including `ShowHolder`, `Admin`, and judge-facing views.
- Show management domain:
  - Entity `Show` includes scheduling (start/end), status (`Upcoming`, `InProgress`, `Completed`, `Cancelled`), access control (private/member-only), judging deadlines, NAN/NAMHSA flags, format/type, and metadata.
  - Judge assignment and Show Holder ownership are modeled via foreign keys.
  - Divisions and classes (with sort order and constraints like scale, breed, performance type) are supported by the underlying schema.
- Clean layering:
  - Repository + service pattern in the server (for example, `IShowRepository` used by `ShowService`) with DTO mapping boundaries.
  - The Hybrid client uses a typed data service (`MauiDataService`) to call APIs, centralizing error handling and auth.
- SOLID-oriented architecture:
  - SRP via focused repositories/services and explicit DTO mapping boundaries; DIP through DI-first design (`IShowRepository`, `IHttpClientFactory`, `IErrorHandler`); ISP by keeping interfaces narrowly scoped in `Shared`.
- Open for extension (OCP):
  - New show formats/metadata can be introduced via enums and `AdditionalMetadata` (JSON) with migrations, minimizing changes to consumers.

Representative server code
- `MauiBlazorWeb.Web/Services/ShowService.cs`:
  - Maps domain entities to DTOs, enforces validity, and mediates repository operations (`CreateAsync`, `UpdateAsync`, `GetAllAsync`, etc.).
- `MauiBlazorWeb.Web/Data/Show.cs`:
  - EF Core entity defining the show surface (type/format, privacy flags, deadlines, metadata, judge, holder, timestamps).
- Latest EF Core migration (example `ShowManagePage`) illustrates the breadth of show metadata and class/division enhancements.

Representative client code
- `MauiBlazorWeb/Services/MauiDataService.cs`:
  - Uses an authenticated `HttpClient` from `IHttpClientFactory` to call APIs.
  - Centralized exception handling and diagnostics, with payload/response logging in Debug builds.
  - Persists and uses tokens securely for subsequent API calls.

Navigation and roles (selected)
- Web Nav (`MauiBlazorWeb.Web/Components/Layout/NavMenu.razor`):
  - Shows login/register for anonymous users.
  - Displays “My Collection,” user management, and account pages for authenticated users; includes anti-forgery protected logout.
- Hybrid Nav (`MauiBlazorWeb/Components/Layout/NavMenu.razor`):
  - Shows “Manage Shows” for `ShowHolder` or `Admin`.
  - Shows “User Management” for `Admin`.

## Roadmap: real-time live show messaging (Kafka + Azure Event Hubs)

To support real-time judging, ring assignments, and scoring updates during live shows, the system will adopt an event-driven architecture:

- Event backbone
  - Apache Kafka (on-premises or Confluent Cloud) for local/venue deployments.
  - Azure Event Hubs for cloud-scale ingestion and fan-out.
  - Compatibility option: Azure Event Hubs Kafka endpoint to run a single producer/consumer code path when desired.
- Event design
  - Topics/streams: `"entries-submitted"`, `"ring-assignments"`, `"live-scoring"`, `"results-published"`, `"notifications"`.
  - Partitioning by `ShowId` or `RingId` to ensure ordering where needed.
  - Schema governance (JSON initially; evaluate Avro + Schema Registry).
  - Idempotent producers; exactly-once processing where supported; consumer groups for judges, stewards, and scoreboard displays.
- Reliability and integration
  - Outbox pattern in the Web app to durably emit domain events from EF Core transactions.
  - Background services (Hosted Services) as producers/consumers with `Confluent.Kafka` and/or `Azure.Messaging.EventHubs`.
  - MAUI app: lightweight consumers for ring-side updates; fall back to polling if messaging is unavailable.
  - Optionally bridge on-prem Kafka with cloud Event Hubs for hybrid shows.
- Observability
  - Structured logging (OpenTelemetry), consumer lag metrics, and DLQs/poison message handling.

This plan allows non-blocking, real-time updates to flow to judges, competitors, and displays, while preserving the current REST/DTO model for CRUD and page loads.

## Getting started

Prerequisites
- .NET 9 SDK
- Visual Studio 2022 with the .NET MAUI workload installed
- A SQL database (localdb/SQL Server/Azure SQL). Configure the connection string in the Web project’s appsettings.
- Android emulator or a Windows desktop target for the Hybrid app

Set up
1. Restore tools and workloads as needed:
   - .NET MAUI: https://learn.microsoft.com/dotnet/maui/get-started/installation
2. Database
   - Apply EF Core migrations from the Web project:
     ```
     dotnet tool restore
     dotnet ef database update --project ./MauiBlazorWeb/MauiBlazorWeb.Web
     ```
3. Start the Web project
   - In Visual Studio, right-click `MauiBlazorWeb.Web` and choose __Debug > Start without Debugging__ (or set as the startup project and press __Ctrl+F5__).
   - Open Swagger at https://localhost:7157/swagger (port may vary per `launchSettings.json`).
4. Create an account
   - Navigate to `https://localhost:7157/Account/Register` and register.
   - Confirm the account using the in-app confirmation link (no real email sender configured in development).
5. Start the MAUI Hybrid client
   - Right-click the `MauiBlazorWeb` project and choose __Set as Startup Project__.
   - Choose a debug target (Windows or an Android emulator) and press __F5__.
   - Sign in using the account created in step 4. Tokens are stored in device secure storage and refreshed as needed.

What to expect
- Anonymous users see limited navigation (Home/Login/Register).
- Authenticated users see shared pages (for example, “My Collection”) and role-dependent admin pages (for example, “Manage Shows”, “User Management”).
- The Hybrid client calls secure APIs using an authenticated `HttpClient`.
- Web logout is anti-forgery protected; Hybrid logout clears tokens from secure storage.

## Development notes

- Tech stack
  - ASP.NET Core 9, Blazor Web App, .NET MAUI Blazor Hybrid
  - ASP.NET Core Identity, EF Core (code-first)
  - Swagger/OpenAPI for API surface
- Patterns
  - DTOs + services in `MauiBlazorWeb.Shared` to enforce contract symmetry.
  - Repository/Service layering in `MauiBlazorWeb.Web` for persistence isolation.
  - Centralized error handling and diagnostics in the Hybrid client.
- Security
  - Identity-based auth with device secure storage for MAUI tokens.
  - Anti-forgery tokens for sensitive form posts in Web.
  - CORS is required for Hybrid-to-Web API calls; configure allowed origins as appropriate.
- Testing
  - Use Swagger to validate server endpoints.
  - Run Hybrid client against local Web server; ensure the Web server is running first.

## Contributing

We are not accepting outside contributions at this time. The repository is available for public review on GitHub; please refrain from opening pull requests.

## License

MIT. See `LICENSE.txt`.
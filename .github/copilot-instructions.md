# GitHub Copilot Instructions for LoanShark

## Build, Test, and Lint Commands

- **Run Full Stack (Recommended)**: Use .NET Aspire to run API, Web UI, and DB together:
  `cd src/LoanShark.AppHost && aspire start`
- **Run API Only**: `cd src/LoanShark.Api && dotnet run`
- **Run Web UI Only**: `cd src/LoanShark.Web && dotnet run`
- **Run Unit/Integration Tests**: `dotnet test src/LoanShark.Api.Tests/LoanShark.Api.Tests.csproj`
- **Run E2E Tests (Playwright)**: `dotnet test src/LoanShark.E2E.Tests/LoanShark.E2E.Tests.csproj`

## High-Level Architecture

LoanShark is a P2P lending platform built using the "Better Than Best" (BTB) methodology on the modern Microsoft ecosystem.

- **Stack**: .NET 10, ASP.NET Core API, Blazor Server Web UI (`LoanShark.Web`), a Shared UI library (`LoanShark.SharedUI`), and an upcoming .NET MAUI mobile app.
- **Database**: Entity Framework Core with Azure SQL / LocalDB.
- **Orchestration**: Fully managed via .NET Aspire (`LoanShark.AppHost`).
- **Core Domain**:
  - **Wallets & Ledger**: Internal wallets for users mapped 1-to-1. Balances are strictly managed via immutable `LedgerTransaction` records and use `RowVersion` for optimistic concurrency.
  - **Loan Marketplace**: Users can request loans, and multiple lenders can fund a single `LoanRequest` via `LoanInvestment`s.

## Key Conventions

- **.NET Aspire Workflows**:
  - Never run `dotnet run` on the AppHost project directly; always use `aspire start`.
  - If you encounter file lock errors during build (`MSB3491`, `CS2012`), run `aspire stop` first to release locks held by the Aspire background process before rebuilding.
- **Blazor & Authentication (ADR 0003)**: Blazor prerendering is intentionally disabled (`@rendermode="new InteractiveServerRenderMode(prerender: false)"`) to allow the `AuthMessageHandler` to synchronously read the JWT token from JSInterop without throwing 500 errors. Maintain this pattern for auth-dependent components.
- **FluentUI Events (ADR 0004)**: When using Microsoft FluentUI Blazor components, standard Blazor lower-case directives must be used (e.g., `@onclick` instead of `OnClick`), and components must be within an `InteractiveServer` render mode.
- **Database Migrations (ADR 0005)**: Schema initialization logic (`db.Database.EnsureCreated()` or migrations) is run explicitly during application startup (not just in development) to support zero-touch deployments in Azure Container Apps.
- **Database Identity (ADR 0002)**: Use Azure Entra ID (Managed Identities) rather than SQL connection string passwords. The app configures `SqlAuthenticationProvider` at startup.
- **Architecture Decision Records (ADRs)**: Important decisions follow the BTB approach and are logged in `docs/adr/`. Consult these before making large architectural shifts.

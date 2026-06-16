# Loan Shark (P2P Lending App)

A 'BTB' (Better Than Best) standard cross-platform P2P lending application built on the modern Microsoft ecosystem. It includes a backend API, robust financial transactions, and a Blazor-based frontend ready to be packaged for desktop and mobile natively via .NET MAUI.

## 🚀 Features
* **Authentication:** Secure JWT-based registration and login.
* **Loan Marketplace:** Borrowers can submit loan requests; Lenders can review and fully or partially fund active loans.
* **Wallet & Ledger:** Integrated internal wallet for each user to track exact deposits, withdrawals, and robust loan funding movement using Entity Framework Core.
* **Telemetry & Orchestration:** Fully integrated with .NET Aspire for instant local dev provisioning and cloud-ready telemetry.

---

## 🛠️ Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* SQL Server (LocalDB or Docker instance)
* [Aspire CLI](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling) (installed via \dotnet tool install -g Aspire.Cli\)

---

## 🏃 Running the Application

### Option A: Using Aspire (Recommended)
This is the easiest way to run the full stack (API, UI, and Database) natively with automatic orchestration and dashboarding.

1. Navigate to the AppHost directory:
   \\\ash
   cd src/LoanShark.AppHost
   \\\
2. Start the orchestration:
   \\\ash
   aspire start
   \\\
3. Open the **Aspire Dashboard** URL provided in the console to monitor logs, traces, and securely click into the Web UI.

### Option B: Manual Execution
If Aspire has permission issues or you'd prefer to test the services independently:

1. **Start the API:**
   \\\ash
   cd src/LoanShark.Api
   dotnet run
   \\\
   *Note: On first run in development, Entity Framework will automatically initialize and create the LocalDB SQL Database for you.*

2. **Start the Web UI:**
   \\\ash
   cd src/LoanShark.Web
   dotnet run
   \\\
3. **Access the Web App:** Open the URL printed by the Web project (usually \https://localhost:7082\).

---

## 🧪 Running Tests
The project features a dedicated \xUnit\ suite for testing API integration and backend resilience.
\\\ash
dotnet test src/LoanShark.Api.Tests/LoanShark.Api.Tests.csproj
\\\

---

## 📱 Future: Mobile App Stores (.NET MAUI)
Because the core UI is built via **Blazor** in the \LoanShark.SharedUI\ project, migrating to iOS and Android is seamless. 

**To package for mobile (Early Adopters):**
1. Add a `.NET MAUI Blazor Hybrid` project to the solution.
2. Add a project reference to `LoanShark.SharedUI`.
3. Drop the `<App />` or specific routing component directly into the MAUI `MainPage.xaml`.
4. Deploy directly to Apple App Store or Google Play Store.

---

## 🏛️ Architecture Decision Records (ADR) & The BTB Approach

We follow the **Better Than Best (BTB)** approach. We believe that what was considered the "best practice" yesterday can always be improved upon today. We maintain an open, infinite loop of betterment, continually optimizing our architecture even into production.

Our critical architectural choices, including lessons learned from cloud deployments and framework gotchas, are documented in our ADRs:
- [ADR 0001: Adopt the "Better Than Best" (BTB) Approach](docs/adr/0001-adopt-better-than-best-btb-approach.md)
- [ADR 0002: Use Azure SQL Entra ID (Managed Identities) Authentication](docs/adr/0002-use-azure-sql-entra-id-managed-identities.md)
- [ADR 0003: Disable Blazor Prerendering for Auth-Dependent JSInterop](docs/adr/0003-disable-blazor-prerendering-for-auth-interop.md)
- [ADR 0004: Standardize FluentUI Event Bindings](docs/adr/0004-standardize-fluentui-event-bindings.md)
- [ADR 0005: Manage Environment-Specific EF Core Migrations](docs/adr/0005-manage-environment-specific-ef-core-migrations.md)


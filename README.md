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

## 📝 Lessons Learned & Troubleshooting (Betterment Loop)

During cloud deployment and UI modernization, we discovered several critical gotchas to remember for future development:

1. **Azure SQL & Entra ID (Managed Identities)**:
   * **Issue:** `Cannot find an authentication provider for 'ActiveDirectoryDefault'` when EF Core tries to connect to Azure SQL.
   * **Fix:** You *must* manually install the `Microsoft.Data.SqlClient.Extensions.Azure` NuGet package and register it at startup (`SqlAuthenticationProvider.SetProvider(...)`). The base SQL client no longer ships with Azure AD logic built-in.
2. **Blazor JSInterop & Prerendering**:
   * **Issue:** Missing JWT auth tokens when rendering protected pages like `/wallet`. `AuthMessageHandler` crashes silently when trying to invoke `localStorage` via JSInterop.
   * **Fix:** Blazor prerendering occurs before the browser loads JS. Disable prerendering globally on `<Routes>` and `<HeadOutlet>` using `@rendermode="new InteractiveServerRenderMode(prerender: false)"`, or cache tokens in memory (via scoped state providers) so HTTP pipelines don't depend directly on JS runtime contexts during lifecycle initializations.
3. **FluentUI Event Bindings**:
   * **Issue:** FluentUI component events (like `<FluentButton>`) not firing.
   * **Fix:** Ensure you are using the standard Blazor lower-case `@onclick` directive, not the capitalized `OnClick` parameter which is often swallowed by the component wrapper context. Ensure `InteractiveServer` mode is applied so C# events trigger.
4. **Environment-Specific Migrations**:
   * **Issue:** 500 Server Errors because SQL tables are missing in production.
   * **Fix:** Ensure `db.Database.EnsureCreated()` (or EF Migrations) are executed outside of the `if (app.Environment.IsDevelopment())` block if you want Azure to auto-provision your tables on the first run.


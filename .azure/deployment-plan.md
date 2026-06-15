# LoanShark — Azure Deployment Plan

**Status:** Draft (awaiting approval)
**Recipe:** AZD (Aspire)
**Mode:** MODIFY (existing .NET Aspire AppHost)

## 1. Workspace Analysis
- .NET Aspire 13.4 application (AppHost: `src/LoanShark.AppHost`)
- Components:
  - `LoanShark.Api` — ASP.NET Core Web API (.NET 10)
  - `LoanShark.Web` — Blazor Web App (.NET 10), external HTTP endpoint
  - `LoanShark.Maui` — .NET MAUI desktop client (NOT cloud-deployable)
  - `LoanShark.SharedUI` — shared Razor components
  - `LoanShark.ServiceDefaults` — Aspire defaults
  - `sql` — SQL Server (Aspire `AddSqlServer().AddDatabase("sqldata")`)
- Tests: `LoanShark.Api.Tests`, `LoanShark.E2E.Tests`

## 2. Requirements
- Classification: MVP / dev workload
- Scale: low/dev
- Budget: minimize (dev tier)

## 3. Recipe Selection
- **AZD with Aspire** — required for AppHost-based projects. azd will auto-generate Bicep from AppHost.

## 4. Architecture (Azure)
| Component | Azure Service |
|-----------|---------------|
| `api` | Azure Container Apps |
| `web` | Azure Container Apps (external ingress) |
| `sqldata` | Azure SQL Database (replaces local SQL Server container) |
| Container registry | Azure Container Registry |
| Observability | Log Analytics + Application Insights (Aspire defaults) |
| Identity | User-assigned managed identity (azd default) |

**Notes:**
- The `maui-windows` `AddExecutable` resource is local-only and will be excluded from the Aspire manifest with `.ExcludeFromManifest()` for cloud publish. Local `aspire start` behavior is preserved.
- SQL Server resource will be published as Azure SQL via `.PublishAsAzureSqlDatabase()` to use managed SQL in Azure (Entra-only auth).

## 5. Configuration Decisions
- **Subscription:** _to be confirmed_
- **Location:** _to be confirmed_
- **Environment name:** `loanshark-dev`

## 6. Steps
1. Add `.ExcludeFromManifest()` to MAUI executable resource
2. Add `.PublishAsAzureSqlDatabase()` to SQL Server resource (Entra-only auth)
3. Run `azd init --from-code -e loanshark-dev`
4. Verify generated `azure.yaml` services section
5. Scan AppHost for `AddParameter` + `WithBuildArg` pattern (none expected)
6. Hand off to azure-validate

## Status Log
- [ ] Plan approved
- [ ] AppHost modified for cloud publish
- [ ] azd init complete
- [ ] Ready for Validation

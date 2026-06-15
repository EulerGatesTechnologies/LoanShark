# LoanShark — Azure Deployment Plan

**Status:** Validated
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
- **Subscription:** Pay-As-You-Go (`9800d7c0-aaa0-4db9-892d-99085cd5cd7b`)
- **Tenant:** Eulergates_Technologies_SA_Directory (`d1092155-2cb0-4d19-8acb-aee6f41790d1`)
- **Location:** South Africa North (`southafricanorth`)
- **Environment name:** `loanshark-dev`

## 6. Steps
1. Add `.ExcludeFromManifest()` to MAUI executable resource
2. Add `.PublishAsAzureSqlDatabase()` to SQL Server resource (Entra-only auth)
3. Run `azd init --from-code -e loanshark-dev`
4. Verify generated `azure.yaml` services section
5. Scan AppHost for `AddParameter` + `WithBuildArg` pattern (none expected)
6. Hand off to azure-validate

## Status Log
- [x] Plan approved
- [x] AppHost modified for cloud publish (MAUI excluded from publish, SQL → Azure SQL)
- [x] `dotnet build` succeeds
- [x] `azd init --from-code -e loanshark-dev` complete
- [x] `azure.yaml` generated with correct Aspire `app` service
- [x] AZURE_SUBSCRIPTION_ID, AZURE_LOCATION, AZURE_TENANT_ID set in azd env
- [x] AddParameter+WithBuildArg scan: none found
- [x] Ready for Validation

## 7. Validation Proof

| Check | Command | Result |
|-------|---------|--------|
| AZD installed | `azd version` | 1.25.5 ✅ |
| azure.yaml schema | `mcp_azure_mcp_azd validate_azure_yaml` | Valid against stable schema ✅ |
| Auth | `azd auth login --check-status` | Logged in as spdkheswa@outlook.com ✅ |
| Env values | `azd env get-values` | AZURE_ENV_NAME, AZURE_LOCATION (southafricanorth), AZURE_SUBSCRIPTION_ID, AZURE_TENANT_ID all set ✅ |
| Aspire pre-prov (Functions secrets) | `grep AddAzureFunctionsProject` | Not Functions — skipped ✅ |
| Provision preview | `azd provision --preview --no-prompt` | SUCCESS in 42s — RG, ACA Env, ACR, Log Analytics, Azure SQL Server ✅ |
| Build | `dotnet build src\LoanShark.AppHost\LoanShark.AppHost.csproj` | 0 errors, 26 warnings (NU1902 OTel advisories — non-blocking) ✅ |
| Docker context | n/a | Aspire-managed container builds (no user Dockerfile) ✅ |
| Package | `azd package --no-prompt` | SUCCESS ✅ |
| Static role review | Aspire-generated infra | azd will create user-assigned managed identity with ACR Pull + Azure SQL contributor role ✅ |

**Generated resources (preview):**
- Resource group: `rg-loanshark-dev`
- Container Apps Environment: `cae-bvvf3sgvstmpm`
- Container Registry: `acrbvvf3sgvstmpm`
- Log Analytics workspace: `law-bvvf3sgvstmpm`
- Azure SQL Server: `sql-bvvf3sgvstmpm` (Entra-only auth)

- [x] **Validated**

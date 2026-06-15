# ADR 0005: Manage Environment-Specific EF Core Migrations

## Status
Accepted

## Context
When deploying the LoanShark API to Azure Container Apps, we encountered a 500 Internal Server Error upon registration. The root cause was that the Azure SQL database lacked the necessary tables. Our `db.Database.EnsureCreated()` (and subsequent migration execution logic) was guarded inside an `if (app.Environment.IsDevelopment())` block, meaning it never executed in the production cloud environment.

## Decision
We decided to **extract schema initialization logic out of the development-only check** for our automated Azure deployments.
Moving forward, we will execute database creation/migration scripts explicitly during startup, regardless of the environment (or scoped safely based on explicit configuration flags like `ApplyMigrationsOnStartup`), to ensure that cloud instances auto-provision the required schema.

## Consequences
- **Positive:** Enables zero-touch deployments where the database schema is automatically updated or created upon the first spin-up of the container in Azure.
- **Negative:** Running schema changes automatically at application startup in production can be risky for highly available, multi-instance systems (potential race conditions or locks). In the future, as the platform scales (BTB betterment loop), this should be migrated to an explicit CI/CD pipeline step rather than happening at app startup.
# ADR 0002: Use Azure SQL Entra ID (Managed Identities) Authentication

## Status
Accepted

## Context
During deployment to Azure Container Apps, we needed a secure way for our .NET 8 API to connect to our Azure SQL database without storing raw credentials (passwords) in configuration files. The standard EF Core connection strings and basic SQL client do not natively handle Azure Active Directory (Entra ID) tokens without explicit configuration in newer .NET versions.

## Decision
We decided to use **Azure Entra ID (Managed Identities)** for database authentication. 
To achieve this, we:
1. Installed the `Microsoft.Data.SqlClient.Extensions.Azure` NuGet package.
2. Explicitly registered the authentication provider at application startup using:
   `SqlAuthenticationProvider.SetProvider(SqlAuthenticationMethod.ActiveDirectoryDefault, new ActiveDirectoryAuthenticationProvider());`

## Consequences
- **Positive:** Significantly improves security by eliminating passwords from our connection strings. The application authenticates using its cloud-native Managed Identity.
- **Negative:** Adds a slight setup overhead and dependency on the `Microsoft.Data.SqlClient.Extensions.Azure` package. Local development must emulate Managed Identity (e.g., via Azure CLI or Visual Studio credentials) or fall back to local connection strings.
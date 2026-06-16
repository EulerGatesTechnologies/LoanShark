# Database Schema Design (EF Core SQL Server)

## 1. Users & Wallets
* **User**
  * `Id` (Guid, PK)
  * `Email` (string, Unique)
  * `PasswordHash` (string)
  * `CreatedAt` (datetime)
* **Wallet** (1-to-1 with User)
  * `Id` (Guid, PK)
  * `UserId` (Guid, FK)
  * `Balance` (decimal(18,4)) - *Strictly managed via transactions*
  * `RowVersion` (timestamp) - *For optimistic concurrency control*

## 2. Loan Marketplace
* **LoanRequest**
  * `Id` (Guid, PK)
  * `BorrowerId` (Guid, FK to User)
  * `AmountRequested` (decimal(18,4))
  * `AmountFunded` (decimal(18,4))
  * `InterestRate` (decimal(5,2)) - *e.g., 15.00 for 15%*
  * `TermDays` (int)
  * `Status` (enum: Pending, FullyFunded, Active, Repaid, Defaulted)
  * `CreatedAt` (datetime)
* **LoanInvestment** (Allows multiple lenders to fund one loan)
  * `Id` (Guid, PK)
  * `LoanRequestId` (Guid, FK)
  * `LenderId` (Guid, FK to User)
  * `Amount` (decimal(18,4))
  * `CreatedAt` (datetime)

## 3. Financial Ledger (Immutable)
* **LedgerTransaction**
  * `Id` (Guid, PK)
  * `FromWalletId` (Guid?, FK) - *Null for external deposits*
  * `ToWalletId` (Guid?, FK) - *Null for external withdrawals*
  * `Amount` (decimal(18,4))
  * `Type` (enum: Deposit, Withdrawal, LoanFunding, Repayment, PlatformFee)
  * `ReferenceId` (Guid?) - *Links to LoanRequestId or LoanInvestmentId*
  * `Timestamp` (datetime)

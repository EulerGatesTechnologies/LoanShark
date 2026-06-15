# P2P Lending App ("Loan Shark") - MVP Plan

## 1. Core User Roles
* **Borrower:** Requests loans, views repayment schedules, makes payments.
* **Lender:** Browses loan requests, funds loans, tracks ROI/returns.

## 2. MVP Features
* **Authentication & Profiles:** Basic sign up/in, profile management.
* **Loan Marketplace:**
  * Borrowers create loan requests (Amount, Term in days/months, Proposed Interest Rate).
  * Lenders can view active requests and fund them (fully or partially).
* **Ledger & Repayment Tracking:**
  * Track when a loan is fully funded and active.
  * Basic amortization/repayment schedule.
  * Track payments made and outstanding balances.
* **Basic Wallet System (Simulated):**
  * Virtual balances for users to simulate deposits, funding, and withdrawals.

## 3. Tech Stack & Architecture (Microsoft Ecosystem / "BTB" Standard)
* **Backend:** ASP.NET Core 8 Web API
* **Database:** SQL Server via Entity Framework Core
* **Frontend (Cross-Platform):** Shared Blazor Components. Hosted via Blazor Web App (Web) and .NET MAUI Blazor Hybrid (iOS, Android, Windows, Mac).
* **Architecture Style:** Vertical Slice Architecture with CQRS (MediatR). Maximizes cohesion and rapid development compared to traditional layered architectures.
* **Orchestration:** .NET Aspire for instant telemetry, service discovery, and local dev containers.
* **Core Practices:** Domain-Driven Design (DDD) for the financial ledger to guarantee strict transactional invariants.

## 4. Post-MVP / Future Considerations
* Real payment gateway integration (Stripe, Plaid).
* KYC (Know Your Customer) identity verification.
* Credit scoring / Risk assessment.
* Late fees and default handling.

## 5. Next Phases
* [x] Choose Tech Stack
* [x] Database Schema Design
* [x] Scaffold Project
* [x] Implement Authentication & Profiles (API + Web)
* [x] Implement Loan Marketplace (API + Web)
* [x] Implement Ledger & Wallet (API + Web)
* [x] Run Application & Local Setup

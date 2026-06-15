# ADR 0001: Adopt the "Better Than Best" (BTB) Approach

## Status
Accepted

## Context
As we develop and deploy the LoanShark platform, we consistently encounter challenges and edge cases that push the boundaries of standard industry "best practices" (e.g., Azure Container Apps deployments, Blazor Interactive Server render modes with Entra ID, etc.). Simply following the documented "best practice" at a given time is often insufficient because technology and requirements constantly evolve. 

We need a structured way to capture our learnings, push boundaries, and continuously improve our architecture and development processes.

## Decision
We will adopt the **"Better Than Best" (BTB) approach** as our core architectural and developmental philosophy. 

The BTB approach dictates that:
1. **Infinite Betterment Loop**: We do not stop at "industry standard" or "best practice." What was considered the best yesterday can be bettered today. We remain in an open, continuous loop of improvement infinitely through development, into production, and beyond.
2. **Documenting Learnings as ADRs**: Every significant architectural pivot, lesson learned, or constraint workaround will be documented as an Architecture Decision Record (ADR). This ensures our betterment loop is institutionalized and easily shareable.
3. **Challenging the Status Quo**: We will actively look for ways to optimize, secure, and streamline beyond what is strictly required, applying these enhancements back into our foundational architecture.

## Consequences
- **Positive:** Creates a culture of continuous learning and architectural resilience. Ensures that hard-learned lessons (like Azure SQL Entra ID configurations or Blazor prerendering gotchas) are captured permanently and used to better our baseline.
- **Negative:** Requires discipline to consistently document and review decisions via ADRs.

## Application
All future technical decisions that resolve complex challenges or establish new patterns for the application will be recorded in this `docs/adr` directory, serving as our BTB knowledge base.
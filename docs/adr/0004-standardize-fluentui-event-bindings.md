# ADR 0004: Standardize FluentUI Event Bindings

## Status
Accepted

## Context
During the migration of our native HTML UI components to Microsoft FluentUI Blazor components, we encountered issues where buttons (e.g., the Register button) were completely unresponsive. The issue stemmed from an incorrect parameter mapping: we used `OnClick` (capitalized) instead of the standard Blazor directive `@onclick`.

## Decision
We mandate the standardization of **standard Blazor lower-case directives (e.g., `@onclick`)** for all FluentUI components unless the component specifically exposes a custom `EventCallback` parameter that necessitates differently cased bindings.
Additionally, all interactive components must reside within a component or page that has `InteractiveServer` render mode active.

## Consequences
- **Positive:** Ensures consistent event firing across the UI. Reduces debugging time for "unresponsive" buttons.
- **Negative:** Developers migrating from older or different component libraries might default to camelCase or PascalCase event names, requiring careful code review.
# ADR 0003: Disable Blazor Prerendering for Auth-Dependent JSInterop

## Status
Accepted

## Context
Our Blazor application uses JWT tokens for authentication. The token was being retrieved via JavaScript Interop (`localStorage`) inside a custom `AuthMessageHandler` (a delegating handler for HTTP client requests). 

When a user navigated directly to a protected page (e.g., `/wallet`), the application threw a 500 Internal Server Error. This occurred because Blazor defaults to Static Server Rendering (Prerendering), which executes the component lifecycle on the server *before* a SignalR circuit or browser JavaScript context is established. Calling `IJSRuntime` during this phase fails silently or crashes the pipeline.

## Decision
We decided to **disable prerendering globally** and rely on an in-memory token cache.
1. In `App.razor`, we applied `@rendermode="new InteractiveServerRenderMode(prerender: false)"` to the `<HeadOutlet>` and `<Routes>` components.
2. We refactored our `JwtAuthenticationStateProvider` to hold the JWT token in memory once loaded, so the `AuthMessageHandler` can synchronously read the token without relying directly on JSInterop during HTTP outbound calls.

## Consequences
- **Positive:** Resolves 500 errors on initial page loads for authenticated routes. Streamlines HTTP handler logic by avoiding async JSInterop.
- **Negative:** Disabling prerendering means the user will briefly see a blank screen or a loading state while the SignalR circuit connects, slightly increasing the Time to Interactive (TTI). However, for an authenticated line-of-business app, correctness trumps immediate SEO-friendly rendering.
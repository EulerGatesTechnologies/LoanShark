# ADR 0006: Force LongPolling for Blazor Server in Azure Container Apps

## Status
Accepted

## Context
When deploying a Blazor InteractiveServer app to Azure Container Apps (ACA) using `azd` and Aspire, the app defaults to WebSockets for its SignalR connection. If there are multiple replicas or load balancing involved, the WebSocket connections can drop or fail to handshake properly if sticky sessions are not enabled. 

In our environment, the ACA environment and the container apps were provisioned via `azd` automatically, and configuring sticky sessions dynamically required tweaking the Azure provisioning APIs or Bicep files directly, which encountered assembly/namespace errors in Aspire's `Azure.Provisioning.AppContainers` wrapper.

## Decision
In keeping with the **Better Than Best (BTB)** philosophy, we chose to bypass the infrastructure limitation entirely by configuring the Blazor application to gracefully handle non-sticky environments.

We updated `App.razor` to force `LongPolling` as the transport mechanism for the SignalR circuit:

```javascript
<script src="_framework/blazor.web.js" autostart="false"></script>
<script>
    Blazor.start({
        circuit: {
            configureSignalR: function (builder) {
                builder.withUrl("_blazor", {
                    transport: 4 // signalR.HttpTransportType.LongPolling
                });
            }
        }
    });
</script>
```

## Consequences
- **Positive:** Resolves the WebSocket 404 handshake drop issues. 
- **Positive:** App becomes resilient to load balancer shifts without requiring sticky session infrastructure changes, making it portable.
- **Negative:** LongPolling has slightly higher latency and overhead compared to WebSockets, but it's an acceptable tradeoff for the reliability gained without altering the underlying infrastructure definitions in Aspire 13.4.

# Acme Billing & Notifications Platform

Internal order-to-cash platform. This monorepo contains the billing API and the
notification/webhook delivery service, plus the infrastructure definitions used
to run them.

| Path | Description |
|------|-------------|
| `services/billing-api-dotnet/` | ASP.NET Core billing API (invoices, charges, report export, partner webhooks) |
| `services/notifications-node/` | Node.js service for reminders, customer webhooks and PDF statements |
| `infra/` | Docker Compose, Terraform (AWS) and Kubernetes manifests |
| `.github/workflows/` | CI pipeline |
| `docs/POC_MANIFEST.md` | Inventory of planted findings (security-tool PoC only) |

## Security tool PoC testbed

This repository is a **synthetic testbed** for comparing Checkmarx, Aikido Security and
Semgrep. It contains intentional vulnerabilities, vulnerable dependencies, EOL runtimes,
IaC misconfigurations and fake credentials. **Do not deploy it.** All secrets are fake.

Nothing needs to be installed to scan it. Open the folder in VS Code and point each
scanner at the repository root. See `docs/POC_MANIFEST.md` for the expected findings.

## Local development (for reference only)

```bash
# billing API
cd services/billing-api-dotnet && dotnet run --project src/BillingApi

# notifications
cd services/notifications-node && npm ci && npm start
```

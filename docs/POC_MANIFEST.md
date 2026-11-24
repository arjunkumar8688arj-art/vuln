# PoC Manifest - Planted Findings

Synthetic testbed for comparing **Checkmarx**, **Aikido Security** and **Semgrep**.
All credentials are fake. Do not deploy this repository.

| Category | Planted count |
|----------|--------------:|
| SAST | 10 |
| SCA | 10 |
| EOL / deprecated runtime | 5 |
| IaC / container misconfiguration | 5 |
| Secrets | 5 |
| **Total** | **35** |

Paths are relative to the repo root. Line numbers are accurate for the files as shipped.

## 1. SAST (10)

| ID | Category (CWE) | File | Line / function | Likely coverage |
|----|----------------|------|-----------------|-----------------|
| SAST-01 | SQL injection (CWE-89) | `services/billing-api-dotnet/src/BillingApi/Data/InvoiceRepository.cs` | L49, `SearchByCustomerAsync` | Checkmarx, Semgrep, Aikido |
| SAST-02 | Path traversal (CWE-22) | `services/billing-api-dotnet/src/BillingApi/Controllers/ReportsController.cs` | L44, `Download` | Checkmarx, Semgrep, Aikido |
| SAST-03 | Weak hash, MD5 (CWE-328) | `services/billing-api-dotnet/src/BillingApi/Services/ApiKeyHasher.cs` | L13, `Hash` | Checkmarx, Semgrep, Aikido |
| SAST-04 | Insecure deserialization (CWE-502) | `services/billing-api-dotnet/src/BillingApi/Services/WebhookProcessor.cs` | L16, `Parse` | Checkmarx, Semgrep, Aikido |
| SAST-05 | TLS certificate validation bypass (CWE-295) | `services/billing-api-dotnet/src/BillingApi/Services/PaymentGatewayClient.cs` | L16, constructor | Checkmarx, Semgrep, Aikido |
| SAST-06 | Open redirect (CWE-601) | `services/billing-api-dotnet/src/BillingApi/Controllers/AuthController.cs` | L19, `Callback` | Checkmarx, Semgrep |
| SAST-07 | OS command injection (CWE-78) | `services/notifications-node/src/routes/reports.js` | L14, `POST /reports/pdf` | Checkmarx, Semgrep, Aikido |
| SAST-08 | SSRF (CWE-918) | `services/notifications-node/src/routes/webhooks.js` | L21, `POST /webhooks/test` | Checkmarx, Semgrep |
| SAST-09 | Reflected XSS (CWE-79) | `services/notifications-node/src/routes/notifications.js` | L19, `GET /notifications/preview` | Checkmarx, Semgrep, Aikido |
| SAST-10 | Insecure randomness (CWE-330) | `services/notifications-node/src/services/tokenService.js` | L24, `generateResetToken` | Checkmarx, Semgrep, Aikido |

## 2. SCA (10) - direct dependencies

| ID | Ecosystem | Package | Version | Manifest | Example advisory |
|----|-----------|---------|---------|----------|------------------|
| SCA-01 | NuGet | Newtonsoft.Json | 12.0.1 | `BillingApi.csproj` L11 | GHSA-5crp-9r3c-p9vr (DoS) |
| SCA-02 | NuGet | System.Net.Http | 4.3.0 | `BillingApi.csproj` L12 | CVE-2018-8292 |
| SCA-03 | NuGet | System.Text.RegularExpressions | 4.3.0 | `BillingApi.csproj` L13 | CVE-2019-0820 |
| SCA-04 | NuGet | log4net | 2.0.8 | `BillingApi.csproj` L14 | CVE-2018-1285 |
| SCA-05 | NuGet | SharpZipLib | 1.2.0 | `BillingApi.Reports.csproj` L9 | CVE-2021-32840 and related |
| SCA-06 | npm | lodash | 4.17.15 | `package.json` / `package-lock.json` | CVE-2020-8203, CVE-2021-23337 |
| SCA-07 | npm | axios | 0.21.1 | `package.json` / `package-lock.json` | CVE-2020-28168, CVE-2021-3749 |
| SCA-08 | npm | jsonwebtoken | 8.5.1 | `package.json` / `package-lock.json` | CVE-2022-23529 and related |
| SCA-09 | npm | minimist | 1.2.5 | `package.json` / `package-lock.json` | CVE-2021-44906 |
| SCA-10 | npm | marked | 0.3.5 | `package.json` / `package-lock.json` | CVE-2017-1000427, ReDoS advisories |

Transitive npm packages in the lockfile are pinned to patched versions, so they should not add findings.

## 3. End-of-Life (5)

| ID | Item | File | Line |
|----|------|------|------|
| EOL-01 | .NET 6 (`net6.0`) | `services/billing-api-dotnet/src/BillingApi/BillingApi.csproj` | L4 |
| EOL-02 | .NET Core 3.1 (`netcoreapp3.1`) | `services/billing-api-dotnet/src/BillingApi.Reports/BillingApi.Reports.csproj` | L4 |
| EOL-03 | .NET 5 SDK pinned in `global.json` | `services/billing-api-dotnet/global.json` | L3 |
| EOL-04 | Node.js 14 base image | `services/notifications-node/Dockerfile` | L2 |
| EOL-05 | Node.js 16 in CI | `.github/workflows/ci.yml` | L31 |

## 4. IaC / container (5)

| ID | Issue | File | Line |
|----|-------|------|------|
| IAC-01 | Unpinned `:latest` base image | `services/billing-api-dotnet/Dockerfile` | L2 |
| IAC-02 | Container runs as root (no `USER`) | `services/notifications-node/Dockerfile` | whole file |
| IAC-03 | `privileged: true` container | `infra/docker-compose.yml` | L18 |
| IAC-04 | S3 bucket ACL `public-read` | `infra/terraform/main.tf` | L58 |
| IAC-05 | SSH (22) open to `0.0.0.0/0` | `infra/terraform/main.tf` | L96 |

`infra/k8s/notifications-deployment.yaml` is intentionally hardened (clean control).

## 5. Secrets (5) - all fake

| ID | Type | File | Line |
|----|------|------|------|
| SEC-01 | Stripe-style live API key | `services/billing-api-dotnet/src/BillingApi/appsettings.json` | L14 |
| SEC-02 | SQL Server connection string with password | `services/billing-api-dotnet/src/BillingApi/appsettings.Development.json` | L8 |
| SEC-03 | AWS access key + secret pair | `services/notifications-node/.env.example` | L11-12 |
| SEC-04 | Hardcoded JWT signing key fallback | `services/notifications-node/src/config.js` | L7 |
| SEC-05 | Slack webhook URL | `services/notifications-node/test/fixtures/webhook-sample.json` | L4 |

## Notes on expected noise

Each tool also applies its own best-practice rules, so reported totals may exceed the
counts above. Examples: missing `HEALTHCHECK` in Dockerfiles, Terraform logging or
versioning rules, `AllowedHosts: "*"`, `marked` rendering unsanitized HTML, or the
EOL base image also being reported as a vulnerable-image finding. Treat anything not
listed here as noise or a bonus finding when benchmarking.

The npm lockfile was hand-authored (no registry access) and has no `integrity` hashes.
Scanners parse it fine; run `npm install --package-lock-only` later if you want a
registry-generated one. NuGet manifests are `.csproj` only (no `packages.lock.json`).

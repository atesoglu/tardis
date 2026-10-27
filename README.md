# Tardis – Unified DevOps Intelligence Platform

> A centralized platform for AI-powered PR automation, task management, observability & analytics, and hybrid workflow orchestration — built for enterprise-scale Azure DevOps environments.

## Architecture Overview

- **Gateway** (pure-edge / BFF): Auth, CORS, security headers, rate-limiting, webhooks, BFF aggregation. **DB-less.**
- **Platform** (backend HTTP API): Query & command endpoints; owns database access.
- **Workers** (background services): ingestion, scoring, notifications, scheduled jobs (HTTP-only end-to-end for MVP).
- Frontend: React SPA (`frontend/`) behind the Gateway (BFF).
- IaC: Terraform (Kubernetes-focused).
- Authentication: Azure AD (Entra ID) only.

## Tech Stack

- **Runtime**: .NET 10 (Web APIs)
- **Platform**: Kubernetes (provider-/cloud-agnostic)
- **IaC**: Terraform
- **Auth**: Azure AD (Entra ID OAuth2/OIDC only - no B2C)
- **Frontend**: React SPA, separated in `frontend/`
- **Gateway**: pure-edge (no DB; no Ocelot)
- **Backend API**: `Platform` HTTP service
- **Background work**: `Workers` hosted services
- **Persistence option**: optional shared `Persistence` library
- **Local dev**: Podman + `docker-compose.yaml` (provider-agnostic) with Postgres for local DB

## Repository Structure

```
/Tardis
├── backend/                 # .NET 10 backend services + .slnx
│   ├── src/Gateway/         # Edge/BFF (pure-edge, DB-less)
│   └── Tardis.slnx
├── frontend/                # React SPA (BFF-friendly, isolated)
├── infra/terraform          # Terraform for GitOps/IaC
├── docs/roadmap.md          # Single source of truth for direction
└── Directory.Build.*        # Shared MSBuild/cpm configuration
```

## Status

Early scaffold phase. All prior sub-projects marked as `revisit`.

## Notes

- ADRs are deliberately omitted at this early stage; decisions tracked in docs/roadmap.md.
- Common/shared layer removed; introduce per-service domain layers later.
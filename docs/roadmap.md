# Tardis — Project Roadmap (Single Source of Truth)

## Tech Direction (Updated)

- Backend: .NET 10 web services running on Kubernetes (not Azure Functions)
- IaC: Terraform (not Bicep)
- Gateway: pure-edge, no Ocelot
- Auth: Azure AD (Entra) only
- Frontend: React SPA, separated into `frontend/` (BFF design TBD)
- Common layer: removed for now (no shared libs)
- No ADRs for now; decisions captured here & in README

---

## Backend Use Cases

### Module: AI PR Automation
- PR ingestion & triage via webhook subscriptions
- AI-generated review comments (LLM)
- Auto-merge / auto-close based on risk threshold
- Risk scoring service (0-100)
- Auto-label & auto-assign reviewers by code ownership

### Module: Task Management Assistant
- Periodic work-item sync across squads (Azure DevOps REST)
- Rule-based auto-assignment (skills × capacity × team availability)
- SLA tracking & escalation notifications (Teams/Slack/email)
- Capacity heatmaps & auto-rebalancing suggestions
- Squad-level live task board

### Module: Observability & Analytics Dashboard
- DORA metrics: lead time, deploy frequency, MTTR, change failure rate
- Pipeline performance & bottleneck analysis
- Metrics pipeline (event → Synapse/ADX → Power BI Embedded)
- Exportable reports (PDF/Excel) and Teams/SharePoint embedding
- Persona-aware views (engineer vs squad lead)

### Module: Hybrid Workflow Orchestrator
- Per-squad office/remote policy engine
- Pipeline trigger orchestration respecting hybrid policies
- What-if simulation services (forecast delays)
- Manual override + audit trails

### Module: Platform Administration / Onboarding
- Azure DevOps bootstrap (org/projects/repos, secure defaults)
- Standard pipeline & branching-strategy templates
- Security posture endpoint (branch policies, gates, secrets scan status)
- IaC provisioning & environment promotion playbook
- Self-service onboarding checklist per squad
- Migration / observability runbooks

### Pers Personas / Stakeholders (from posting)
Engineers, Squad Leads, Scrum Masters/POs/Solution Architects, Platform maintainers

---

## Additional Value (Security Posture Governance — Mission Extension)

### Module: Security Posture Governance (new 2026 addition)

A generic regulated enterprise should offer “secure by default” at scale. Centralising security signals into a single persona-view keeps 400+ squads on track without friction.

- **Secret-leak detection hooks**: Azure Key Vault–ready secrets rotation events, scanning pipeline outputs
- **Container image signing & SLSA evidence**: Signed images, SBOM per Release, Cosign integration
- **Dependency scanning telemetry**: CVE/SCA surface consolidated across repositories + squad scorecards
- **Branch & release policies validation**: Monitoring policy drift against Golden Path templates
- **Risk score** combining dependency + PR risk + policy drift for daily scorecard
- Dashboards: Platform / Squad / Repo lens; auto-notify via Teams on threshold breach

---

## Additional Value (DevEx Analytics — Mission Extension)

### Module: Developer Experience Analytics (new 2026 addition)

Investing in **flow metrics** across 400 squads is increasingly treated as the leading indicator of delivery health, often more sensitive than raw activity counts. For a platform already aggregating PR + work-item + pipeline metadata, surfacing it as a **programmatic analytics surface** is low-effort, high-impact.

- Time-based flow metrics: Pickup time, First-review time, Cycle time (p50/p90), Flow efficiency
- Loop-detection: idle PRs (>48h, 7d, 14d), abandoned branches, orphan branches
- WIP optimisation: bottleneck warning, crowding (too-many-unmerged-PRs) detection
- Distractions: Context-switching detection (PR↔task oscillation, high review load)
- Segmentations: by squad, persona (engineer vs reviewer), skill tag, size-of-change
- Drill-initated tickets with recommended interventions (reassign, ping, auto-merge after explicit gate)

### New backend endpoints (placeholder)

- `GET /api/devex/metrics?period=30d&squad=<slug>`
- `GET /api/devex/idle-prs?since=...`
- `POST /api/devex/focus-report` (snapshot via BFF/aggregator)

*Note:* OpenTelemetry signals will be leveraged (suitable correlations available from existing backend observability pipeline).

### Boundary decision: messaging vs HTTP (captured)

- **All internal commands and reads** — HTTP synchronous end-to-end (GET/POST), no queues/Service Bus at MVP.
- **Gateway** = pure-edge (auth, CORS, security headers, rate-limits, webhooks, BFF aggregation), DB-less, and only talks HTTP to `Tardis.Api`.
- **BFF/Read side:** HTTP GET/POST endpoints that shape dashboard payloads in one round-trip (recommended by the user).
- **Long-running & heavy workloads:** Keeping them on sync HTTP for the MVP simplifies tracing/ops; if backlog/p99 needs rise, revisit async via a queue later.
- *Alternative parked, not in scope for now:* queue-side topics + CQRS eventual consistency - revisit only after traffic pain.

---

## Local Development Environment

- Container runtime: **Podman** (Fedora 44 friendly)
- Compose file (provider-agnostic): `docker-compose.yaml` (works with Podman via `podman-compose`/`podman compose`)
- Database: Postgres (cloud-agnostic by default, provider choice later)
- Orchestrated tilt/workflows later if required

## Backend Project Topology  

### Selected project names
- **Gateway** — edge/BFF, auth/CORS/security, rate-limit, webhooks, BFF aggregation; DB-less.
- **Platform** — HTTP endpoints (queries + commands), talks to database, domain reads.
- **Workers** — background hosted services (ingest, scoring, task assignment, notifications).
- **Persistence** — shared data-access library (referenced by Platform and Workers).

### Rules
- Three distinct deployable units with independent release cadences.
- HTTP-only end-to-end (MVP); no queues today.
- Gateways never touch DB directly.
- Workers trigger via HTTP to Platform (no direct DB mutations by Gateway).
- Persistence is optional initially but becomes the shared library when DB-access needs replicate.

- Multi-tenant aware from day one (squad/tenant claim propagated in JWT, enforced in Platform filters).

---

## Prioritized MVP Tasks (phase-aligned)

### P0 – Sprint 1–2 (core critical path)
1. PR ingestion webhook + verification endpoint (Gateway→Platform command)
2. AI-risk scoring service scaffold (neutral vendor, no provider selected yet) (Platform)
3. Work-item sync loop (Workers) + auto-assignment rule v0 (Platform command)
4. Minimal observability feed: lead time + deploy frequency surface from Platform GETs
5. Auth/CORS/rate-limit middleware at Gateway incl. squad claim propagation
6. BFF `GET /bff/dashboard` aggregate for authenticated user (Platform query)

### P1 – Sprint 3 (orchestration)
7. Hybrid policy engine + What-if simulation service
8. Security Posture scorecard (secrets + CVE + policies)

### P2 – Sprint 4+ (observability deep)
9. DevEx Analytics endpoints (flow metrics / idle PRs)
10. Queue conversion: Webhooks → Workers queue ONLY IF latency pain arises (currently parked; HTTP-only MVP)

---

## Epic Breakdown (sub-tasks / vertical slices)

### Epic E1 — PR Automation (ingest + scoring + triage)
- 1.1 Webhook verify (Gateway): validate HMAC / AAD JWT
- 1.2 Endpoint `POST /api/pr-events` (Platform) — accept a validated payload
- 1.3 Persist raw events + state snapshot (Persistence)
- 1.4 AI scoring trigger (Workers background job, pluggable provider interface)
- 1.5 Auto-label / auto-close / auto-assign decision tables + rules
- 1.6 Risk thresholds config (per squad/per repo)
- 1.7 Outbound actions hook: applied to Azure DevOps via platform client
- 1.8 Audit trail of automated decisions

### Epic E2 — Work-Item Management
- 2.1 Periodic work-item sync (Workers) via Azure DevOps REST
- 2.2 Basic auto-assignment rule engine (policy × skills × capacity)
- 2.3 `POST /api/items/assigned/auto/{ruleVersion}` (Platform)
- 2.4 SLA tracking + Teams/Slack/email notification hook (Workers)
- 2.5 Capacity heatmap + signal of overload
- 2.6 Rebalancing suggestion (rule-driven manual action)
- 2.7 BFF shape: `GET /bff/tasks/overview`

### Epic E3 — Minimum Observability Feed
- 3.1 Data model/metric ingestion surface (Platform)
- 3.2 Feed ingestion: lead time, deploy frequency captured from hooks/ingress
- 3.3 BFF `GET /bff/metrics/summary`
- 3.4 Squad filter + personal dashboard cards

### Epic E4 — BFF + Auth edge
- 4.1 Gateway Minimal API + token validation + secure headers + CORS
- 4.2 Squad claim propagation
- 4.3 Rate limit middleware
- 4.4 Edge `GET /bff/{view}` composition (aggregate to avoid front-end N+1)

### Epic E5 — Hybrid Orchestration (P1)
- 5.1 Policy model + per-squad schedule definitions
- 5.2 What-if dry-run API
- 5.3 Trigger orchestration respecting hybrid policy
- 5.4 Manual override + journal

### Epic E6 — Security Posture Scorecard (P1)
- 6.1 Secret rotation detection signals from Key Vault
- 6.2 Dependency / SCA ingest (Python/PowerShell wrappers later)
- 6.3 Policy-drift detection vs Golden Path
- 6.4 Risk score composite API
- 6.5 Squad-level dashboards + Teams alert

### Epic E7 — DevEx Analytics (P2)
- 7.1 PR flow metrics (p50/p90 cycle time)
- 7.2 Idle/abandoned branch detectors
- 7.3 BFF defocus-report trigger

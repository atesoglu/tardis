# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Initial project structure and repository foundation
- Solution file (`Tardis.sln`) with 6 projects + 6 test projects
- GitHub Actions CI/CD pipeline (`.github/workflows/ci.yml`)
- Azure Pipelines alternative (`azure-pipelines.yml`)
- Dependabot configuration for automated dependency updates
- CodeQL security scanning configuration
- Comprehensive `.gitignore`, `.dockerignore`, `.editorconfig`
- Directory.Build.props for centralized package management
- Global.json for .NET SDK version pinning
- MIT License and project README
- Project roadmap (`docs/roadmap.md`)
- Architecture Decision Records directory (`docs/adr/`)

### Infrastructure
- Terraform placeholder in `infra/terraform/` (Bicep removed; Azure Functions removed):
- Dockerfile for the Gateway experimental/placeholder removed too; now plain K8s-oriented build packaging

### Backend Services (Scaffolding)
- **Gateway**: Edge/BFF (auth, CORS, security headers, webhooks) — pure ASP.NET Core, DB-less
- **Platform**: HTTP API surface
- **Workers**: Background hosts (ingestion, scoring, notifications)
- **Persistence**: Shared data-access layer

### Frontend (Dashboard)
- React SPA root at `dashboard/` (placeholder, BFF-friendly)

## [0.1.0] - 2026-10-27

### Added
- Repository initialization with complete foundation
- All scaffolding ready for feature development

---

## Release Process

1. Update version in `Directory.Build.props` (or use GitVersion)
2. Update this CHANGELOG.md
3. Create Git tag: `git tag -a v0.1.0 -m "Release v0.1.0"`
4. Push tag: `git push origin v0.1.0`
5. GitHub Actions will build, test, and create release

## Commit Message Convention

This project follows [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>[optional scope]: <description>

[optional body]

[optional footer(s)]
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation only
- `style`: Formatting, missing semicolons, etc.
- `refactor`: Code change that neither fixes a bug nor adds a feature
- `perf`: Performance improvement
- `test`: Adding missing tests
- `chore`: Maintenance, build process, tools
- `ci`: CI/CD changes
- `build`: Build system or dependencies
- `revert`: Reverts a previous commit

Examples:
```
feat(pr-automation): add AI-powered risk scoring for PRs
fix(dashboard): resolve hydration mismatch on settings page
docs(roadmap): update timeline for Phase 3
ci: add CodeQL security scanning
```
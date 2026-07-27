# LTI Advantage Roadmap — Deferred Work (Tier 2 + Tier 3)

> Companion to `2026-05-03-lti-tier1-dynreg-jwks-submissionreview.md`.
> Captures the LTI 1.3 / LTI Advantage features the LtiAdvantage library does NOT yet implement, beyond Tier 1, so we don't have to re-derive the gap analysis next time.
>
> Authored 2026-05-03 against spec status as of that date. Re-verify spec versions before picking anything up — Candidate Final specs sometimes get breaking edits before Final Release.

---

## Tier 2 — Finalized public specs, narrower audience

These are stable, public, and implementable. Pick one up only when a downstream consumer asks for it; otherwise the value-to-effort ratio is lower than Tier 1.

### Course Groups Service 1.0
- Spec: https://www.imsglobal.org/spec/lti-gs/v1p0
- Adds: read-only group membership for a context (sibling of NRPS)
- Scope: `https://purl.imsglobal.org/spec/lti-gs/scope/contextgroup.readonly`
- Library shape: mirror NRPS — domain models under `src/LtiAdvantage/CourseGroupsService/`, `GroupsControllerBase` + `SetsControllerBase` under `src/LtiAdvantage.AspNetCore/CourseGroupsService/`
- Estimated size: similar to NRPS (a few days of work)

### Proctoring Services 1.0
- Spec: https://www.imsglobal.org/spec/proctoring/v1p0
- Adds three message types: `LtiStartProctoring`, `LtiStartAssessment`, `LtiEndAssessment`
- Library shape: three new request types under `src/LtiAdvantage/Lti/`, plus claim value types under `src/LtiAdvantage/Proctoring/`. No new HTTP service endpoints — it's all launch-flow.
- Caveats: only useful if a consumer ships an assessment platform AND a proctoring tool. The session-coordination flow (assessment-control-protocol) is non-trivial — review §5 of the spec carefully before estimating.
- Estimated size: medium (~1 week including reference-JSON tests for each message type)

### Asset Processor 1.0
- Spec: https://standards.1edtech.org/lti/specifications/proposals/asset-processor/specification (Candidate Final)
- Adds three message types: `LtiAssetProcessorRequest`, `LtiReportReviewRequest`, `LtiEulaRequest`, plus a "report" content type
- Caveat: Candidate Final, not yet Final Release as of 2026-05. Wait for the Final Release vote unless a consumer commits to it.
- Library shape: similar to Proctoring — message types + claim value types
- Estimated size: medium

---

## Tier 3 — Member-only or draft specs, defer

These are either behind 1EdTech membership login or still in early draft. Don't speculatively implement.

### Member-only Candidate Final
- **Platform Notification Service (PNS) 1.0** — https://www.imsglobal.org/spec/lti-pns/v1p0/main/ (membership required to read full spec). Async platform→tool messaging; carries Context Copy notice and other notices. Implement IF a consumer is doing course-copy-aware tools.
- **Link and Content Service 1.0** — Candidate Final, member-only. Tool-published content catalog.
- **Caliper Analytics Connector 1.0** — Bridges LTI launches to Caliper events. Member-only.
- **AfA PNP Connector Service 1.0** — https://www.imsglobal.org/sites/default/files/spec/afa/3p0/connector_service/1edtech-ltipnp-servicev1.html — Personal Needs and Preferences for Accessibility. Niche, only matters in K-12 / accessibility-focused deployments.
- **Data Privacy Launch 1.0** — Base Document, member-only.

### Draft (do NOT implement yet)
- **OIDC Login with LTI Client-Side postMessages 0.1** — draft
- **LTI Client-Side postMessages 0.1** — draft
- **LTI postMessage Storage 0.1** — draft

These three together are an emerging pattern for cookieless launches in third-party iframe contexts (Safari ITP, Chrome 3p-cookie deprecation). Worth tracking — when they hit Candidate Final, this could move up to Tier 1 quickly because the cookieless-iframe problem is real and growing.

### AI / GenAI
1EdTech has NOT published an LTI GenAI service as of 2026-05. There is only:
- AI-Generated Content Best Practices v1.0 — https://www.imsglobal.org/resource/AI-Generated_Content_Best_Practices/v1p0
- TrustEd Apps GenAI Data Rubric — https://www.1edtech.org/standards/ai-rubric (expanded version expected early 2026 — re-check)

Nothing to build in the library against this until 1EdTech publishes a launch-shaped GenAI spec.

---

## Open quality gaps (independent of new specs)

Worth doing in a future minor — these aren't spec features, but they'd make the library easier to consume:

1. **First-class ID-token signature validation helper.** The library parses ID tokens but assumes upstream JWT bearer middleware does signature verification against the platform's JWKS. A built-in `LtiTokenValidator` that fetches+caches the platform JWKS and validates `iss`/`aud`/`exp`/`nonce` would lower the integration bar. Likely lives in `src/LtiAdvantage.IdentityModel/`.
2. **Platform-side OpenID configuration controller.** `/.well-known/openid-configuration` for platforms — not strictly required (most platforms publish it through their CMS), but a `OpenIdConfigurationControllerBase` would close the symmetry with the new `JwksControllerBase`.
3. **Concrete in-memory `IJwksKeyStore`.** Ship one boring reference implementation so getting started doesn't require writing one.
4. **Custom-variable substitution coverage audit.** `CustomPropertySubstitutions.cs` is large but predates several spec additions (e.g. `Context.id.history` for course-copy traceability). Audit against the current spec's variables list.
5. **Async controller base hardening.** The existing `…ControllerBase` classes have `try/catch/finally` blocks that catch `Exception` and emit a `ProblemDetails`. Audit for `OperationCanceledException` (currently treated as 500 — should propagate).

---

## Decision log

- **2026-05-03** — Decided Tier 1 (Submission Review + JWKS + Dynamic Registration) for next minor. Plan: `2026-05-03-lti-tier1-dynreg-jwks-submissionreview.md`. Tier 2 / Tier 3 captured here for later.

(Append new entries above this line when revisiting.)

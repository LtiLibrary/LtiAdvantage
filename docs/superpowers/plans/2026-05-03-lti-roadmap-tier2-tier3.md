# LTI Advantage Roadmap — Deferred Work (Tier 2 + Tier 3)

> Companion to `2026-05-03-lti-tier1-dynreg-jwks-submissionreview.md`.
> Captures the LTI 1.3 / LTI Advantage features the LtiAdvantage library does NOT yet implement, beyond Tier 1, so we don't have to re-derive the gap analysis next time.
>
> Authored 2026-05-03 against spec status as of that date. Re-verify spec versions before picking anything up — Candidate Final specs sometimes get breaking edits before Final Release.

---

## Tier 2 — Finalized public specs, narrower audience

These are stable, public, and implementable. Pick one up only when a downstream consumer asks for it; otherwise the value-to-effort ratio is lower than Tier 1.

### Course Groups Service 1.0 — DONE (2026-07-27)
- Spec: https://www.imsglobal.org/spec/lti-gs/v1p0
- Adds: read-only group membership for a context (sibling of NRPS)
- Scope: `https://purl.imsglobal.org/spec/lti-gs/scope/contextgroup.readonly`
- Library shape: mirror NRPS — domain models under `src/LtiAdvantage/CourseGroupsService/`, `GroupsControllerBase` + `SetsControllerBase` under `src/LtiAdvantage.AspNetCore/CourseGroupsService/`
- Estimated size: similar to NRPS (a few days of work)
- As built: the shape above, with `SetsControllerBase` named `GroupSetsControllerBase`. Two extras the plan missed: the service also extends the NRPS membership response with `group_enrollments` (surfaced as `?groups=true`), and it defines two custom-variable substitutions (`$Membership.course.groupIds`, `$CourseGroup.id`).
- On where the `groupsservice` claim is declared: it went on `LtiRequest` first, on the reasoning that a platform may advertise the service on any launch, then moved down onto `LtiResourceLinkRequest` and `LtiSubmissionReviewRequest` — the same two message types that already carry the AGS and NRPS claims. Base-class placement was wrong for a reason worth remembering: the service claims are platform-to-tool advertisements of what the tool may call, so inheriting them put `groupsservice` on `LtiStartAssessmentRequest`, which travels tool to platform and cannot meaningfully advertise anything. The spec is no help here — it says only "for inclusion in LTI 1.3 messages" and names no message types — so the placement follows the existing NRPS convention rather than a spec statement. A test asserts the three service claims stay on the same types.
  - Update (2026-10-28, rebase onto Tier 1 as merged): Tier 1 moved the AGS, NRPS and `resource_link` claims into a shared `LtiServiceLaunchRequest` base that only `LtiResourceLinkRequest` and `LtiSubmissionReviewRequest` derive from, so `groupsservice` now sits there too. The placement decision above is unchanged — tool-to-platform messages such as `LtiStartAssessmentRequest` derive from `LtiRequest` and still don't inherit it.

### Proctoring Services 1.0 — DONE (2026-07-27)
- Spec: https://www.imsglobal.org/spec/proctoring/v1p0
- Adds three message types: `LtiStartProctoring`, `LtiStartAssessment`, `LtiEndAssessment`
- Library shape: three new request types under `src/LtiAdvantage/Lti/`, plus claim value types under `src/LtiAdvantage/Proctoring/`. No new HTTP service endpoints — it's all launch-flow.
- Caveats: only useful if a consumer ships an assessment platform AND a proctoring tool. The session-coordination flow (assessment-control-protocol) is non-trivial — review §5 of the spec carefully before estimating.
- Estimated size: medium (~1 week including reference-JSON tests for each message type)
- As built: **the "no new HTTP service endpoints" note above was wrong.** §5 defines the Assessment Control Service as a real HTTP endpoint with its own `control.all` scope, so `AssessmentControlControllerBase` plus request/response models were needed as well. Other findings worth carrying forward:
  - The `lti-ap` `errormsg`/`errorlog` claims collide by name with the `lti-dl` ones. Constants are `ProctoringErrorMessage`/`ProctoringErrorLog`.
  - The spec's own examples are inconsistent about quoting numbers: `attempt_number` is `1` in the LtiStartProctoring example and `"1"` in the LtiEndAssessment one. The models read both and always write the JSON number.
  - ACS `status` has a real `none` value meaning "no attempt in progress", which must not be conflated with an absent status — hence `AssessmentControlStatus.Unknown` alongside `None`.

### Asset Processor 1.0 — STILL DEFERRED (re-checked 2026-07-27)
- Spec: https://standards.1edtech.org/lti/specifications/proposals/asset-processor/specification (Candidate Final)
- Adds three message types: `LtiAssetProcessorRequest`, `LtiReportReviewRequest`, `LtiEulaRequest`, plus a "report" content type
- Caveat: Candidate Final, not yet Final Release as of 2026-05. Wait for the Final Release vote unless a consumer commits to it.
- Library shape: similar to Proctoring — message types + claim value types
- Estimated size: medium
- **2026-07-27 re-check:** still Candidate Final, and the URL above now sits behind the 1EdTech membership login — the page renders only "Sign in to view this content". So this one is effectively Tier 3, not Tier 2: it cannot be implemented faithfully without spec access, and the claim URIs and JSON property names are exactly the details that must not be guessed. The 1EdTech roadmap also lists its sub-services (Asset, Asset Report, EULA Acceptance, EULA Deployment) as under development. Revisit when it reaches Final Release **and** the specification is readable.

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

> All five were closed on 2026-07-27. Kept here with their outcomes, because the original
> framing understated two of them.

Worth doing in a future minor — these aren't spec features, but they'd make the library easier to consume:

1. **First-class ID-token signature validation helper.** The library parses ID tokens but assumes upstream JWT bearer middleware does signature verification against the platform's JWKS. A built-in `LtiTokenValidator` that fetches+caches the platform JWKS and validates `iss`/`aud`/`exp`/`nonce` would lower the integration bar. Likely lives in `src/LtiAdvantage.IdentityModel/`.
2. **Platform-side OpenID configuration controller.** `/.well-known/openid-configuration` for platforms — not strictly required (most platforms publish it through their CMS), but a `OpenIdConfigurationControllerBase` would close the symmetry with the new `JwksControllerBase`.
3. **Concrete in-memory `IJwksKeyStore`.** Ship one boring reference implementation so getting started doesn't require writing one.
4. **Custom-variable substitution coverage audit.** `CustomPropertySubstitutions.cs` is large but predates several spec additions (e.g. `Context.id.history` for course-copy traceability). Audit against the current spec's variables list.
5. **Async controller base hardening.** The existing `…ControllerBase` classes have `try/catch/finally` blocks that catch `Exception` and emit a `ProblemDetails`. Audit for `OperationCanceledException` (currently treated as 500 — should propagate).

### Outcomes (2026-07-27)

1. **DONE** — `LtiTokenValidator` in `src/LtiAdvantage.IdentityModel/Validation/`. Two things the plan didn't anticipate: JWKS caching has to refresh on an *unknown key id* and not just on a timer, or every launch is rejected for the length of the cache duration after the platform rotates its signing key (that early-refresh path is rate limited so bogus key ids can't be used to hammer the platform); and `azp` must be checked when `aud` holds more than one value, otherwise a token minted for a sibling tool in the same `aud` list is accepted. Nonce single-use needs shared state the validator can't own, so it is delegated to an optional `INonceStore`.
2. **DONE** — `OpenIdConfigurationControllerBase`. It takes its document from an override rather than an injected store (so it does *not* mirror `JwksControllerBase` exactly): nearly every value is an absolute URL to the same host, which is easiest to build from the current request via the library's named routes.
3. **DONE** — `InMemoryJwksKeyStore`. It sanitizes on the way in rather than trusting callers, so handing it a private JWK by mistake still publishes public material only. It also rejects a missing or duplicate `kid`, since verifiers select by key id.
4. **DONE, and it was a bug fix, not just coverage.** The audit found three live defects, all the same root cause — a variable whose name is a prefix of another being substituted first. `$Context.id.history` and `$ResourceLink.id.history` both produced `<the id>.history`, and `$Context.title` was substituted twice (first with the sourced id, then with the title) so the title was wrong *and* `$Context.sourcedId` was unreachable. Two latent hazards went with it: variable names weren't regex-escaped (the dots matched any character) and values were used as regex replacement strings (a value containing `$1` or `$&` was reinterpreted). All 104 call sites now go through one escaping helper. Added the two Course Groups variables.
5. **DONE** — every controller base rethrows `OperationCanceledException` when `HttpContext.RequestAborted` fired. A cancellation that *isn't* the caller leaving still reports as a 500, since that genuinely is a fault.

---

## Decision log

- **2026-10-28** — Rebased `feature/lti-tier2` onto `master` after Tier 1 (PR #237) merged, and re-checked the deferred specs: Asset Processor is still Candidate Final and member-only, and the postMessage trio shows no Candidate Final release. Nothing moves up.
- **2026-07-27** — Implemented Tier 2 on `feature/lti-tier2` (branched off `feature/lti-tier1`): Course Groups Service 1.0, Proctoring Services 1.0 including the Assessment Control Service, and all five open quality gaps. Two deliberate exclusions:
  - **Asset Processor** — dropped from this pass. Still Candidate Final, and its specification is now behind the 1EdTech membership login, so the claim URIs and property names would have to be guessed. See the re-check note in its section above.
  - **All of Tier 3** — unchanged, and for the same reason the original plan gave. Five of the eight are member-only (PNS, Link and Content, Caliper Connector, AfA PNP, Data Privacy Launch) and cannot be read; the three postMessage specs are still v0.1 drafts and the plan's own guidance is not to implement drafts. Nothing here was speculatively built.

  The realistic next candidate is still the postMessage trio, for the cookieless-iframe reason given below — re-check whether they have reached Candidate Final.
- **2026-05-03** — Decided Tier 1 (Submission Review + JWKS + Dynamic Registration) for next minor. Plan: `2026-05-03-lti-tier1-dynreg-jwks-submissionreview.md`. Tier 2 / Tier 3 captured here for later.

(Append new entries above this line when revisiting.)

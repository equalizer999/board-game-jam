# PR Validation Workflow Documentation

## Overview

The PR Validation workflow provides comprehensive automated quality gates for pull requests, ensuring code quality, security, and API standards before merging. It performs linting, formatting checks, security scans, and OpenAPI specification validation.

## Workflow File

`.github/workflows/pr-validation.yml`

## Triggers

The workflow runs automatically on:
- **Pull requests** targeting `main` or `develop` branches

## Jobs

The workflow consists of 5 jobs that run in parallel (except the summary):

### 1. Lint Backend

**Timeout:** 5 minutes

Validates C# code formatting and analyzer rules.

**Steps:**
- Run `dotnet format --verify-no-changes` to check code formatting
- Build with `-p:TreatWarningsAsErrors=true` to enforce analyzer rules
- Post PR comment with fix instructions on failure
- Add `needs-linting` label on failure

**Failure Criteria:**
- Code formatting does not match `.editorconfig` standards
- Any analyzer warnings are present

**How to Fix:**
```bash
# Fix formatting issues
dotnet format

# View analyzer warnings
dotnet build
```

### 2. Lint Frontend

**Timeout:** 5 minutes

Validates TypeScript/React code with ESLint and Prettier formatting.

**Steps:**
- Run `npm run lint` (ESLint with React and TypeScript rules)
- Run `npm run format:check` (Prettier formatting validation)
- Post PR comment with fix instructions on failure
- Add `needs-linting` label on failure

**Failure Criteria:**
- ESLint violations detected
- Code formatting does not match Prettier configuration

**How to Fix:**
```bash
cd client

# Fix ESLint issues (auto-fixable only)
npm run lint

# Fix Prettier formatting
npm run format
```

### 3. Security Scan

**Timeout:** 5 minutes

Scans for known vulnerabilities in both .NET and npm dependencies.

**Steps:**
- Run `dotnet list package --vulnerable --include-transitive` for NuGet packages
- Run `npm audit --audit-level=high --omit=dev` for npm packages
- Post detailed security comment if vulnerabilities found
- Add `security-issue` label on vulnerabilities
- Fail job if high or critical vulnerabilities detected

**Failure Criteria:**
- High or critical severity vulnerabilities in production dependencies

**How to Fix:**
```bash
# Check .NET vulnerabilities
dotnet list package --vulnerable --include-transitive

# Check npm vulnerabilities
cd client
npm audit

# Update vulnerable packages
dotnet add package <PackageName>  # For .NET
npm update <package-name>         # For npm
```

**Notes:**
- Only production dependencies are scanned (dev dependencies excluded)
- Transitive dependencies are included in the scan
- If vulnerabilities cannot be immediately fixed, document them and create a follow-up issue

### 4. OpenAPI Spec Validation

**Timeout:** 5 minutes

Validates the OpenAPI specification for quality and standards compliance.

**Steps:**
- Build and start the backend API
- Wait for API health check
- Download OpenAPI spec from `/swagger/v1/swagger.json`
- Validate with Spectral CLI using OpenAPI standards
- Upload spec as artifact
- Post informational comment if validation issues found

**Failure Criteria:**
- API fails to start
- OpenAPI spec cannot be downloaded
- (Informational only - does not block PR)

**Notes:**
- This job is **informational only** and does not block PR merges
- Spectral validation warnings should be reviewed but are not mandatory to fix
- The generated OpenAPI spec is uploaded as an artifact for manual review

### 5. Validation Summary

**Timeout:** 2 minutes

**Dependencies:** Runs after all validation jobs complete

Aggregates results from all validation jobs and posts a summary comment.

**Steps:**
- Collect results from all validation jobs
- Post comprehensive summary comment on PR with:
  - Overall status (pass/fail)
  - Individual job results with icons (✅/❌)
  - Next steps if failures detected
- Fail if any critical checks failed (linting or security)

**Critical Checks (must pass):**
- Lint Backend
- Lint Frontend
- Security Scan

**Informational Checks (won't block):**
- OpenAPI Validation

## Permissions

The workflow requires the following GitHub permissions:
- `contents: read` - Read repository code
- `pull-requests: write` - Post comments and add labels
- `checks: write` - Report check status

## Labels

The workflow automatically adds labels to PRs:
- `needs-linting` - Code has linting or formatting issues
- `security-issue` - Known security vulnerabilities detected

## Artifacts

The workflow uploads the following artifacts on demand:
- `openapi-spec-{run_number}` - Generated OpenAPI specification (always)
- `backend-logs-openapi-{run_number}` - Backend logs if API fails to start

## PR Comments

The workflow posts contextual comments on PRs:

### Lint Failure Comment
Provides fix commands and links to workflow run for detailed errors.

### Security Issues Comment
Details which ecosystem (NuGet/npm) has vulnerabilities and provides remediation steps.

### OpenAPI Validation Comment
Notifies about specification quality issues with artifact link.

### Validation Summary Comment
Comprehensive overview of all check results with visual indicators.

## Testing the Workflow

### Test with Formatting Issues

Create a test PR with intentional formatting issues:

```bash
# Backend: Add inconsistent whitespace
# Frontend: Remove semicolons or change quotes

git checkout -b test-pr-validation
# Make formatting changes
git commit -am "Test: Add formatting issues"
git push origin test-pr-validation
```

Expected: Workflow fails, posts comment, adds `needs-linting` label

### Test with Security Issues

**⚠️ WARNING:** Only test this on a test branch and **DO NOT MERGE** the vulnerable package.

```bash
# Add an old package with known vulnerabilities (FOR TESTING ONLY)
cd client
npm install axios@0.21.0  # Known to have vulnerabilities
git commit -am "Test: Add vulnerable package"
git push origin test-pr-validation
```

Expected: Workflow fails, posts security comment, adds `security-issue` label

**After testing, remove the vulnerable package:**
```bash
npm install axios@latest
git commit -am "Remove test vulnerability"
git push origin test-pr-validation
```

### Test Success Case

```bash
# Fix all issues
dotnet format
cd client
npm run format
npm audit fix
git commit -am "Fix: Resolve all validation issues"
git push origin test-pr-validation
```

Expected: All checks pass, summary shows ✅ for all jobs

## Troubleshooting

### Job Timeout

If a job times out (exceeds 5 minutes):
- Check for network issues blocking package restoration
- Verify no infinite loops in build process
- Review workflow run logs for hung processes

### False Positive Security Alerts

If a security alert is a false positive:
1. Document why it's a false positive in the PR description
2. Create a follow-up issue to investigate alternatives
3. Consider requesting an exception from security team

### OpenAPI Validation Errors

If OpenAPI validation fails:
1. Download the artifact to review the generated spec
2. Check backend logs artifact for startup errors
3. Verify the `/swagger/v1/swagger.json` endpoint is accessible

### Missing Labels

If labels aren't applied:
1. Verify the workflow has `pull-requests: write` permission
2. Check that labels exist in the repository (they will be created if missing)
3. Review workflow run logs for GitHub API errors

## Integration with Branch Protection

To enforce PR validation as required status checks:

1. Go to repository Settings → Branches
2. Edit branch protection rule for `main` and `develop`
3. Enable "Require status checks to pass before merging"
4. Select required checks:
   - `Lint Backend`
   - `Lint Frontend`
   - `Security Scan`
   - `Validation Summary`
5. Optionally add `OpenAPI Spec Validation` if desired

**Note:** The issue description mentions adding required status checks and branch protection rules, but this must be done manually via GitHub UI by a repository administrator.

## Continuous Improvement

### Optional Enhancements (Not Implemented)

The following features were mentioned in the acceptance criteria but marked as optional:

- **Auto-fix suggestions**: Could use GitHub suggestions API to propose fixes
- **Breaking change detection**: Would require baseline OpenAPI spec comparison
- **Dependabot integration**: GitHub Dependabot alerts are separate from this workflow

These can be added in future iterations based on team needs.

## Related Documentation

- [E2E Testing Workflow](./E2E_WORKFLOW.md)
- [API Testing Guide](./api-testing-guide.md)
- [Dependency Setup](./DEPENDENCY_SETUP.md)

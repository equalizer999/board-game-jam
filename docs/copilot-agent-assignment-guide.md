# GitHub Copilot Coding Agent Assignment Guide

## Overview

GitHub Copilot's coding agent can autonomously implement issues by analyzing the problem statement, generating code, running tests, and creating pull requests. This guide shows how to effectively assign issues to Copilot for the Board Game Café project.

---

## Quick Start

### Assigning an Issue to Copilot

**Method 1: Via GitHub Issue UI**
1. Open any issue (e.g., [Issue #3](https://github.com/equalizer999/board-game-jam-plan/issues/3))
2. In the issue description or comments, mention: `@copilot please implement this issue`
3. Copilot will analyze the issue and create a PR with implementation

**Method 2: Via Issue Assignment**
1. Navigate to the issue
2. Click "Assignees" → Search for "copilot"
3. Assign the issue to Copilot
4. Copilot automatically starts working

**Method 3: Using Copilot Workspace (if enabled)**
1. From VS Code or GitHub UI, select "Open in Copilot Workspace"
2. Select the issue you want Copilot to implement
3. Copilot creates a workspace with implementation plan
4. Review and accept to generate PR

---

## Writing Copilot-Friendly Issues

### ✅ Good Problem Statement Structure

```markdown
## Problem Statement
[Single, clear description of what needs to be built]

## Acceptance Criteria
- [ ] Specific, testable requirement 1
- [ ] Specific, testable requirement 2
- [ ] Verification step

## Technical Details
- Technology/library to use
- File/folder structure hints
- Naming conventions
- Design patterns to follow

## Testing
- How to verify it works
- Expected test coverage
```

### Example: Good vs Bad

#### ❌ BAD (Too Vague)
```markdown
## Problem Statement
Add game management stuff.

## Acceptance Criteria
- Make games work
- Add API
```

**Why it's bad:**
- No clear scope
- "Stuff" is ambiguous
- No technical guidance
- No testing requirements

#### ✅ GOOD (Clear and Specific)
```markdown
## Problem Statement
Create the `Game` domain entity representing board games in the café's library, 
including properties for gameplay metadata, availability tracking, and rental pricing.

## Acceptance Criteria
- [ ] Create `Game` entity in `src/BoardGameCafe.Domain/Entities/Game.cs`
- [ ] Add properties: Id (Guid), Title (string, max 200), Publisher, MinPlayers, 
      MaxPlayers, PlayTimeMinutes, AgeRating, Complexity (decimal 1-5), 
      Category (enum), CopiesOwned, CopiesInUse, DailyRentalFee, Description, ImageUrl
- [ ] Add computed property `IsAvailable`: `CopiesOwned > CopiesInUse`
- [ ] Configure entity in DbContext with fluent API
- [ ] Create EF migration for Game table
- [ ] Add indexes on Title and Category

## Technical Details
- Use Guid for Id with default generation
- Category enum: Strategy, Party, Family, Cooperative, Abstract
- Check constraint: CopiesInUse <= CopiesOwned
- Fluent API in OnModelCreating, not data annotations

## Testing
- Verify migration creates table with correct schema
- Seed 3-5 sample games to test constraints
```

**Why it's good:**
- Single, focused task
- Explicit file paths and names
- Detailed property specifications
- Clear technical constraints
- Concrete verification steps

---

## Best Practices

### 1. **Single Responsibility**
Each issue should do ONE thing well.

✅ Good: "Create Game domain entity"
❌ Bad: "Create all domain entities and APIs"

### 2. **Provide Context**
Link to related issues and existing code:

```markdown
## Context
- Depends on: #6 (Database Context setup)
- Related to: #10 (Games API endpoints)
- See existing pattern in `Customer.cs` for reference
```

### 3. **Explicit File Paths**
Don't make Copilot guess where code goes:

✅ Good: `src/BoardGameCafe.Api/Features/Games/GamesEndpoints.cs`
❌ Bad: "Add games endpoints somewhere"

### 4. **Specify Libraries/Frameworks**
```markdown
## Technical Details
- Use xUnit for testing (NOT NUnit)
- Use FluentAssertions for assertions
- Use NSubstitute for mocking (NOT Moq)
```

### 5. **Include Examples**
Show expected input/output:

```markdown
## Example API Request
```json
POST /api/v1/games
{
  "title": "Catan",
  "publisher": "Catan Studio",
  "minPlayers": 3,
  "maxPlayers": 4
}
```

## Expected Response
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "title": "Catan",
  "isAvailable": true
}
```
```

### 6. **State Testing Requirements**
```markdown
## Testing
- [ ] Write unit tests for all service methods
- [ ] Write integration test for API endpoint
- [ ] Verify error handling (400, 404, 409 status codes)
- [ ] Test edge cases: empty strings, null values, boundary conditions
```

---

## Monitoring Copilot's Work

### After Assignment

1. **Watch for PR Creation**
   - Copilot typically creates PR within 5-15 minutes
   - PR title: "[Copilot] Implement Issue #XX: [Issue Title]"

2. **Review the PR**
   - Check files changed
   - Verify all acceptance criteria met
   - Run tests locally or wait for CI
   - Review code quality and patterns

3. **Provide Feedback (if needed)**
   - Comment on specific lines
   - Request changes via GitHub review
   - Copilot can iterate based on feedback

4. **Merge or Request Changes**
   - If acceptable: merge PR, issue auto-closes
   - If needs work: request changes, Copilot updates PR

---

## Common Issues & Solutions

### Issue: Copilot Creates Too Many Files
**Cause:** Vague scope
**Solution:** Break into smaller issues, each creating 1-3 files max

### Issue: Tests Fail
**Cause:** Copilot missed edge case or misunderstood requirement
**Solution:** Add specific test case to acceptance criteria, reassign

### Issue: Code Doesn't Follow Existing Patterns
**Cause:** No reference to existing code
**Solution:** Link to similar file in issue: "Follow pattern in CustomerEndpoints.cs"

### Issue: Copilot Gets Stuck
**Cause:** Conflicting requirements or missing dependencies
**Solution:** 
1. Check dependency issues are completed first
2. Simplify acceptance criteria
3. Split into 2 smaller issues

### Issue: Implementation Differs from Expected
**Cause:** Ambiguous wording in problem statement
**Solution:** Update issue with more explicit requirements, reassign

---

## Example Workflow

### Step-by-Step: Assigning Issue #5 (Game Domain Entity)

1. **Verify Prerequisites**
   - Issue #3 (Solution structure) complete ✅
   - Issue #6 (Database context) complete ✅

2. **Review Issue Quality**
   - Problem statement clear? ✅
   - Acceptance criteria specific? ✅
   - Technical details provided? ✅
   - Testing requirements included? ✅

3. **Assign to Copilot**
   - Navigate to https://github.com/equalizer999/board-game-jam-plan/issues/5
   - Comment: `@copilot please implement this issue`
   - Or assign via Assignees dropdown

4. **Wait for PR**
   - Copilot analyzes issue (~2-5 min)
   - Copilot generates code (~5-10 min)
   - Copilot creates PR with implementation

5. **Review PR**
   - Check `src/BoardGameCafe.Domain/Entities/Game.cs` created
   - Verify all properties present with correct types
   - Check DbContext configuration added
   - Verify migration created
   - Run tests: `dotnet test`

6. **Merge or Iterate**
   - If all acceptance criteria met: **Approve & Merge**
   - If missing something: **Request Changes** with specific feedback
   - Copilot updates PR based on review comments

7. **Verify Completion**
   - Issue auto-closes when PR merged
   - Check off in roadmap
   - Move to next issue

---

## Copilot Agent Capabilities

### ✅ Copilot Excels At:
- Creating new files from scratch (entities, DTOs, services)
- Writing boilerplate code (CRUD endpoints, test setup)
- Implementing well-defined algorithms (calculations, validators)
- Generating tests from existing code
- Following established patterns in codebase
- Adding migrations, seed data, configuration

### ⚠️ Copilot May Need Guidance:
- Complex business logic with many edge cases
- Performance-critical code (needs explicit constraints)
- Security-sensitive operations (authentication, authorization)
- UI/UX decisions (layout, styling, user flows)
- Architectural decisions (choosing between patterns)

### ❌ Copilot Should NOT Be Used For:
- Intentional bug creation (Phase 6 bug branches)
- Meta-tasks (documentation about using Copilot itself)
- Decisions requiring human judgment (API design trade-offs)
- Tasks requiring external research or domain expertise

---

## Tips for Maximum Effectiveness

### 1. Progressive Complexity
Start with simple issues to establish patterns, then assign more complex ones:

1. Issue #3: Basic project setup (simple) ✅
2. Issue #5: Single entity (simple) ✅
3. Issue #10: CRUD API (medium) ✅
4. Issue #14: Workflow with state machine (complex) ⚠️

### 2. Provide Examples
If you have a working example (e.g., `CustomerEndpoints.cs`), reference it:

```markdown
## Technical Details
- Follow the pattern in `src/BoardGameCafe.Api/Features/Customers/CustomerEndpoints.cs`
- Use same DTO mapping approach
- Use same error handling pattern
```

### 3. Break Down Large Tasks
Instead of "Build complete reservation system":

1. Issue A: Create Reservation entity
2. Issue B: Create reservation endpoints
3. Issue C: Add availability checking logic
4. Issue D: Add conflict detection

### 4. Include "Do Not" Constraints
Explicitly state what NOT to do:

```markdown
## Constraints
- Do NOT use [Authorize] attributes yet (auth not implemented)
- Do NOT add Swagger examples manually (auto-generate from XML docs)
- Do NOT use Entity Framework lazy loading (use eager loading)
```

### 5. Specify Test Structure
```markdown
## Testing
Create tests in `tests/BoardGameCafe.Tests.Unit/Services/OrderCalculationServiceTests.cs`:
- Use `[Theory]` with `[InlineData]` for parameterized tests
- Name tests: `MethodName_Scenario_ExpectedResult`
- Use FluentAssertions: `result.Should().Be(expected)`
- Mock dependencies with NSubstitute
```

---

## Issue Assignment Checklist

Before assigning an issue to Copilot, verify:

- [ ] Problem statement is a single, clear task
- [ ] Acceptance criteria are specific and testable (checkboxes)
- [ ] Technical details include file paths, libraries, patterns
- [ ] Dependencies listed and completed
- [ ] Testing requirements specified
- [ ] Examples provided where helpful
- [ ] "Do not" constraints stated if needed
- [ ] Issue has appropriate labels
- [ ] Estimated complexity is appropriate for Copilot (not too complex)

---

## Advanced: Batch Assignment Strategy

### Parallel Track Assignments

Some issues can be worked on simultaneously if they don't conflict:

**Phase 1: Foundation (Sequential)**
1. Assign #3 → wait for completion
2. Assign #6 → wait for completion
3. Assign #5, #7, #8 **in parallel** (different entities)

**Phase 2: APIs (Parallel Tracks)**
- Track A: #10 (Games API)
- Track B: #4 (Reservations API)
- Track C: #9 (Orders API)

All three can be in progress simultaneously since they work on different feature folders.

---

## Troubleshooting Guide

### Problem: PR Not Created After 30 Minutes
**Steps:**
1. Check if issue is actually assigned to Copilot
2. Verify repository has Copilot enabled
3. Check GitHub status page for outages
4. Try unassigning and reassigning
5. Fall back to manual implementation

### Problem: Copilot's Code Doesn't Compile
**Steps:**
1. Check CI logs for specific errors
2. Comment on PR with error message
3. Request Copilot to fix build errors
4. If persistent, take over and fix manually

### Problem: Tests Written But Don't Cover Edge Cases
**Steps:**
1. Add specific edge cases to acceptance criteria
2. Request changes on PR: "Please add test for X scenario"
3. Copilot updates PR with additional tests

---

## Example Issue Templates

### Template 1: Domain Entity
```markdown
## Problem Statement
Create the `[EntityName]` domain entity representing [description] with [key features].

## Acceptance Criteria
- [ ] Create `[EntityName]` entity in `src/BoardGameCafe.Domain/Entities/[EntityName].cs`
- [ ] Add properties: [list each with type and constraints]
- [ ] Add relationships: [foreign keys, navigation properties]
- [ ] Configure entity in DbContext with fluent API
- [ ] Create EF migration for [EntityName] table
- [ ] Add indexes on [fields for performance]

## Technical Details
- Use Guid for Id
- [Specific enum definitions]
- [Validation constraints]
- [Unique constraints]

## Testing
- Verify migration creates table with correct schema
- Seed sample data to test constraints
```

### Template 2: REST API Endpoint
```markdown
## Problem Statement
Create REST API endpoints for [resource] with [operations] and Swagger documentation.

## Acceptance Criteria
- [ ] Create `Features/[Resource]/[Resource]Endpoints.cs`
- [ ] Implement DTOs: [list each DTO]
- [ ] Add endpoints:
  - `GET /api/v1/[resource]` - [description]
  - `POST /api/v1/[resource]` - [description]
  - [etc.]
- [ ] Add XML documentation for Swagger
- [ ] Configure `[ProducesResponseType]` for status codes: [list codes]
- [ ] Add validation using [library]
- [ ] Register endpoints in Program.cs

## Technical Details
- Use `Results<Ok<T>, NotFound, BadRequest>` return types
- [Specific business rules]
- [Error handling patterns]

## Testing
- Browse /swagger, verify endpoints appear
- Test via Swagger UI: [specific test cases]
- Verify OpenAPI spec includes schemas
```

### Template 3: Unit Tests
```markdown
## Problem Statement
Write comprehensive unit tests for `[ServiceName]` with [coverage goals].

## Acceptance Criteria
- [ ] Create `tests/BoardGameCafe.Tests.Unit/Services/[ServiceName]Tests.cs`
- [ ] Add test methods for:
  - `[Method1]` - [scenarios to cover]
  - `[Method2]` - [scenarios to cover]
- [ ] Use test data builders for test setup
- [ ] Mock dependencies with [library]
- [ ] Achieve >80% code coverage

## Technical Details
- Use `[Theory]` with `[InlineData]` for parameterized tests
- FluentAssertions for assertions
- Test naming: `MethodName_Scenario_ExpectedResult`
- [Specific edge cases to cover]

## Testing
Run `dotnet test tests/BoardGameCafe.Tests.Unit` - all tests pass
```

---

## Quick Reference

### Issue Assignment Commands
```markdown
# In GitHub issue comment:
@copilot please implement this issue
@copilot please review this implementation
@copilot please add tests for [specific scenario]
```

### PR Review Commands
```markdown
# In PR review comment:
@copilot please update this to [specific change]
@copilot the build is failing, please fix [specific error]
@copilot please add documentation for [specific item]
```

---

## Success Metrics

Track Copilot effectiveness:

- **First-Pass Success Rate:** % of PRs merged without changes
- **Iteration Count:** Average review cycles per issue
- **Time to PR:** Average time from assignment to PR creation
- **Test Coverage:** % of Copilot-generated code with tests
- **Build Success:** % of Copilot PRs that pass CI on first run

**Target Metrics for This Project:**
- First-Pass Success: >60%
- Iteration Count: <2 average
- Time to PR: <20 minutes
- Test Coverage: >70%
- Build Success: >80%

---

## Additional Resources

- [GitHub Copilot Documentation](https://docs.github.com/copilot)
- [Board Game Café Roadmap](./ROADMAP.md)
- [Copilot Prompts Guide](./copilot-prompts-guide.md)
- [API Testing Guide](./api-testing-guide.md)
- [Bug Hunting Guide](./bug-hunting-guide.md)

---

## Summary

**Keys to Success with Copilot:**
1. ✅ Write clear, single-purpose issues
2. ✅ Provide explicit acceptance criteria
3. ✅ Specify file paths and libraries
4. ✅ Include testing requirements
5. ✅ Review PRs thoroughly
6. ✅ Provide specific feedback when needed
7. ✅ Celebrate when it works! 🎉

Now you're ready to assign issues to GitHub Copilot and accelerate your Board Game Café implementation!

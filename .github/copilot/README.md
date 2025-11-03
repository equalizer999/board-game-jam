# Copilot Prompt Templates

This directory contains ready-to-use prompt templates for GitHub Copilot to help developers and testing engineers quickly generate high-quality code and tests.

## Available Templates

### [Unit Test Templates](./unit-test-templates.md)
Templates for generating xUnit unit tests for backend services and business logic.

**Use for:**
- Service layer testing
- Domain entity testing
- Validation logic testing
- Business rule testing
- Edge case testing

**Example prompt:**
```
Generate xUnit unit tests for OrderCalculationService in tests/BoardGameCafe.Tests.Unit/Services/OrderCalculationServiceTests.cs:
- Test method: CalculateTotal
- Scenarios: food-only order, alcohol-only order, mixed order, order with Bronze/Silver/Gold discount
- Use FluentAssertions for assertions
- Follow Arrange-Act-Assert pattern
```

---

### [Integration Test Templates](./integration-test-templates.md)
Templates for generating integration tests for API endpoints and database operations.

**Use for:**
- REST API endpoint testing
- CRUD operation testing
- Error scenario testing
- Database relationship testing
- Authentication/authorization testing

**Example prompt:**
```
Generate integration test for POST /api/v1/games:
- Use WebApplicationFactory<Program>
- Seed database with: empty Games table
- Send request: { "title": "Catan", "publisher": "Catan Studio", "minPlayers": 3, "maxPlayers": 4 }
- Expected response: 201 Created with Location header
- Verify database: Games table contains 1 record
```

---

### [E2E Test Templates](./e2e-test-templates.md)
Templates for generating end-to-end tests with Playwright.

**Use for:**
- Page Object Model generation
- User journey testing
- Form interaction testing
- Cross-browser testing
- Mobile responsive testing

**Example prompt:**
```
Create Playwright E2E test for reservation booking journey:
- Journey: customer browses games, selects one, makes a reservation
- Starting point: /games
- User actions: browse → select game → view details → make reservation
- Assertions: games load, detail shows, success message with confirmation
- Use Page Objects: GameCatalogPage, ReservationPage
```

---

## How to Use These Templates

### 1. Choose the Right Template
Select the template that matches your testing needs:
- **Unit tests** → unit-test-templates.md
- **API/integration tests** → integration-test-templates.md
- **E2E/UI tests** → e2e-test-templates.md

### 2. Copy the Template
Find a template that matches your scenario and copy it.

### 3. Customize the Placeholders
Replace `{placeholders}` with your specific values:
- `{ClassName}` → actual class name (e.g., `OrderCalculationService`)
- `{MethodName}` → actual method name (e.g., `CalculateTotal`)
- `{Feature}` → feature name (e.g., `Games`, `Reservations`)
- `{Entity}` → entity name (e.g., `Game`, `Customer`)

### 4. Use with GitHub Copilot
Paste the customized prompt in:
- **GitHub Copilot Chat** (preferred) - for interactive assistance
- **Inline comments** - for quick inline suggestions
- **Command Palette** - Ctrl+Shift+I / Cmd+Shift+I

### 5. Review and Refine
- Review the generated code
- Verify it follows project conventions
- Run tests to ensure they pass
- Iterate if needed with more specific prompts

---

## Example Workflow

**Scenario:** You need to test a new `ReservationValidator` class

**Step 1:** Open `unit-test-templates.md` and find "Validation Logic Test"

**Step 2:** Copy and customize:
```
Generate validation tests for ReservationValidator:
- Valid scenarios: party size within table capacity, future date, duration 1-4 hours
- Invalid scenarios with expected errors:
  - Party size 0: "Party size must be at least 1"
  - Party size > capacity: "Party size exceeds table capacity"
  - Past date: "Reservation must be in the future"
- Use FluentAssertions to check error messages
```

**Step 3:** Open GitHub Copilot Chat and paste the prompt

**Step 4:** Review generated test code, run `dotnet test`, iterate if needed

---

## Tips for Better Results

### Be Specific
❌ "Generate tests for the service"
✅ "Generate xUnit tests for OrderCalculationService with FluentAssertions"

### Include Context
❌ "Test the method"
✅ "Test CalculateTotal method with scenarios: food tax (8%), alcohol tax (10%), member discounts"

### Request Edge Cases
❌ "Test validation"
✅ "Test validation including null inputs, empty strings, boundary values, negative numbers"

### Reference Existing Code
❌ "Create tests"
✅ "Create tests following the pattern in GameRepositoryTests.cs"

---

## Quick Reference: Template Categories

### Unit Test Categories
- Basic service test
- Parameterized test (Theory)
- Validation logic
- Exception testing
- Mocked dependencies
- Computed property
- Business rule
- Edge cases

### Integration Test Categories
- Basic API endpoint
- Success scenario
- Error scenarios (400, 404, 409)
- CRUD operations
- Filtering and pagination
- Database relationships
- Concurrency
- Authentication

### E2E Test Categories
- Page Object Model
- User journey
- Form interaction
- Navigation flow
- Search and filter
- CRUD workflow
- Cross-browser
- Mobile responsive
- Error handling

---

## Additional Resources

### Main Documentation
- [Copilot Quick Prompts](../../docs/copilot-quick-prompts.md) - Quick reference for all prompt types
- [Testing Engineer's Guide](../../docs/testing-engineer-copilot-guide.md) - Comprehensive QA guide
- [Copilot Prompts Guide](../../docs/copilot-prompts-guide.md) - Detailed prompt engineering
- [Copilot Instructions](../copilot-instructions.md) - Workspace-wide Copilot configuration

### GitHub Resources
- [GitHub Copilot Documentation](https://docs.github.com/copilot)
- [Copilot Best Practices](https://docs.github.com/copilot/using-github-copilot/prompt-engineering-for-github-copilot)

---

## Contributing

Found a useful prompt pattern? Add it to the appropriate template file:

1. Follow the existing format
2. Include a clear example
3. Add usage tips
4. Test it first to ensure it works

---

**Remember:** These templates are starting points. Customize them for your specific needs and iterate based on Copilot's output!

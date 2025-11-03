# GitHub Copilot Resources - Quick Navigation

> Your one-stop reference for all GitHub Copilot documentation in this repository.

## 🚀 New to Copilot? Start Here

### 1. **[Testing Engineer's Copilot Guide](./testing-engineer-copilot-guide.md)**
**Best for:** QA engineers and testers new to Copilot
- Quick start tutorial
- Common testing workflows  
- Real-world examples
- Success metrics

**Time to read:** 15 minutes

---

### 2. **[Copilot Quick Prompts](./copilot-quick-prompts.md)**
**Best for:** Quick reference while coding
- Copy-paste ready prompts
- Organized by task type
- Minimal fluff, maximum value

**Time to read:** 5 minutes (bookmark for later use)

---

## 📚 Comprehensive Guides

### [Copilot Prompts Guide](./copilot-prompts-guide.md)
**Deep dive into effective prompt engineering**

Topics:
- Unit test generation (xUnit + FluentAssertions)
- API endpoint creation (Minimal APIs + Swagger)
- Playwright E2E test generation
- Test data builders
- Refactoring prompts
- Best practices and anti-patterns

**Time to read:** 30 minutes

---

### [Copilot Agent Assignment Guide](./copilot-agent-assignment-guide.md)
**How to assign GitHub Issues to Copilot for autonomous implementation**

Topics:
- Writing Copilot-friendly issues
- Assignment methods
- Monitoring Copilot's work
- Reviewing PRs
- Troubleshooting
- Complete example workflow

**Time to read:** 25 minutes

---

## 🎯 Prompt Templates (Copy-Paste Ready)

Located in `.github/copilot/` directory:

### [Unit Test Templates](../.github/copilot/unit-test-templates.md)
Templates for generating xUnit unit tests
- Service layer testing
- Validation logic
- Exception testing
- Parameterized tests (Theory)
- Business rule testing

---

### [Integration Test Templates](../.github/copilot/integration-test-templates.md)
Templates for API and database testing
- REST endpoint testing
- CRUD operations
- Error scenarios (400, 404, 409)
- Filtering and pagination
- Authentication tests

---

### [E2E Test Templates](../.github/copilot/e2e-test-templates.md)
Templates for Playwright end-to-end tests
- Page Object Models
- User journey testing
- Form interactions
- Cross-browser testing
- Mobile responsive tests

---

### [Template Directory README](../.github/copilot/README.md)
Overview of all templates with usage instructions

---

## ⚙️ Configuration

### [Copilot Instructions](../.github/copilot-instructions.md)
**Workspace-wide configuration for GitHub Copilot**

Tells Copilot about:
- Project tech stack (.NET 9, React 18, Playwright)
- Code conventions and patterns
- Business rules specific to this project
- Do's and don'ts
- File structure

**Note:** This file is automatically read by GitHub Copilot when you open this workspace.

---

## 🎓 Learning Path

### For Developers

**Day 1: Basics**
1. Read [Copilot Quick Prompts](./copilot-quick-prompts.md) (5 min)
2. Try 3 prompts from [Unit Test Templates](../.github/copilot/unit-test-templates.md) (15 min)
3. Complete Exercise 1 from [exercises](../exercises/01-unit-test-generation.md) (10 min)

**Day 2: APIs**
1. Read API sections in [Copilot Prompts Guide](./copilot-prompts-guide.md) (10 min)
2. Try 2 prompts from [Integration Test Templates](../.github/copilot/integration-test-templates.md) (15 min)
3. Complete Exercise 2 from [exercises](../exercises/02-api-endpoint-creation.md) (12 min)

**Day 3: E2E Testing**
1. Read E2E sections in [Testing Engineer's Guide](./testing-engineer-copilot-guide.md) (10 min)
2. Try 2 prompts from [E2E Test Templates](../.github/copilot/e2e-test-templates.md) (15 min)
3. Complete Exercise 3 from [exercises](../exercises/03-playwright-test-writing.md) (7 min)

**Day 4: Advanced**
1. Read [Copilot Agent Assignment Guide](./copilot-agent-assignment-guide.md) (25 min)
2. Assign an issue to Copilot and review the PR (30 min)

---

### For QA/Testing Engineers

**Quick Start (30 minutes)**
1. Read [Testing Engineer's Copilot Guide](./testing-engineer-copilot-guide.md) - Quick Start section (5 min)
2. Try 5 test generation prompts from [Copilot Quick Prompts](./copilot-quick-prompts.md) (10 min)
3. Complete Exercise 1: Unit Test Generation (10 min)
4. Experiment with your own tests (5 min)

**Deep Dive (2 hours)**
1. Complete all 5 exercises in [exercises folder](../exercises/README.md) (50 min)
2. Read full [Testing Engineer's Guide](./testing-engineer-copilot-guide.md) (30 min)
3. Create tests for a real feature using templates (30 min)
4. Review and refine (10 min)

---

## 📖 Use Cases & Quick Links

### "I want to generate unit tests"
→ [Unit Test Templates](../.github/copilot/unit-test-templates.md)
→ [Testing Engineer's Guide - Unit Testing](./testing-engineer-copilot-guide.md#unit-testing-workflow)

### "I want to test an API endpoint"
→ [Integration Test Templates](../.github/copilot/integration-test-templates.md)
→ [API Testing Guide](./api-testing-guide.md)

### "I want to write E2E tests"
→ [E2E Test Templates](../.github/copilot/e2e-test-templates.md)
→ [Testing Engineer's Guide - E2E Testing](./testing-engineer-copilot-guide.md#e2e-testing-with-playwright)

### "I want to create an API endpoint"
→ [Quick Prompts - Backend Development](./copilot-quick-prompts.md#backend-development)
→ [Copilot Prompts Guide - API Endpoint Generation](./copilot-prompts-guide.md#api-endpoint-generation)

### "I want to assign work to Copilot agent"
→ [Copilot Agent Assignment Guide](./copilot-agent-assignment-guide.md)

### "I need test data"
→ [Quick Prompts - Test Data Management](./copilot-quick-prompts.md#test-data-management)
→ [Copilot Prompts Guide - Test Data Builder](./copilot-prompts-guide.md#test-data-builder-prompts)

### "I'm debugging a test failure"
→ [Quick Prompts - Debugging](./copilot-quick-prompts.md#debugging--troubleshooting)
→ [Testing Engineer's Guide - Troubleshooting](./testing-engineer-copilot-guide.md#troubleshooting-with-copilot)

### "I want to reproduce a bug with a test"
→ [Testing Engineer's Guide - Bug Hunting](./testing-engineer-copilot-guide.md#bug-hunting--regression-testing)
→ [Bug Hunting Guide](./bug-hunting-guide.md)

---

## 🎯 Tips for Success

### Getting the Best from Copilot

1. **Be Specific:** Include framework names, libraries, and patterns
2. **Provide Context:** Reference existing files and code patterns  
3. **Request Edge Cases:** Ask for null checks, boundary values, error scenarios
4. **Iterate:** Refine prompts based on initial output
5. **Review Carefully:** Copilot generates starting points, not final code

### Common Patterns

**Good Prompt:**
```
Generate xUnit unit tests for OrderCalculationService with FluentAssertions:
- Test CalculateTotal method
- Scenarios: food tax (8%), alcohol tax (10%), member discounts (Bronze 5%, Silver 10%, Gold 15%)
- Edge cases: null order, empty items, zero amounts
- Use OrderBuilder for test data
```

**Bad Prompt:**
```
Write tests for the service
```

---

## 🔗 External Resources

- [GitHub Copilot Documentation](https://docs.github.com/copilot)
- [Prompt Engineering for Copilot](https://docs.github.com/copilot/using-github-copilot/prompt-engineering-for-github-copilot)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Playwright Documentation](https://playwright.dev/)

---

## 🤝 Contributing

Found a helpful prompt pattern? Add it!

1. Choose the appropriate template file
2. Follow the existing format
3. Include a clear example
4. Test it first

---

## 📊 Document Map

```
docs/
├── copilot-quick-prompts.md          # Quick reference (START HERE for quick wins)
├── testing-engineer-copilot-guide.md # QA engineer's comprehensive guide
├── copilot-prompts-guide.md          # Detailed prompt engineering guide
├── copilot-agent-assignment-guide.md # Autonomous issue implementation
└── COPILOT_RESOURCES.md              # This file - navigation hub

.github/
├── copilot-instructions.md           # Workspace configuration
└── copilot/
    ├── README.md                     # Template overview
    ├── unit-test-templates.md        # xUnit test prompts
    ├── integration-test-templates.md # API test prompts
    └── e2e-test-templates.md         # Playwright test prompts

exercises/
├── 01-unit-test-generation.md        # Hands-on unit testing
├── 02-api-endpoint-creation.md       # Hands-on API development
├── 03-playwright-test-writing.md     # Hands-on E2E testing
├── 04-test-data-builder.md           # Hands-on test data patterns
└── 05-bug-fix-with-test.md           # Hands-on bug hunting
```

---

**Questions?** Open a GitHub Discussion or check existing issues for examples of Copilot-generated code.

Happy prompting! 🚀

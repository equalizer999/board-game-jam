# Board Game Café Implementation Roadmap

## Overview

This roadmap breaks down the Board Game Café demo environment into phased milestones with dependencies and issue references. Each phase builds on the previous, culminating in a complete testing automation showcase.

## Phase 1: Foundation 🏗️

**Goal:** Establish project structure, database, and core domain entities.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#3](https://github.com/equalizer999/board-game-jam-plan/issues/3) | Setup .NET 9 Solution Structure | None | ✅ Yes |
| [#6](https://github.com/equalizer999/board-game-jam-plan/issues/6) | Implement SQLite Database Context | #3 | ✅ Yes |
| [#5](https://github.com/equalizer999/board-game-jam-plan/issues/5) | Create Game Domain Entity | #6 | ✅ Yes |
| [#7](https://github.com/equalizer999/board-game-jam-plan/issues/7) | Create Table, Reservation, Customer Entities | #6 | ✅ Yes |
| [#8](https://github.com/equalizer999/board-game-jam-plan/issues/8) | Implement Order and MenuItem Entities | #6 | ✅ Yes |

**Completion Criteria:**
- ✅ Solution builds without errors
- ✅ Database migrations applied successfully
- ✅ All core entities defined with relationships
- ✅ Can seed basic test data manually

---

## Phase 2: REST API Development 🚀

**Goal:** Build comprehensive REST APIs with Swagger documentation for all resources.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#10](https://github.com/equalizer999/board-game-jam-plan/issues/10) | Build Games CRUD REST API | #5 | ✅ Yes |
| [#4](https://github.com/equalizer999/board-game-jam-plan/issues/4) | Build Reservations REST API | #7 | ✅ Yes |
| [#9](https://github.com/equalizer999/board-game-jam-plan/issues/9) | Build Orders REST API | #8 | ✅ Yes |
| [#14](https://github.com/equalizer999/board-game-jam-plan/issues/14) | Create Game Checkout/Return Workflow | #10 | ✅ Yes |
| [#12](https://github.com/equalizer999/board-game-jam-plan/issues/12) | Build Event Management System | #7 | ✅ Yes |
| [#27](https://github.com/equalizer999/board-game-jam-plan/issues/27) | Build Menu REST API | #8 | ✅ Yes |
| [#28](https://github.com/equalizer999/board-game-jam-plan/issues/28) | Implement Loyalty Points System | #9, #27 | ✅ Yes |

**Completion Criteria:**
- ✅ All endpoints accessible via Swagger UI at `/swagger`
- ✅ Full OpenAPI spec generated with schemas
- ✅ Request validation working (400 for invalid data)
- ✅ Proper error responses with ProblemDetails
- ✅ Business logic enforced (conflicts, capacity, availability)

---

## Phase 3: Backend Testing Infrastructure 🧪

**Goal:** Comprehensive unit and integration test coverage for backend services and APIs.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#17](https://github.com/equalizer999/board-game-jam-plan/issues/17) | Setup Unit Testing Infrastructure | Phase 2 | ✅ Yes |
| [#16](https://github.com/equalizer999/board-game-jam-plan/issues/16) | Create Integration Tests with WebApplicationFactory | Phase 2 | ⚠️ Partial* |
| [#11](https://github.com/equalizer999/board-game-jam-plan/issues/11) | Create Comprehensive Seed Data | Phase 2 | ✅ Yes |

*Copilot can generate tests, but may need guidance on WebApplicationFactory setup and database mocking strategies.

**Completion Criteria:**
- ✅ Unit test coverage >70% for service layer
- ✅ Integration tests for all API endpoints
- ✅ Test data builders implemented (Game, Customer, Order, Reservation)
- ✅ Mock services for external dependencies (email, payment)
- ✅ Seed data includes 60+ games, 40+ menu items, 15+ tables

---

## Phase 4: Frontend Development 🎨

**Goal:** Build React TypeScript frontend with game-themed UI and complete user workflows.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#13](https://github.com/equalizer999/board-game-jam-plan/issues/13) | Initialize React + TypeScript Frontend | None | ✅ Yes |
| [#15](https://github.com/equalizer999/board-game-jam-plan/issues/15) | Build Game Catalog UI | #13, #10 | ✅ Yes |
| [#18](https://github.com/equalizer999/board-game-jam-plan/issues/18) | Build Reservation Booking Flow | #13, #4 | ⚠️ Partial* |

*Copilot can build components, but complex UI like visual table selector may need refinement.

**Completion Criteria:**
- ✅ Frontend runs on `localhost:5173`
- ✅ API proxy configured, calls backend successfully
- ✅ Game catalog with filtering and search working
- ✅ Reservation flow end-to-end functional
- ✅ Game-themed styling (meeple icons, board game aesthetic)
- ✅ Responsive design (mobile, tablet, desktop)

---

## Phase 5: E2E Testing with Playwright 🎭

**Goal:** Complete E2E test coverage for critical user journeys across browsers.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#19](https://github.com/equalizer999/board-game-jam-plan/issues/19) | Setup Playwright E2E Infrastructure | Phase 4 | ✅ Yes |
| [#20](https://github.com/equalizer999/board-game-jam-plan/issues/20) | Create Complete E2E Test Suite | #19 | ⚠️ Partial* |

*Copilot can generate tests, but Page Object Models and complex interactions may need refinement.

**Completion Criteria:**
- ✅ Playwright configured for Chrome, Firefox, Safari
- ✅ Page Object Models for major pages
- ✅ E2E tests for: game browsing, reservation booking, order placement, event registration
- ✅ Screenshots/videos on failure
- ✅ All tests passing on all 3 browsers

---

## Phase 6: Bug Demonstrations 🐛

**Goal:** Create intentional bug branches with linked issues for workshop practice.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#21](https://github.com/equalizer999/board-game-jam-plan/issues/21) | Create Bug Demonstration Branches | Phase 2, Phase 4 | ❌ No* |

*This requires manual branch creation and intentional bug introduction. Copilot should NOT be assigned to fix these bugs initially.

**Bug Branches:**
1. `bug/midnight-reservation` - Timezone conversion issue
2. `bug/double-discount` - Negative total from stacked discounts
3. `bug/vanishing-game` - Cache invalidation missing
4. `bug/table-time-traveler` - Past date validation only client-side
5. `bug/order-item-duplication` - Race condition on rapid clicks
6. `bug/case-sensitive-email` - Duplicate accounts with email casing
7. `bug/event-registration-race` - Concurrency on last event spot
8. `bug/loyalty-points-reversal` - Cancelled orders don't deduct points

**Completion Criteria:**
- ✅ 8 bug branches created from stable main
- ✅ GitHub Issues created for each bug with reproduction steps
- ✅ `/docs/bug-hunting-guide.md` documents all bugs with hints
- ✅ Each bug is realistic and educational

---

## Phase 7: DevOps & CI/CD 🔄

**Goal:** Automate testing, linting, and deployment with GitHub Actions.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#23](https://github.com/equalizer999/board-game-jam-plan/issues/23) | Configure VS Code Devcontainer | None | ✅ Yes |
| [#22](https://github.com/equalizer999/board-game-jam-plan/issues/22) | Create GitHub Actions CI Workflow | Phase 3 | ✅ Yes |
| [#24](https://github.com/equalizer999/board-game-jam-plan/issues/24) | Create E2E Workflow for Playwright | Phase 5 | ✅ Yes |
| [#25](https://github.com/equalizer999/board-game-jam-plan/issues/25) | Create PR Validation Workflow | Phase 2 | ✅ Yes |

**Completion Criteria:**
- ✅ Devcontainer launches with all tools pre-installed
- ✅ CI workflow runs on push to main/develop
- ✅ E2E workflow runs with browser matrix
- ✅ PR validation catches linting/formatting issues
- ✅ Code coverage reports uploaded
- ✅ Build status badges in README

---

## Phase 8: Workshop Materials 📚

**Goal:** Documentation and exercises for workshop participants.

| Issue | Title | Dependencies | Assignable to Copilot |
|-------|-------|--------------|------------------------|
| [#26](https://github.com/equalizer999/board-game-jam-plan/issues/26) | Create Workshop Documentation | All phases | ⚠️ Partial* |
| [#29](https://github.com/equalizer999/board-game-jam-plan/issues/29) | Create Exercise Templates | All phases | ⚠️ Partial* |
| [#30](https://github.com/equalizer999/board-game-jam-plan/issues/30) | Document Copilot Agent Assignment | None | ❌ No** |

*Copilot can help draft documentation, but human review essential for clarity and accuracy.
**This is meta-documentation about using Copilot itself.

**Completion Criteria:**
- ✅ `/docs/copilot-prompts-guide.md` with examples
- ✅ `/docs/bug-hunting-guide.md` complete
- ✅ `/docs/api-testing-guide.md` with Swagger usage
- ✅ `/exercises/` folder with 5+ exercise templates
- ✅ README updated with workshop guide section
- ✅ Architecture diagram included

---

## Dependency Graph

```
Phase 1 (Foundation)
  ↓
Phase 2 (REST APIs)
  ↓
Phase 3 (Backend Testing) ← Phase 6 (Bug Demos)
  ↓
Phase 4 (Frontend)
  ↓
Phase 5 (E2E Testing)
  ↓
Phase 7 (CI/CD)
  ↓
Phase 8 (Workshop Materials)
```

---

## Milestones

### Milestone 1: Minimum Viable Demo (Phases 1-2)
- **Goal:** Working backend API with Swagger
- **Duration:** ~2-3 days
- **Issues:** #3, #6, #5, #7, #8, #10, #4, #9

### Milestone 2: Full Backend with Tests (Phase 3)
- **Goal:** Comprehensive test coverage
- **Duration:** ~2 days
- **Issues:** #17, #16, #11

### Milestone 3: Complete Application (Phases 4-5)
- **Goal:** Full-stack application with E2E tests
- **Duration:** ~3-4 days
- **Issues:** #13, #15, #18, #19, #20

### Milestone 4: Workshop Ready (Phases 6-8)
- **Goal:** All materials prepared for workshop delivery
- **Duration:** ~2 days
- **Issues:** #21, #22, #23, #24, #25, #26, #29, #30

---

## Implementation Tips

### Working with GitHub Copilot Coding Agent

1. **Start with Foundation:** Complete Phase 1 issues sequentially (#3 → #6 → #5,#7,#8)
2. **Assign Small, Focused Issues:** Copilot works best on single-purpose tasks
3. **Review Before Merge:** Always review Copilot's PRs, especially for business logic
4. **Provide Context:** Link related issues and existing code patterns in issue descriptions
5. **Iterate:** If Copilot's first attempt isn't perfect, provide feedback and reassign

### Progressive Checkpoints

Consider creating git tags at key milestones:
- `v0.1-foundation` after Phase 1
- `v0.2-apis` after Phase 2
- `v0.3-backend-tests` after Phase 3
- `v0.4-frontend` after Phase 4
- `v1.0-workshop-ready` after Phase 8

### Branching Strategy

- `main` - Stable, complete implementation
- `develop` - Active development (Copilot creates PRs here)
- `checkpoint-XX` - Incremental workshop reference branches
- `bug/*` - Bug demonstration branches (created manually)
- `workshop-exercises` - Partially implemented with TODOs (for live coding)

---

## Success Metrics

**Technical:**
- ✅ All 30 issues completed
- ✅ 100% of endpoints documented in Swagger
- ✅ >70% code coverage on backend
- ✅ All E2E tests passing on 3 browsers
- ✅ CI/CD green on main and develop

**Workshop:**
- ✅ One-command setup (devcontainer)
- ✅ Clear documentation for all exercises
- ✅ 8 bug scenarios ready to demonstrate
- ✅ Copilot assignment guide complete
- ✅ Architecture easy to understand

---

## Next Steps

1. Start with Issue #3 (Setup .NET 9 Solution)
2. Assign to GitHub Copilot or implement manually
3. Review and merge PR
4. Move to next issue in Phase 1
5. Track progress in this roadmap

For detailed guidance on assigning issues to Copilot, see [Copilot Agent Assignment Guide](./copilot-agent-assignment-guide.md).

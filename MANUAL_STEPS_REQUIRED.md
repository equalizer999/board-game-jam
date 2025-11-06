# Manual Steps Required

This file documents the manual steps required to complete the bug branch setup, as some operations cannot be automated due to authentication constraints.

## Status

✅ **Completed:**
- All 8 bug branches created locally
- Comprehensive documentation created (BUG_BRANCHES_SUMMARY.md, GITHUB_ISSUES_TEMPLATE.md)
- Helper scripts created (push-bug-branches.sh)
- All documentation committed and pushed to PR branch

❌ **Requires Manual Action:**
- Pushing bug branches to remote repository
- Creating GitHub Issues
- Updating bug-hunting-guide.md with issue numbers

## Step 1: Push Bug Branches to Remote

The bug branches are created locally but need to be pushed to GitHub. Run:

```bash
./scripts/push-bug-branches.sh
```

Or manually push each branch:

```bash
git push origin bug/midnight-reservation
git push origin bug/double-discount
git push origin bug/vanishing-game
git push origin bug/table-time-traveler
git push origin bug/order-item-duplication
git push origin bug/case-sensitive-email
git push origin bug/event-registration-race
git push origin bug/loyalty-points-reversal
```

**Verification:**
```bash
git branch -r | grep bug/
```

You should see all 8 branches listed.

## Step 2: Create GitHub Issues

Use the templates in `GITHUB_ISSUES_TEMPLATE.md` to create 8 GitHub Issues.

**For each bug:**
1. Go to https://github.com/equalizer999/board-game-jam/issues/new
2. Copy the issue content from GITHUB_ISSUES_TEMPLATE.md
3. Add the appropriate labels (bug-demo, severity level, category)
4. Note the issue number assigned

**Or use GitHub CLI:**
```bash
# Example for first bug
gh issue create \
  --title "Midnight Reservation Shows Wrong Date" \
  --body-file issue-content.md \
  --label "bug-demo,datetime,high-severity"
```

**Issue Numbers to Track:**
- [ ] Issue #__ - Midnight Reservation bug
- [ ] Issue #__ - Double Discount bug
- [ ] Issue #__ - Vanishing Game bug
- [ ] Issue #__ - Table Time Traveler bug
- [ ] Issue #__ - Order Item Duplication bug
- [ ] Issue #__ - Case Sensitive Email bug
- [ ] Issue #__ - Event Registration Race bug
- [ ] Issue #__ - Loyalty Points Reversal bug

## Step 3: Update bug-hunting-guide.md

Once issues are created, update `docs/bug-hunting-guide.md`:

1. Replace all `[#XX]` placeholders with actual issue numbers
2. Update links like `https://github.com/equalizer999/board-game-jam/issues/XX`

**Example:**
```markdown
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)
```
becomes:
```markdown
**Issue:** [#42](https://github.com/equalizer999/board-game-jam/issues/42)
```

## Step 4: Verify Bug Reproducibility

Test each bug branch to ensure bugs reproduce as documented:

```bash
# For each bug
git checkout bug/midnight-reservation
# Follow reproduction steps in BUG_BRANCHES_SUMMARY.md or the GitHub Issue
# Verify the bug manifests as described
```

## Step 5: Workshop Preparation

Before the workshop:
- [ ] Verify all branches are pushed
- [ ] Verify all issues are created and properly labeled
- [ ] Test at least 2-3 bugs to ensure reproduction steps work
- [ ] Share fork instructions with participants
- [ ] Prepare demo of Bug #1 for instructor walkthrough

## Reference Files

- **BUG_BRANCHES_SUMMARY.md** - Technical details, commit hashes, affected files
- **GITHUB_ISSUES_TEMPLATE.md** - Complete issue templates
- **docs/bug-hunting-guide.md** - Participant guide with hints and test examples
- **scripts/push-bug-branches.sh** - Helper script for pushing branches

## Troubleshooting

**If a branch push fails:**
```bash
# Check branch exists locally
git branch | grep bug/

# Force push if needed (careful!)
git push -f origin bug/branch-name
```

**If issue creation fails:**
- Verify you have write permissions to the repository
- Check label names match exactly (bug-demo, high-severity, etc.)
- Ensure markdown formatting is correct

**If bug doesn't reproduce:**
- Verify you're on the correct bug branch: `git branch --show-current`
- Check the commit hash matches: `git log -1 --oneline`
- Review BUG_BRANCHES_SUMMARY.md for the exact affected file and line

## Contact

If you encounter issues completing these manual steps, refer to:
- BUG_BRANCHES_SUMMARY.md for technical details
- GITHUB_ISSUES_TEMPLATE.md for issue content
- Repository maintainer for access/permissions issues

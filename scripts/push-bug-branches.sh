#!/usr/bin/env bash
# Script to push all bug demonstration branches to remote repository
# Note: This requires push permissions to the repository

set -e

echo "Pushing bug demonstration branches to origin..."

branches=(
    "bug/midnight-reservation"
    "bug/double-discount"
    "bug/vanishing-game"
    "bug/table-time-traveler"
    "bug/order-item-duplication"
    "bug/case-sensitive-email"
    "bug/event-registration-race"
    "bug/loyalty-points-reversal"
)

for branch in "${branches[@]}"
do
    echo "Pushing $branch..."
    git push origin "$branch" || echo "Failed to push $branch"
done

echo "All bug branches pushed successfully!"
echo ""
echo "Next steps:"
echo "1. Create GitHub Issues for each bug"
echo "2. Update docs/bug-hunting-guide.md with issue numbers"
echo "3. Test each bug branch to verify reproducibility"

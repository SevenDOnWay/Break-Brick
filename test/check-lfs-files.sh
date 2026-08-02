#!/usr/bin/env bash
set -euo pipefail

max_size_bytes=10485760 # 10 MiB
failed=0
before_revision="${1:-}"
after_revision="${2:-HEAD}"
zero_revision="0000000000000000000000000000000000000000"

if [[ "${GITHUB_EVENT_NAME:-}" == "pull_request" ]]; then
  range="origin/${GITHUB_BASE_REF}...HEAD"
  changed_files_command=(git diff --name-only -z --diff-filter=AMRC "$range")
elif [[ -n "$before_revision" && "$before_revision" != "$zero_revision" ]]; then
  range="${before_revision}..${after_revision}"
  changed_files_command=(git diff --name-only -z --diff-filter=AMRC "$range")
else
  changed_files_command=(git diff-tree --root --no-commit-id --name-only -r -z --diff-filter=AMRC "$after_revision")
fi

while IFS= read -r -d '' path; do
  if ! git cat-file -e "HEAD:$path" 2>/dev/null; then
    continue
  fi

  size=$(git cat-file -s "HEAD:$path")
  attribute=$(git check-attr filter -- "$path")

  if [[ "$attribute" == *": lfs" ]]; then
    if ! git cat-file blob "HEAD:$path" | git lfs pointer --check --stdin >/dev/null; then
      echo "::error file=$path::This file is configured for Git LFS but is not a valid LFS pointer."
      failed=1
    fi
  elif (( size > max_size_bytes )); then
    echo "::error file=$path::File is larger than 10 MiB and is not stored in Git LFS."
    failed=1
  fi
done < <( "${changed_files_command[@]}" )

exit "$failed"

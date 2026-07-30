#!/usr/bin/env bash
set -euo pipefail

max_size_bytes=10485760 # 10 MiB
failed=0

if [[ "${GITHUB_EVENT_NAME:-}" == "pull_request" ]]; then
  range="origin/${GITHUB_BASE_REF}...HEAD"
else
  range="HEAD^..HEAD"
fi

while IFS= read -r -d '' path; do
  if ! git cat-file -e "HEAD:$path" 2>/dev/null; then
    continue
  fi

  size=$(git cat-file -s "HEAD:$path")
  attribute=$(git check-attr filter -- "$path")

  if [[ "$attribute" == *": lfs" ]]; then
    first_line=$(git cat-file -p "HEAD:$path" | sed -n '1p')

    if [[ "$first_line" != "version https://git-lfs.github.com/spec/v1" ]]; then
      echo "::error file=$path::This file is configured for Git LFS but is not stored as an LFS pointer."
      failed=1
    fi
  elif (( size > max_size_bytes )); then
    echo "::error file=$path::File is larger than 10 MiB and is not stored in Git LFS."
    failed=1
  fi
done < <(git diff --name-only -z --diff-filter=AM "$range")

exit "$failed"

#!/usr/bin/env bash
set -euo pipefail

failed=0

if [[ "${GITHUB_EVENT_NAME:-}" == "pull_request" ]]; then
  range="origin/${GITHUB_BASE_REF}...HEAD"
else
  range="HEAD^..HEAD"
fi

while IFS= read -r -d '' path; do
  if [[ "$path" == *.meta ]]; then
    asset_path="${path%.meta}"

    if [[ ! -e "$asset_path" && -e "$path" ]]; then
      echo "::error file=$path::Orphan .meta file. No matching Unity asset or folder exists."
      failed=1
    fi
  elif [[ -e "$path" && ! -e "${path}.meta" ]]; then
    echo "::error file=$path::Missing Unity .meta file."
    failed=1
  elif [[ ! -e "$path" && -e "${path}.meta" ]]; then
    echo "::error file=${path}.meta::Asset was deleted but its .meta file remains."
    failed=1
  fi
done < <(git diff --no-renames --name-only -z --diff-filter=ACMRD "$range" -- Assets)

exit "$failed"

#!/usr/bin/env bash
set -euo pipefail

failed=0

while IFS= read -r -d '' path; do
  if [[ "$path" == *.meta ]]; then
    asset_path="${path%.meta}"

    if [[ ! -e "$asset_path" ]]; then
      echo "::error file=$path::Orphan .meta file. No matching Unity asset or folder exists."
      failed=1
    fi
  else
    meta_path="${path}.meta"

    if [[ ! -e "$meta_path" ]]; then
      echo "::error file=$path::Missing Unity .meta file."
      failed=1
    fi
  fi
done < <(git ls-files -z -- Assets)

exit "$failed"

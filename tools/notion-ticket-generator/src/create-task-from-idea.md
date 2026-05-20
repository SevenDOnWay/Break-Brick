# Create Task From Idea

Use this doc instead of loading `create-task-from-idea.ts` into context.

## Purpose

`create-task-from-idea.ts` creates a Notion implementation task from a plain-language idea. It uses the same `.env` as the existing ticket generator:

- `NOTION_TOKEN`
- `NOTION_DATA_SOURCE_ID` or `NOTION_DATABASE_ID`

The script turns a human idea into a Notion row with enough context for a later coding agent to work from the Notion task instead of a long prompt.

## Current Notion Schema

The script can create these properties:

- `Task name`: title
- `Status`: defaults to `Codex Ready`
- `Priority`: select, use `Low`, `Medium`, or `High`
- `Task type`: multi-select. Exact Notion option names work, and common aliases like `code`, `bug`, `feature`, `polish`, `asset`, `experiment`, and `tech debt` are normalized.
- `Risk Level`: multi-select. Use `Low`, `Med`, `Medium`, or `High`; `Medium` is normalized to `Med`.
- `Assignee`: person, requires Notion person ID with `--assignee-id`
- `Due date`: date, accepts ISO or `DD/MM/YYYY`
- `Description`: rich text
- `Acceptance Criteria`: rich text
- `Technical Notes`: rich text
- `Test Instructions`: rich text
- `Ticket Path`: not set here; `index.ts` fills this after ticket generation
- `GDD Link`: URL
- `Files Allowed`: rich text
- `Result Summary`: optional rich text
- `Verification Result`: optional rich text
- `Agent Notes`: rich text

`Task ID` is read-only auto-increment in Notion. This script does not write it. If `--task-id` is provided, it is kept only as a client reference inside `Agent Notes`.

The page body also gets:

- `Implementation Idea`: the original idea text
- `Agent Brief`: goal, description, agent rules, acceptance criteria, technical guidance, allowed files, verification, and agent notes

## Basic Usage

Run from:

```powershell
cd "C:\Users\ngoda\Git\Break_Brick\Break Brick\tools\notion-ticket-generator"
```

Preview first:

```powershell
npm run create:task -- --dry-run --idea "Improve paddle input feel."
```

Create a Notion task:

```powershell
npm run create:task -- --idea "Add a combo meter that increases when the player breaks bricks without missing the ball, resets on miss, and displays the current combo near the score."
```

Use a markdown/text file as the idea:

```powershell
npm run create:task -- --title "Combo meter" --idea-file ./idea.md
```

## Options

- `--idea`: idea text. Positional text also works.
- `--idea-file`: file path to read idea text from.
- `--title`: override generated task title.
- `--description`: general description/grouping text.
- `--acceptance`: acceptance criteria. Separate items with semicolons or new lines.
- `--technical`: technical notes or constraints.
- `--files`: files or folders the coding agent may modify.
- `--tests`: verification steps the coding agent should run.
- `--gdd`: optional design/GDD URL.
- `--status`: Notion status. Defaults to `Codex Ready`.
- `--priority`: `Low`, `Medium`, or `High`.
- `--task-type`: one or more values, separated by commas or semicolons. Exact values or aliases are accepted.
- `--risk-level`: one or more values: `Low`, `Med`, `Medium`, `High`.
- `--assignee-id`: one or more Notion person IDs, separated by commas or semicolons.
- `--due-date`: ISO date/time or `DD/MM/YYYY` with optional `HH:mm`.
- `--result-summary`: optional initial result summary.
- `--verification-result`: optional initial QA/verification notes.
- `--agent-notes`: extra notes for the coding agent.
- `--task-id`: client reference only; Notion auto-generates the real Task ID.
- `--dry-run`: print generated draft JSON and do not call Notion.
- `--help`: print CLI help.

## Recommended Agent Workflow

1. Read this doc, not the TypeScript source.
2. Ask the user for the implementation idea if it is unclear.
3. Run `--dry-run` first to inspect the generated draft.
4. If the draft is good, run the same command without `--dry-run`.
5. Pass the created Notion page id or Notion Task ID to future agents.

Example with richer context:

```powershell
npm run create:task -- `
  --title "Combo meter" `
  --idea "Add a combo meter that increases when the player breaks bricks without missing the ball, resets on miss, and displays the current combo near the score." `
  --description "Gameplay scoring improvement" `
  --priority "Medium" `
  --task-type "💬 Feature request,✍️code" `
  --risk-level "Med" `
  --acceptance "Combo increments after consecutive brick breaks; Combo resets when the player misses the ball; Current combo is visible near the score; Existing score behavior still works" `
  --technical "Inspect current score and ball-miss handling before editing. Prefer existing UI patterns." `
  --files "Assets/Scripts/GamePlayScripts; relevant UI scripts only" `
  --tests "Run the Unity scene manually and verify combo increment/reset behavior"
```

## Relationship To Existing Script

`create-task-from-idea.ts` pushes ideas into Notion.

`index.ts` pulls Notion tasks with `Status = Codex Ready`, writes local markdown tickets under `.codex-autorunner/tickets`, fills `Ticket Path`, and moves them to `Codex Working`.

Typical flow:

```powershell
npm run create:task -- --idea "Describe feature idea here"
npm run generate:tickets
```

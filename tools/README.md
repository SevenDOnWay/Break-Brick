# Tools

## Purpose

Local automation and helper scripts for repository workflows.

## Component Registry

- `notion-ticket-generator/`: Creates local Codex tickets from Notion and can create Notion tasks from ideas.
- `codex-runner/`: Runs one local Markdown ticket through Codex CLI on a branch from `agile`.

## Communication

The Notion ticket generator writes tickets into `.codex-autorunner/tickets/`. The Codex runner consumes those tickets and prepares reviewable branches.

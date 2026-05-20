# Codex Runner

## Purpose

Runs one Markdown ticket from `.codex-autorunner/tickets/` through Codex CLI on a fresh branch from `agile`.

## Component Registry

- `run-next-ticket.js`: Safe MVP runner. It checks prerequisites, refuses dirty worktrees, moves one ticket through the queue folders, creates a `codex/<ticket-name>` branch, invokes Codex CLI, and prints manual next steps.

## Communication

This tool consumes tickets produced by `tools/notion-ticket-generator`. If a ticket contains a Notion page id and Notion credentials are available in `tools/notion-ticket-generator/.env`, the runner attempts to update the Notion task to `Codex Review` after Codex finishes.

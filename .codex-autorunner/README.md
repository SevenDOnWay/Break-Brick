# Codex Autorunner Queue

## Purpose

This folder stores local Markdown tickets as they move through the Codex autorunner workflow.

## Component Registry

- `tickets/`: Pending Markdown tickets generated from Notion or written by hand.
- `working/`: The single ticket currently being processed by the local runner.
- `done/`: Tickets that have already been handed to Codex CLI.

## Communication

`tools/codex-runner/run-next-ticket.js` reads one ticket from `tickets/`, moves it to `working/`, creates a branch from `agile`, sends the ticket content to Codex CLI, then moves the ticket to `done/` after Codex finishes successfully.

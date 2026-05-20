# Working

## Purpose

Temporary holding area for the ticket currently being processed by Codex CLI.

## Component Registry

- Markdown files: At most one active ticket should normally live here during a runner execution.

## Communication

The runner moves a pending ticket here before checking out `agile` and creating the Codex branch. If the run fails, inspect this folder before retrying.

#!/usr/bin/env node

const { spawnSync } = require("node:child_process");
const fs = require("node:fs");
const path = require("node:path");

const repoRoot = path.resolve(__dirname, "..", "..");
const autorunnerDir = path.join(repoRoot, ".codex-autorunner");
const ticketsDir = path.join(autorunnerDir, "tickets");
const workingDir = path.join(autorunnerDir, "working");
const doneDir = path.join(autorunnerDir, "done");
const baseBranch = process.env.CODEX_AUTORUNNER_BASE_BRANCH || "agile";
const codexCommand = process.env.CODEX_CLI_COMMAND || "codex";
const maxReviewDepth = Number.parseInt(
  process.env.CODEX_AUTORUNNER_MAX_REVIEW_DEPTH || "5",
  10,
);

function fail(message) {
  console.error(`\nERROR: ${message}`);
  process.exit(1);
}

function run(command, args, options = {}) {
  const result = spawnSync(command, args, {
    cwd: repoRoot,
    encoding: "utf8",
    shell: false,
    ...options,
  });

  if (result.error) {
    return {
      ok: false,
      status: result.status,
      stdout: result.stdout || "",
      stderr: result.error.message,
    };
  }

  return {
    ok: result.status === 0,
    status: result.status,
    stdout: result.stdout || "",
    stderr: result.stderr || "",
  };
}

function runChecked(command, args, message) {
  const result = run(command, args);
  if (!result.ok) {
    fail(`${message}\n${result.stderr || result.stdout}`.trim());
  }
  return result.stdout.trim();
}

function ensureDirectories() {
  for (const dir of [ticketsDir, workingDir, doneDir]) {
    fs.mkdirSync(dir, { recursive: true });
  }
}

function requireCommand(command, args, label) {
  const result = run(command, args);
  if (!result.ok) {
    fail(
      `${label} is required but was not available or could not run. Install/configure ${label} and retry.`,
    );
  }
}

function assertPrerequisites() {
  requireCommand("git", ["--version"], "Git");
  requireCommand("gh", ["--version"], "GitHub CLI");
}

function assertCleanWorktree() {
  const result = run("git", [
    "status",
    "--porcelain",
    "--untracked-files=all",
    "--",
    ".",
    ":(exclude).codex-autorunner",
  ]);

  if (!result.ok) {
    fail(`Unable to inspect git status.\n${result.stderr || result.stdout}`);
  }

  if (result.stdout.trim()) {
    fail(
      [
        "Git working tree is dirty outside .codex-autorunner.",
        "Commit, stash, or discard unrelated changes before running the autorunner.",
        "",
        result.stdout.trim(),
      ].join("\n"),
    );
  }
}

function getNextTicketPath() {
  const tickets = fs
    .readdirSync(ticketsDir)
    .filter((fileName) => fileName.toLowerCase().endsWith(".md"))
    .sort((left, right) => left.localeCompare(right));

  if (tickets.length === 0) {
    fail(`No Markdown tickets found in ${path.relative(repoRoot, ticketsDir)}.`);
  }

  return path.join(ticketsDir, tickets[0]);
}

function ticketNameFromPath(ticketPath) {
  return path.basename(ticketPath, path.extname(ticketPath));
}

function branchNameForTicket(ticketPath) {
  const safeName = ticketNameFromPath(ticketPath)
    .toLowerCase()
    .replace(/[^a-z0-9._/-]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .slice(0, 80);

  return `codex/${safeName || "ticket"}`;
}

function moveFile(sourcePath, targetDir) {
  const targetPath = path.join(targetDir, path.basename(sourcePath));
  if (fs.existsSync(targetPath)) {
    fail(`Cannot move ticket because target already exists: ${targetPath}`);
  }

  fs.renameSync(sourcePath, targetPath);
  return targetPath;
}

function assertBranchDoesNotExist(branchName) {
  const local = run("git", ["rev-parse", "--verify", "--quiet", branchName]);
  if (local.ok) {
    fail(`Branch already exists locally: ${branchName}`);
  }

  const remote = run("git", [
    "ls-remote",
    "--exit-code",
    "--heads",
    "origin",
    branchName,
  ]);
  if (remote.ok) {
    fail(`Branch already exists on origin: ${branchName}`);
  }
}

function checkoutFreshBranch(branchName) {
  runChecked("git", ["fetch", "origin", baseBranch], `Failed to fetch origin/${baseBranch}.`);
  const checkout = run("git", ["checkout", baseBranch]);
  if (!checkout.ok) {
    runChecked(
      "git",
      ["checkout", "-b", baseBranch, `origin/${baseBranch}`],
      `Failed to create local ${baseBranch} from origin/${baseBranch}.`,
    );
  }

  runChecked(
    "git",
    ["pull", "--ff-only", "origin", baseBranch],
    `Failed to pull latest ${baseBranch}.`,
  );
  runChecked(
    "git",
    ["checkout", "-b", branchName],
    `Failed to create branch ${branchName}.`,
  );
}

function buildCodexPrompt(ticketContent, ticketPath, branchName) {
  return `You are running from the local Codex autorunner.

Ticket path:
${path.relative(repoRoot, ticketPath)}

Target branch:
${branchName}

Full ticket content:
${ticketContent}

Rules:
- Keep changes focused.
- Do not edit unrelated files.
- Do not merge branches.
- If requirements are missing, document assumptions clearly.
- If the task is risky or unclear, make the smallest safe implementation.
- At the end, provide an implementation log including:
  - Intent
  - Interpretation
  - Changes Made
  - Files Changed
  - Assumptions
  - Missing Specification
  - Tests / Verification
  - Risk Level
`;
}

function runCodex(prompt) {
  console.log("\nRunning Codex CLI for one ticket...");
  const result = spawnSync(codexCommand, ["exec", prompt], {
    cwd: repoRoot,
    encoding: "utf8",
    shell: false,
    stdio: "inherit",
  });

  if (result.error) {
    fail(`Codex CLI failed to start: ${result.error.message}`);
  }

  if (result.status !== 0) {
    fail(`Codex CLI exited with status ${result.status}. Ticket remains in working/.`);
  }
}

function parseEnvFile(envPath) {
  if (!fs.existsSync(envPath)) return {};

  const values = {};
  const lines = fs.readFileSync(envPath, "utf8").split(/\r?\n/);
  for (const line of lines) {
    const trimmed = line.trim();
    if (!trimmed || trimmed.startsWith("#")) continue;

    const separatorIndex = trimmed.indexOf("=");
    if (separatorIndex === -1) continue;

    const key = trimmed.slice(0, separatorIndex).trim();
    const value = trimmed.slice(separatorIndex + 1).trim();
    values[key] = value.replace(/^["']|["']$/g, "");
  }

  return values;
}

function getNotionConfig() {
  const envValues = parseEnvFile(
    path.join(repoRoot, "tools", "notion-ticket-generator", ".env"),
  );

  return {
    token: process.env.NOTION_TOKEN || envValues.NOTION_TOKEN,
  };
}

function getNotionPageId(ticketContent) {
  const match = ticketContent.match(/^- Notion Page ID:\s*(.+)$/m);
  return match?.[1]?.trim();
}

async function updateNotionAfterCodex(ticketContent, branchName) {
  const pageId = getNotionPageId(ticketContent);
  const { token } = getNotionConfig();

  if (!pageId || !token) {
    console.log("\nSkipping Notion update: missing Notion Page ID or NOTION_TOKEN.");
    return;
  }

  const properties = {
    Status: { status: { name: "Codex Review" } },
    "Result Summary": {
      rich_text: [
        {
          text: {
            content: `Codex CLI completed local implementation on branch ${branchName}.`,
          },
        },
      ],
    },
    "Verification Result": {
      rich_text: [
        {
          text: {
            content:
              "Autorunner completed. Review Codex implementation log and local git status for verification details.",
          },
        },
      ],
    },
    "Agent Notes": {
      rich_text: [
        {
          text: {
            content: `Moved to Codex Review by local autorunner. Max review depth before Blocked: ${maxReviewDepth}.`,
          },
        },
      ],
    },
  };

  const response = await fetch(`https://api.notion.com/v1/pages/${pageId}`, {
    method: "PATCH",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
      "Notion-Version": "2022-06-28",
    },
    body: JSON.stringify({ properties }),
  });

  if (!response.ok) {
    const body = await response.text();
    console.warn(`\nWarning: Notion update failed (${response.status}). ${body}`);
    return;
  }

  console.log("\nUpdated Notion status: Codex Review");
}

function printNextSteps(branchName) {
  console.log("\nGit status:");
  spawnSync("git", ["status", "--short"], {
    cwd: repoRoot,
    encoding: "utf8",
    shell: false,
    stdio: "inherit",
  });

  console.log("\nNext manual commands:");
  console.log(`  git diff`);
  console.log(`  git add <files>`);
  console.log(`  git commit -m "Implement ticket"`);
  console.log(`  git push -u origin ${branchName}`);
  console.log(`  gh pr create --base ${baseBranch} --head ${branchName}`);
  console.log("\nNo merge was performed.");
}

async function main() {
  ensureDirectories();
  assertPrerequisites();
  assertCleanWorktree();
  requireCommand(codexCommand, ["--version"], "Codex CLI");

  const ticketPath = getNextTicketPath();
  const branchName = branchNameForTicket(ticketPath);
  assertBranchDoesNotExist(branchName);

  const workingTicketPath = moveFile(ticketPath, workingDir);
  const ticketContent = fs.readFileSync(workingTicketPath, "utf8");
  const prompt = buildCodexPrompt(ticketContent, workingTicketPath, branchName);

  checkoutFreshBranch(branchName);
  runCodex(prompt);

  await updateNotionAfterCodex(ticketContent, branchName);
  moveFile(workingTicketPath, doneDir);

  console.log(
    `\nMoved ticket to ${path.relative(repoRoot, path.join(doneDir, path.basename(workingTicketPath)))}.`,
  );
  printNextSteps(branchName);
}

main().catch((error) => {
  console.error(error);
  process.exit(1);
});

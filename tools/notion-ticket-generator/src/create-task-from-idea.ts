import "dotenv/config";
import { Client } from "@notionhq/client";
import fs from "node:fs";
import path from "node:path";

const notion = new Client({ auth: process.env.NOTION_TOKEN });
const configuredDataSourceId =
  process.env.NOTION_DATA_SOURCE_ID ?? process.env.NOTION_DATABASE_ID;

if (!configuredDataSourceId) {
  throw new Error(
    "Missing NOTION_DATA_SOURCE_ID in .env. You can also set NOTION_DATABASE_ID for backwards compatibility.",
  );
}

const dataSourceId = configuredDataSourceId;

const taskTypeAliases: Record<string, string> = {
  asset: "🖼️ asset",
  assets: "🖼️ asset",
  bug: "🐞 Bug",
  code: "✍️code",
  experiment: "🧪Experiment",
  feature: "💬 Feature request",
  "feature request": "💬 Feature request",
  polish: "💅 Polish",
  tech: "📉Tech Debt",
  "tech debt": "📉Tech Debt",
};

const riskLevelAliases: Record<string, string> = {
  high: "High",
  low: "Low",
  med: "Med",
  medium: "Med",
};

type CliOptions = {
  acceptance?: string;
  agentNotes?: string;
  assigneeIds?: string;
  description?: string;
  dryRun: boolean;
  dueDate?: string;
  files?: string;
  gdd?: string;
  idea?: string;
  ideaFile?: string;
  priority?: string;
  resultSummary?: string;
  riskLevel?: string;
  status: string;
  taskId?: string;
  taskType?: string;
  technical?: string;
  tests?: string;
  title?: string;
  verificationResult?: string;
};

type TaskDraft = {
  acceptanceCriteria: string;
  agentBrief: string;
  agentNotes: string;
  assigneeIds: string[];
  clientReferenceId?: string;
  description: string;
  dueDate?: string;
  filesAllowed: string;
  gddLink?: string;
  idea: string;
  priority?: string;
  resultSummary?: string;
  riskLevels: string[];
  status: string;
  taskTypes: string[];
  technicalNotes: string;
  testInstructions: string;
  title: string;
  verificationResult?: string;
};

function readArgValue(args: string[], index: number, flag: string): string {
  const value = args[index + 1];
  if (!value || value.startsWith("--")) {
    throw new Error(`Missing value for ${flag}.`);
  }
  return value;
}

function parseArgs(args: string[]): CliOptions {
  const options: CliOptions = {
    dryRun: false,
    status: "Codex Ready",
  };

  for (let index = 0; index < args.length; index += 1) {
    const arg = args[index];

    if (arg === "--dry-run") {
      options.dryRun = true;
      continue;
    }

    if (arg === "--help" || arg === "-h") {
      printHelp();
      process.exit(0);
    }

    if (!arg.startsWith("--")) {
      options.idea = [options.idea, arg].filter(Boolean).join(" ");
      continue;
    }

    const [flag, inlineValue] = arg.split("=", 2);
    const value = inlineValue ?? readArgValue(args, index, flag);
    if (!inlineValue) index += 1;

    switch (flag) {
      case "--acceptance":
        options.acceptance = value;
        break;
      case "--agent-notes":
        options.agentNotes = value;
        break;
      case "--assignee-id":
      case "--assignee-ids":
        options.assigneeIds = value;
        break;
      case "--description":
        options.description = value;
        break;
      case "--due-date":
        options.dueDate = value;
        break;
      case "--files":
        options.files = value;
        break;
      case "--gdd":
        options.gdd = value;
        break;
      case "--idea":
        options.idea = value;
        break;
      case "--idea-file":
        options.ideaFile = value;
        break;
      case "--priority":
        options.priority = value;
        break;
      case "--result-summary":
        options.resultSummary = value;
        break;
      case "--risk":
      case "--risk-level":
        options.riskLevel = value;
        break;
      case "--status":
        options.status = value;
        break;
      case "--task-id":
        options.taskId = value;
        break;
      case "--task-type":
      case "--type":
        options.taskType = value;
        break;
      case "--technical":
        options.technical = value;
        break;
      case "--tests":
        options.tests = value;
        break;
      case "--title":
        options.title = value;
        break;
      case "--verification-result":
        options.verificationResult = value;
        break;
      default:
        throw new Error(`Unknown option: ${flag}`);
    }
  }

  return options;
}

function printHelp() {
  console.log(`Create a Notion implementation task from an idea.

Usage:
  npm run create:task -- --idea "Add a combo meter to brick breaking"
  npm run create:task -- --title "Combo meter" --idea-file ./idea.md
  npm run create:task -- --dry-run --idea "Improve paddle input feel"

Options:
  --idea                  Implementation idea text. Positional text is also accepted.
  --idea-file             Read the idea from a text or markdown file.
  --title                 Override generated task title.
  --description           General description/grouping text.
  --acceptance            Override generated acceptance criteria.
  --technical             Add technical notes or constraints.
  --files                 Files or folders Codex is allowed to modify.
  --tests                 Test/build instructions for Codex.
  --gdd                   Optional GDD/design URL.
  --status                Notion status. Defaults to "Codex Ready".
  --priority              Low, Medium, or High.
  --task-type             One or more Task type values, separated by commas or semicolons.
  --risk-level            One or more Risk Level values: Low, Med, High.
  --assignee-id           One or more Notion person IDs, separated by commas or semicolons.
  --due-date              ISO date or DD/MM/YYYY. Time is optional.
  --result-summary        Optional initial Result Summary text.
  --verification-result   Optional initial Verification Result text.
  --agent-notes           Extra notes for the coding agent.
  --task-id               Client reference only. Notion Task ID is auto-increment and read-only.
  --dry-run               Print the task draft without creating a Notion page.
`);
}

function readIdea(options: CliOptions): string {
  if (options.ideaFile) {
    const fullPath = path.resolve(options.ideaFile);
    return fs.readFileSync(fullPath, "utf8").trim();
  }

  if (options.idea?.trim()) {
    return options.idea.trim();
  }

  if (!process.stdin.isTTY) {
    return fs.readFileSync(0, "utf8").trim();
  }

  return "";
}

function firstSentence(value: string): string {
  return value
    .replace(/\s+/g, " ")
    .split(/(?<=[.!?])\s+/)[0]
    .replace(/[.!?]+$/, "")
    .trim();
}

function titleFromIdea(idea: string): string {
  const sentence = firstSentence(idea);
  if (!sentence) return "New implementation task";

  const cleaned = sentence
    .replace(
      /^(please\s+)?(add|build|create|implement|make|improve|fix)\s+/i,
      "",
    )
    .trim();

  const title = cleaned || sentence;
  return title.length > 80 ? `${title.slice(0, 77).trim()}...` : title;
}

function generateClientReferenceId(title: string): string {
  const date = new Date().toISOString().slice(0, 10).replace(/-/g, "");
  const slug = title
    .toUpperCase()
    .replace(/[^A-Z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .slice(0, 24);

  return `IDEA-${date}-${slug || "TASK"}`;
}

function normalizeListText(value: string): string {
  return value
    .split(/\r?\n|;/)
    .map((line) => line.trim())
    .filter(Boolean)
    .map((line) => (line.startsWith("- ") ? line : `- ${line}`))
    .join("\n");
}

function splitNames(value?: string): string[] {
  if (!value?.trim()) return [];
  return value
    .split(/[,;]/)
    .map((item) => item.trim())
    .filter(Boolean);
}

function normalizeNames(values: string[], aliases: Record<string, string>): string[] {
  return values.map((value) => aliases[value.toLowerCase()] ?? value);
}

function buildAcceptanceCriteria(idea: string, override?: string): string {
  if (override?.trim()) return normalizeListText(override);

  return [
    "- The requested behavior is implemented in the relevant gameplay/UI flow.",
    "- Existing behavior outside the requested idea remains unchanged unless required.",
    "- Edge cases from the idea are handled gracefully and do not create runtime errors.",
    "- The implementation is small enough for Codex to review and summarize clearly.",
  ].join("\n");
}

function buildTechnicalNotes(idea: string, override?: string): string {
  const notes = [
    "Start by reading the existing implementation around the affected gameplay, UI, and data flow before editing.",
    "Prefer the current project patterns over new architecture.",
    "Keep the implementation scoped to the idea below.",
  ];

  if (override?.trim()) {
    notes.push("Additional constraints from task creator:", override.trim());
  }

  notes.push(`Original idea: ${idea}`);
  return normalizeListText(notes.join("\n"));
}

function buildTestInstructions(override?: string): string {
  if (override?.trim()) return normalizeListText(override);

  return [
    "- Run the relevant project build or test command if available.",
    "- If there is no automated coverage, manually verify the changed gameplay/UI path.",
    "- Report any checks that could not be run and why.",
  ].join("\n");
}

function normalizeDueDate(value?: string): string | undefined {
  if (!value?.trim()) return undefined;
  const trimmed = value.trim();

  if (/^\d{4}-\d{2}-\d{2}/.test(trimmed)) {
    return trimmed;
  }

  const match = trimmed.match(
    /^(\d{1,2})\/(\d{1,2})\/(\d{4})(?:[ ,T]+(\d{1,2}):(\d{2}))?$/,
  );
  if (!match) {
    throw new Error(
      `Invalid --due-date "${value}". Use ISO date or DD/MM/YYYY with optional HH:mm.`,
    );
  }

  const [, day, month, year, hour, minute] = match;
  const date = `${year}-${month.padStart(2, "0")}-${day.padStart(2, "0")}`;
  if (!hour || !minute) return date;
  return `${date}T${hour.padStart(2, "0")}:${minute}:00+07:00`;
}

function buildAgentNotes(options: CliOptions, clientReferenceId: string): string {
  const notes = [
    options.agentNotes?.trim(),
    `Client reference: ${options.taskId?.trim() || clientReferenceId}`,
    "Generated from create-task-from-idea.ts.",
  ];

  return notes.filter(Boolean).join("\n");
}

function buildAgentBrief(draft: Omit<TaskDraft, "agentBrief">): string {
  return `You are implementing a focused Notion task generated from a product idea.

Goal:
${draft.idea}

Description:
${draft.description}

Before editing:
- Inspect the existing code path and nearby conventions.
- Identify the smallest set of files needed for the change.
- Avoid unrelated rewrites.

Implementation expectations:
${draft.acceptanceCriteria}

Technical guidance:
${draft.technicalNotes}

Allowed files:
${draft.filesAllowed}

Verification:
${draft.testInstructions}

Agent notes:
${draft.agentNotes}`;
}

function buildDraft(options: CliOptions): TaskDraft {
  const idea = readIdea(options);
  if (!idea) {
    throw new Error(
      "Provide an idea with --idea, --idea-file, positional text, or stdin.",
    );
  }

  const title = options.title?.trim() || titleFromIdea(idea);
  const clientReferenceId =
    options.taskId?.trim() || generateClientReferenceId(title);
  const withoutBrief = {
    acceptanceCriteria: buildAcceptanceCriteria(idea, options.acceptance),
    agentNotes: buildAgentNotes(options, clientReferenceId),
    assigneeIds: splitNames(options.assigneeIds),
    clientReferenceId,
    description: options.description?.trim() || firstSentence(idea) || idea,
    dueDate: normalizeDueDate(options.dueDate),
    filesAllowed:
      options.files?.trim() ||
      "- Not specified. Prefer minimal changes and avoid unrelated systems.",
    gddLink: options.gdd?.trim(),
    idea,
    priority: options.priority?.trim(),
    resultSummary: options.resultSummary?.trim(),
    riskLevels: normalizeNames(splitNames(options.riskLevel), riskLevelAliases),
    status: options.status,
    taskTypes: normalizeNames(splitNames(options.taskType), taskTypeAliases),
    technicalNotes: buildTechnicalNotes(idea, options.technical),
    testInstructions: buildTestInstructions(options.tests),
    title,
    verificationResult: options.verificationResult?.trim(),
  };

  return {
    ...withoutBrief,
    agentBrief: buildAgentBrief(withoutBrief),
  };
}

function richText(content: string) {
  const safeContent =
    content.length > 1900 ? `${content.slice(0, 1897)}...` : content;
  return { rich_text: [{ text: { content: safeContent } }] };
}

function titleText(content: string) {
  return { title: [{ text: { content } }] };
}

function select(name: string) {
  return { select: { name } };
}

function multiSelect(names: string[]) {
  return { multi_select: names.map((name) => ({ name })) };
}

function people(ids: string[]) {
  return { people: ids.map((id) => ({ id })) };
}

function date(start: string) {
  return { date: { start } };
}

function paragraph(text: string) {
  return {
    object: "block",
    type: "paragraph",
    paragraph: {
      rich_text: [{ type: "text", text: { content: text.slice(0, 1900) } }],
    },
  };
}

function heading(text: string) {
  return {
    object: "block",
    type: "heading_2",
    heading_2: {
      rich_text: [{ type: "text", text: { content: text } }],
    },
  };
}

function codeBlock(text: string) {
  return {
    object: "block",
    type: "code",
    code: {
      language: "markdown",
      rich_text: [{ type: "text", text: { content: text.slice(0, 1900) } }],
    },
  };
}

function chunkText(text: string, size = 1800): string[] {
  const chunks: string[] = [];
  for (let index = 0; index < text.length; index += size) {
    chunks.push(text.slice(index, index + size));
  }
  return chunks;
}

function buildPageChildren(draft: TaskDraft) {
  return [
    heading("Implementation Idea"),
    ...chunkText(draft.idea).map(paragraph),
    heading("Agent Brief"),
    ...chunkText(draft.agentBrief).map(codeBlock),
  ];
}

function buildPageProperties(draft: TaskDraft) {
  const properties: Record<string, any> = {
    "Task name": titleText(draft.title),
    Status: { status: { name: draft.status } },
    Description: richText(draft.description),
    "Acceptance Criteria": richText(draft.acceptanceCriteria),
    "Technical Notes": richText(draft.technicalNotes),
    "Files Allowed": richText(draft.filesAllowed),
    "Test Instructions": richText(draft.testInstructions),
    "Agent Notes": richText(draft.agentNotes),
  };

  if (draft.priority) {
    properties.Priority = select(draft.priority);
  }

  if (draft.taskTypes.length > 0) {
    properties["Task type"] = multiSelect(draft.taskTypes);
  }

  if (draft.riskLevels.length > 0) {
    properties["Risk Level"] = multiSelect(draft.riskLevels);
  }

  if (draft.assigneeIds.length > 0) {
    properties.Assignee = people(draft.assigneeIds);
  }

  if (draft.dueDate) {
    properties["Due date"] = date(draft.dueDate);
  }

  if (draft.gddLink) {
    properties["GDD Link"] = { url: draft.gddLink };
  }

  if (draft.resultSummary) {
    properties["Result Summary"] = richText(draft.resultSummary);
  }

  if (draft.verificationResult) {
    properties["Verification Result"] = richText(draft.verificationResult);
  }

  return properties;
}

function getNotionTaskId(page: any): string {
  const taskId = page.properties?.["Task ID"]?.unique_id;
  if (!taskId) return "";
  return `${taskId.prefix ?? ""}${taskId.number ?? ""}`;
}

async function createNotionTask(draft: TaskDraft) {
  return notion.pages.create({
    parent: { data_source_id: dataSourceId },
    properties: buildPageProperties(draft),
    children: buildPageChildren(draft),
  } as any);
}

async function main() {
  const options = parseArgs(process.argv.slice(2));
  const draft = buildDraft(options);

  if (options.dryRun) {
    console.log(JSON.stringify(draft, null, 2));
    return;
  }

  const page = await createNotionTask(draft);
  const notionTaskId = getNotionTaskId(page);

  console.log(`Created Notion task: ${draft.title}`);
  console.log(`Task ID: ${notionTaskId || "assigned by Notion"}`);
  console.log(`Client reference: ${draft.clientReferenceId}`);
  console.log(`Status: ${draft.status}`);
  console.log(`Page ID: ${page.id}`);
}

main().catch((error) => {
  console.error(error);
  process.exit(1);
});

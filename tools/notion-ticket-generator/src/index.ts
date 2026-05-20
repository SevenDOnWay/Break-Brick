import "dotenv/config";
import { Client } from "@notionhq/client";
import fs from "node:fs";
import path from "node:path";

const notion = new Client({ auth: process.env.NOTION_TOKEN });
const configuredDataSourceId =
  process.env.NOTION_DATA_SOURCE_ID ?? process.env.NOTION_DATABASE_ID;
const repoRoot = process.env.REPO_ROOT ?? "../..";

if (!configuredDataSourceId) {
  throw new Error(
    "Missing NOTION_DATA_SOURCE_ID in .env. You can also set NOTION_DATABASE_ID for backwards compatibility.",
  );
}

const dataSourceId = configuredDataSourceId;

type NotionPage = any;

function slugify(value: string): string {
  return value
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .slice(0, 80);
}

function getTitle(page: NotionPage): string {
  const titleProp = page.properties["Task name"];
  return (
    titleProp?.title?.map((text: any) => text.plain_text).join("") ??
    "Untitled Task"
  );
}

function getPropertyText(page: NotionPage, propertyName: string): string {
  const prop = page.properties[propertyName];
  if (!prop) return "";

  if (prop.title) {
    return prop.title.map((text: any) => text.plain_text).join("");
  }

  if (prop.rich_text) {
    return prop.rich_text.map((text: any) => text.plain_text).join("");
  }

  if (prop.unique_id) {
    return `${prop.unique_id.prefix ?? ""}${prop.unique_id.number ?? ""}`;
  }

  if (prop.status) {
    return prop.status.name ?? "";
  }

  if (prop.select) {
    return prop.select.name ?? "";
  }

  if (prop.multi_select) {
    return prop.multi_select.map((item: any) => item.name).join(", ");
  }

  if (prop.people) {
    return prop.people
      .map((person: any) => person.name ?? person.id)
      .filter(Boolean)
      .join(", ");
  }

  if (prop.date) {
    return [prop.date.start, prop.date.end ? `to ${prop.date.end}` : ""]
      .filter(Boolean)
      .join(" ");
  }

  if (prop.url) {
    return prop.url;
  }

  return "";
}

function getUrl(page: NotionPage, propertyName: string): string {
  const prop = page.properties[propertyName];
  return prop?.url ?? "";
}

function fieldLine(label: string, value: string): string {
  return value ? `- ${label}: ${value}\n` : "";
}

function section(title: string, body: string, fallback: string): string {
  return `## ${title}\n\n${body || fallback}\n`;
}

function buildTicketMarkdown(page: NotionPage): string {
  const title = getTitle(page);
  const taskId = getPropertyText(page, "Task ID") || page.id;
  const status = getPropertyText(page, "Status");
  const priority = getPropertyText(page, "Priority");
  const taskType = getPropertyText(page, "Task type");
  const riskLevel = getPropertyText(page, "Risk Level");
  const assignee = getPropertyText(page, "Assignee");
  const dueDate = getPropertyText(page, "Due date");
  const description = getPropertyText(page, "Description");
  const gddLink = getUrl(page, "GDD Link");
  const acceptanceCriteria = getPropertyText(page, "Acceptance Criteria");
  const technicalNotes = getPropertyText(page, "Technical Notes");
  const filesAllowed = getPropertyText(page, "Files Allowed");
  const testInstructions = getPropertyText(page, "Test Instructions");
  const resultSummary = getPropertyText(page, "Result Summary");
  const verificationResult = getPropertyText(page, "Verification Result");
  const agentNotes = getPropertyText(page, "Agent Notes");

  return `# ${title}

## Source

- Notion Task ID: ${taskId}
- Notion Page ID: ${page.id}
${fieldLine("Status", status)}${fieldLine("Priority", priority)}${fieldLine("Task type", taskType)}${fieldLine("Risk level", riskLevel)}${fieldLine("Assignee", assignee)}${fieldLine("Due date", dueDate)}${fieldLine("GDD Link", gddLink)}
## Goal

Implement the task described by this ticket.

${section("Description", description, "- No description provided.")}
${section(
    "Acceptance Criteria",
    acceptanceCriteria,
    "- No acceptance criteria provided. Infer carefully from the task title and technical notes, but avoid unrelated changes.",
  )}
${section("Technical Notes", technicalNotes, "- No technical notes provided.")}
${section(
    "Files Allowed to Modify",
    filesAllowed,
    "- Not specified. Prefer minimal changes and avoid unrelated systems.",
  )}
${section(
    "Test Instructions",
    testInstructions,
    "- Run the project/build/test steps that are available in the repository.",
  )}
${section("Agent Notes", agentNotes, "- No agent notes provided.")}
${section("Previous Result Summary", resultSummary, "- No previous result summary.")}
${section(
    "Previous Verification Result",
    verificationResult,
    "- No previous verification result.",
  )}
## Rules for Codex

- Read existing module README files before editing code.
- Keep the change focused on this task only.
- Do not rewrite unrelated architecture.
- Prefer small, testable changes.
- After implementation, summarize modified files.
- If the task is ambiguous, make the smallest reasonable implementation and document assumptions.

## Expected Result

- Code is implemented.
- Local checks are run if available.
- Notion can be updated with Result Summary, Verification Result, Agent Notes, and changed files if requested.
`;
}

async function updateStatus(pageId: string, statusName: string) {
  await notion.pages.update({
    page_id: pageId,
    properties: { Status: { status: { name: statusName } } },
  });
}

async function updateTicketPath(pageId: string, ticketPath: string) {
  await notion.pages.update({
    page_id: pageId,
    properties: {
      "Ticket Path": { rich_text: [{ text: { content: ticketPath } }] },
    },
  });
}

async function main() {
  const response = await notion.dataSources.query({
    data_source_id: dataSourceId,
    filter: { property: "Status", status: { equals: "Codex Ready" } },
  });

  if (response.results.length === 0) {
    console.log("No tasks with Status = Codex Ready.");
    return;
  }

  const ticketsDir = path.resolve(repoRoot, ".codex-autorunner", "tickets");
  fs.mkdirSync(ticketsDir, { recursive: true });

  for (const page of response.results as NotionPage[]) {
    const title = getTitle(page);
    const slug = slugify(title);
    const date = new Date().toISOString().slice(0, 10);
    const fileName = `${date}-${slug}.md`;
    const fullPath = path.join(ticketsDir, fileName);

    if (fs.existsSync(fullPath)) {
      console.log(`Skipping existing ticket: ${fileName}`);
      continue;
    }

    const markdown = buildTicketMarkdown(page);
    fs.writeFileSync(fullPath, markdown, "utf8");

    const relativeTicketPath = `.codex-autorunner/tickets/${fileName}`;
    await updateTicketPath(page.id, relativeTicketPath);
    await updateStatus(page.id, "Codex Working");

    console.log(`Created ticket: ${relativeTicketPath}`);
    console.log(`Updated Notion status: ${title} -> Codex Working`);
  }
}

main().catch((error) => {
  console.error(error);
  process.exit(1);
});

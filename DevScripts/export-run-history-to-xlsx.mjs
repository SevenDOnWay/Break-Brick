import fs from "node:fs/promises";
import path from "node:path";
import { SpreadsheetFile, Workbook } from "@oai/artifact-tool";

const [inputPath, outputPath = "run-history.xlsx"] = process.argv.slice(2);

if (!inputPath) {
  console.error("Usage: node export-run-history-to-xlsx.mjs <run-history.json> [output.xlsx]");
  process.exit(1);
}

const run = JSON.parse(await fs.readFile(inputPath, "utf8"));
const turns = Array.isArray(run.turnHistory) ? run.turnHistory : [];
const workbook = Workbook.create();
const dashboard = workbook.worksheets.add("Dashboard");
const turnsSheet = workbook.worksheets.add("Turns");
const damageSheet = workbook.worksheets.add("Damage Sources");
const upgradesSheet = workbook.worksheets.add("Upgrades");

const darkBlue = "#1F4E78";
const lightBlue = "#D9EAF7";
const headerFormat = {
  fill: darkBlue,
  font: { bold: true, color: "#FFFFFF" },
  horizontalAlignment: "center",
};

function applyHeader(sheet, range) {
  sheet.getRange(range).format = headerFormat;
}

function setWidths(sheet, widths) {
  Object.entries(widths).forEach(([column, width]) => {
    sheet.getRange(`${column}:${column}`).format.columnWidth = width;
  });
}

const turnRows = turns.map((turn) => [
  turn.turnIndex ?? 0,
  turn.waveIndex ?? 0,
  turn.ballCount ?? 0,
  turn.damageDealt ?? 0,
  turn.bricksDestroyed ?? 0,
  Array.isArray(turn.upgrades) ? turn.upgrades.length : 0,
]);

turnsSheet.getRange("A1:F1").values = [[
  "Turn",
  "Wave",
  "Balls",
  "Damage dealt",
  "Bricks destroyed",
  "Upgrades equipped",
]];
applyHeader(turnsSheet, "A1:F1");
if (turnRows.length > 0) {
  turnsSheet.getRange(`A2:F${turnRows.length + 1}`).values = turnRows;
  turnsSheet.tables.add(`A1:F${turnRows.length + 1}`, true, "TurnsTable");
}
turnsSheet.freezePanes.freezeRows(1);
turnsSheet.showGridLines = false;
setWidths(turnsSheet, { A: 10, B: 10, C: 10, D: 16, E: 18, F: 20 });

const damageRows = [];
const upgradeRows = [];
turns.forEach((turn) => {
  (turn.damageBySource ?? []).forEach((entry) => {
    damageRows.push([turn.turnIndex ?? 0, turn.waveIndex ?? 0, entry.source ?? "Unknown", entry.damageDealt ?? 0]);
  });
  (turn.upgrades ?? []).forEach((upgrade) => {
    upgradeRows.push([
      turn.turnIndex ?? 0,
      turn.waveIndex ?? 0,
      upgrade.upgradeId ?? "",
      upgrade.upgradeName ?? "",
    ]);
  });
});

damageSheet.getRange("A1:D1").values = [["Turn", "Wave", "Damage source", "Damage dealt"]];
applyHeader(damageSheet, "A1:D1");
if (damageRows.length > 0) {
  damageSheet.getRange(`A2:D${damageRows.length + 1}`).values = damageRows;
  damageSheet.tables.add(`A1:D${damageRows.length + 1}`, true, "DamageSourcesTable");
}
damageSheet.freezePanes.freezeRows(1);
damageSheet.showGridLines = false;
setWidths(damageSheet, { A: 10, B: 10, C: 20, D: 16 });

upgradesSheet.getRange("A1:D1").values = [["Turn", "Wave", "Upgrade ID", "Upgrade name"]];
applyHeader(upgradesSheet, "A1:D1");
if (upgradeRows.length > 0) {
  upgradesSheet.getRange(`A2:D${upgradeRows.length + 1}`).values = upgradeRows;
  upgradesSheet.tables.add(`A1:D${upgradeRows.length + 1}`, true, "UpgradesTable");
}
upgradesSheet.freezePanes.freezeRows(1);
upgradesSheet.showGridLines = false;
setWidths(upgradesSheet, { A: 10, B: 10, C: 36, D: 26 });

const finalTurnRow = Math.max(turnRows.length + 1, 2);
const finalDamageRow = Math.max(damageRows.length + 1, 2);
dashboard.getRange("A1:H1").merge();
dashboard.getRange("A1").values = [["Run history balance report"]];
dashboard.getRange("A1:H1").format = {
  fill: darkBlue,
  font: { bold: true, color: "#FFFFFF", size: 16 },
  horizontalAlignment: "center",
};

dashboard.getRange("A3:B7").values = [
  ["Run ID", run.runId ?? "Unknown"],
  ["Difficulty", run.difficult ?? "Unknown"],
  ["Turns played", null],
  ["Total damage", null],
  ["Bricks destroyed", null],
];
dashboard.getRange("B5:B7").formulas = [
  [`=COUNTA('Turns'!$A$2:$A$${finalTurnRow})`],
  [`=SUM('Turns'!$D$2:$D$${finalTurnRow})`],
  [`=SUM('Turns'!$E$2:$E$${finalTurnRow})`],
];
dashboard.getRange("A3:A7").format = { fill: lightBlue, font: { bold: true } };
dashboard.getRange("A3:B7").format.borders = { preset: "outside", style: "thin", color: "#A6A6A6" };
dashboard.getRange("B5:B7").format.numberFormat = "#,##0";

dashboard.getRange("A10:B10").values = [["Turn", "Damage dealt"]];
applyHeader(dashboard, "A10:B10");
if (turnRows.length > 0) {
  dashboard.getRange("A11").formulas = [["='Turns'!A2"]];
  dashboard.getRange("B11").formulas = [["='Turns'!D2"]];
  dashboard.getRange(`A11:B${turnRows.length + 10}`).fillDown();
}

const sourceNames = [...new Set(damageRows.map((row) => row[2]))].sort();
dashboard.getRange("D10:E10").values = [["Damage source", "Total damage"]];
applyHeader(dashboard, "D10:E10");
if (sourceNames.length > 0) {
  dashboard.getRange(`D11:D${sourceNames.length + 10}`).values = sourceNames.map((source) => [source]);
  dashboard.getRange("E11").formulas = [[`=SUMIF('Damage Sources'!$C$2:$C$${finalDamageRow},D11,'Damage Sources'!$D$2:$D$${finalDamageRow})`]];
  dashboard.getRange(`E11:E${sourceNames.length + 10}`).fillDown();
}

if (turnRows.length > 0) {
  const damageChart = dashboard.charts.add("line", dashboard.getRange(`A10:B${turnRows.length + 10}`));
  damageChart.title = "Damage dealt per turn";
  damageChart.hasLegend = false;
  damageChart.xAxis = { axisType: "textAxis" };
  damageChart.yAxis = { numberFormatCode: "#,##0" };
  damageChart.setPosition("G3", "N17");
}

if (sourceNames.length > 0) {
  const sourceChart = dashboard.charts.add("bar", dashboard.getRange(`D10:E${sourceNames.length + 10}`));
  sourceChart.title = "Damage by source";
  sourceChart.hasLegend = false;
  sourceChart.xAxis = { numberFormatCode: "#,##0" };
  sourceChart.setPosition("G19", "N34");
}

dashboard.showGridLines = false;
setWidths(dashboard, { A: 20, B: 32, C: 5, D: 20, E: 16, F: 5 });
dashboard.getRange("A10:E30").format.wrapText = true;

await fs.mkdir(path.dirname(outputPath), { recursive: true });
const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(outputPath);
console.log(`Exported ${outputPath}`);

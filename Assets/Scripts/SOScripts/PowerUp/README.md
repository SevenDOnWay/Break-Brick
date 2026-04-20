# SOScripts/PowerUp/ — Upgrade & Process System

## Module Summary

The core **data-driven upgrade system**. Contains the ScriptableObject definitions for upgrades (stats and behaviors) and the runtime `Process` classes that execute on-hit effects during gameplay.

## Sub-Folder Index

| Folder | Purpose |
|---|---|
| `Process/` | Concrete `Process` subclasses (explosions, lightning, freeze, etc.). |
| `Behavior/` | `IBehavior` implementations (e.g., magnet toggle). |

## Root Scripts

| Script | Type | Purpose |
|---|---|---|
| `UpgradeStatSO` | `ScriptableObject` | Stat-only upgrade: modifies `StatManager` values and returns its applied key/value data to the caller. |
| `UpgradeBehaviorSO` | `ScriptableObject` | Behavior-based upgrade: applies `IBehavior` implementations (e.g., magnet). |
| `IBehavior` | Interface | Contract for behavior upgrades: `Apply(UpgradeManager)` and `Type`. |
| `Process` (base class) | Abstract class | Base for on-hit process strategies. Provides `GetChance()`, `CheckChance()`, `OnHit(BrickScript, ...)`. |
| `IProcess` | Interface | Minimal process contract marker. |

## Class Interactions

### Internal

1. `UpgradeStatSO.ApplyStat()` calls `StatManager.ModifyStat()` and returns its `UpgradePair` data.
2. `UpgradeManager` consumes that returned data, handles non-stat actions like `ExtraBalls`, then creates/registers processes via `ProcessFactory.CreateProcess()`.
3. `UpgradeBehaviorSO.ApplyBehavior()` iterates its `IBehavior[]` list and calls `Apply(upgradeManager)`.

### External

| Dependency | Relationship |
|---|---|
| `Manager/StatManager` | `UpgradeStatSO` modifies stats. |
| `Manager/UpgradeManager` | Applies returned stat-pair data, registers processes, and activates behaviors. |
| `Utils/ProcessFactory` | Used by `UpgradeManager` to create process instances. |
| `VFX/VFXEvent` | `Process` subclasses raise VFX commands. |
| `BrickVariant/BrickEffect/*` | Some processes (Freeze, Poison) add effect components to bricks. |

## Design Patterns

- **Strategy** — Each `Process` subclass is an interchangeable on-hit strategy.
- **Template Method** — `Process.OnHit()` calls `CheckChance()` → subclass `Execute()`.
- **Composite** — `UpgradeBehaviorSO` holds an array of `IBehavior` implementations.

## Mermaid Diagram

```mermaid
sequenceDiagram
    participant USO as UpgradeStatSO
    participant SM as StatManager
    participant PF as ProcessFactory
    participant UM as UpgradeManager
    participant BS as BallScript

    Note over USO: Player picks upgrade
    USO->>SM: ModifyStat(type, value)
    USO-->>UM: Return applied UpgradePair list
    UM->>PF: CreateProcess(type)
    PF-->>UM: Process instance
    UM->>UM: ApplyProcess(process)

    Note over BS: Ball hits brick
    BS->>UM: GetCurrentProcess()
    loop Each Process
        BS->>Process: OnHit(brick, ball, ...)
        alt Chance check passes
            Process->>Process: Execute()
            Process->>VFXEvent: RaiseVFXCommand()
        end
    end
```

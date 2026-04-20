# SOScripts/ — ScriptableObject Data Layer

## Module Summary

**Data-driven configuration** for the upgrade/powerup system, character definitions, and upgrade databases. ScriptableObjects serve as the **immutable data bridge** between authoring (Inspector) and runtime logic.

## Sub-Folder Index

| Folder | Purpose |
|---|---|
| `PowerUp/` | Upgrade definitions (`UpgradeSO`, `UpgradeStatSO`, `UpgradeBehaviorSO`) and runtime logic (`Process`, `IBehavior`). |
| `CharacterPowerUp/` | Character definitions (`CharacterSO`) that bundle stat and behavior upgrades. |

## Root Scripts

| Script | Type | Purpose |
|---|---|---|
| `UpgradeSO` | `ScriptableObject` | Base upgrade definition: name, type, description, icon, rarity. Contains a reference to either `UpgradeStatSO` or `UpgradeBehaviorSO` (or both). |

## External Dependencies

| Dependency | Relationship |
|---|---|
| `Enum/UpgradeType` | Keys for stat upgrades. |
| `Enum/UpgradeBehaviourType` | Keys for behavior upgrades. |
| `GamePlayScripts/Manager/StatManager` | `UpgradeStatSO.ApplyStat()` modifies stats. |
| `GamePlayScripts/Manager/UpgradeManager` | Manages upgrade inventory. |
| `Utils/ProcessFactory` | `UpgradeManager` creates processes via factory from `UpgradeStatSO` returned data. |

## Design Patterns

- **ScriptableObject as Data Bridge** — Separates design-time configuration from runtime execution. Designers can tweak values in the Inspector without touching code.
- **Strategy** — `Process` subclasses and `IBehavior` implementations are interchangeable strategies attached to upgrade SOs.

## Mermaid Diagram
```mermaid
graph TD

    subgraph SOScripts [Data Layer: ScriptableObjects]
        USO[UpgradeSO]
        USSO[UpgradeStatSO]
        UBSO[UpgradeBehaviorSO]
        CSO[CharacterSO]
    end

    subgraph ProcessLayer [Logic Layer: Processes]
        P[Process base]
        EP[ExplosionProcess]
        LP[LightningProcess]
        CP[CritProcess]
        FRP[FreezeProcess]
        PP[PoisonProcess]
        SP[SniperProcess]
        SWP[ShockwaveProcess]
        RP[RallyProcess]
    end

    subgraph BehaviorLayer [Execution Layer: Behaviors]
        IB{IBehavior}
        MB[MagnetBehavior]
    end

    %% Data Relationships
    USO --> USSO
    USO --> UBSO
    
    %% Dependency Injection / References
    USSO -.-> P
    UBSO -.-> IB
    
    %% Inheritance (Represented by standard arrows for stability)
    P --> EP
    P --> LP
    P --> CP
    P --> FRP
    P --> PP
    P --> SP
    P --> SWP
    P --> RP
    
    %% Implementation
    IB -.-> MB
    
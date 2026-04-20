# Enum/ — Shared Enumeration Types

## Module Summary

Centralized enum definitions used across **all modules** as type-safe identifiers for game systems (bricks, upgrades, VFX, damage sources).

## Scripts

| Script | Purpose | Key Consumers |
|---|---|---|
| `BrickType` | Identifies brick variant types (`Normal`, `Split`, `x2health`, etc.) | `SpawnController`, `BrickData`, `IBrickVariant` |
| `DamageSource` | Tags the origin of damage (`Ball`, `Explosion`, `Lightning`, `Poison`, etc.) | `BrickScript`, `BrickManager`, all `Process` classes |
| `ProcessType` | Identifies on-hit process types (`Crit`, `Explosion`, `Freeze`, etc.) | `Process` subclasses, `ProcessFactory` |
| `UpgradeType` | Keys for the stat dictionary (`Speed`, `CritChance`, `ExplosionRadius`, etc.) | `StatManager`, `UpgradeStatSO`, `ProcessFactory` |
| `UpgradeBehaviourType` | Identifies behavior-type upgrades (`Magnet`, `Crit`, etc.) | `IBehavior`, `MagnetBehavior` |
| `VFXType` | Tags visual effect types (`Explosion`, `Lightning`, `Freeze`, etc.) | `IVFXCommand`, `VFXManager`, `VFXPlayerBase` |

## Class Interactions

### Internal
No inter-dependencies; each enum is independent.

### External
These enums are **consumed globally** — they serve as the shared vocabulary between data layers (`SOScripts/`), runtime logic (`GamePlayScripts/`), and visual effects (`VFX/`).

## Communication

Enums act as **contract constants** across modules:
- `UpgradeType` is the key for `StatManager`'s stat dictionary and `ProcessFactory`'s switch table.
- `DamageSource` enables damage-source-specific logic in `BrickScript.ApplyDamageInternal`.
- `VFXType` maps commands to pooled VFX players in `VFXManager`.

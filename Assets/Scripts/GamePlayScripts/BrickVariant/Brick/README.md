# GamePlayScripts/BrickVariant/Brick/ — Concrete Brick Variants

## Module Summary

Concrete `IBrickVariant` implementations that define special brick behaviors when spawned, hit, or destroyed.

## Scripts

| Script | Type | Purpose |
|---|---|---|
| `IDamageRequestReceiver` | Interface | Marker for bricks that can receive cross-brick damage requests. |
| `SplitBrick` | `MonoBehaviour`, `IBrickVariant` | On death, spawns two new normal bricks at adjacent positions via `SpawnController`. |
| `x2Brick` | `MonoBehaviour`, `IBrickVariant` | Doubles the brick's initial health on spawn. |
| `HorizontalDamageBoost` | `MonoBehaviour`, `IBrickVariant` | On death, deals damage to all bricks in the same row via `BrickManager.RequestHorizontalDamage()`. Raises beam VFX. |
| `VerticalDamageBoost` | `MonoBehaviour`, `IBrickVariant` | On death, deals damage to all bricks in the same column via `BrickManager.RequestVerticalDamage()`. Raises beam VFX. |
| `PlusBallBoost` | `MonoBehaviour`, `IBrickVariant` | On death, grants extra balls via `BallManager.RequestExtraBall()`. |

## External Dependencies

| Dependency | Used By |
|---|---|
| `Manager/BrickManager` | `HorizontalDamageBoost`, `VerticalDamageBoost` request cross-row/column damage. |
| `Controller/SpawnController` | `SplitBrick` spawns new bricks at adjacent cells. |
| `Manager/BallManager` | `PlusBallBoost` adds extra balls. |
| `VFX/VFXEvent` | Beam variants raise `HorizontalBeamVFXCommand` / `VerticalBeamVFXCommand`. |

## Decoupling Notes

> **Heavy**: `HorizontalDamageBoost` and `VerticalDamageBoost` directly reference `BrickManager` (injected). Consider routing damage requests through `BrickScript.OnDie` events instead, letting `BrickManager` interpret the variant type.

> `SplitBrick` holds a reference to `SpawnController` (injected) — tighter coupling than needed. Could emit an event for the manager to handle.

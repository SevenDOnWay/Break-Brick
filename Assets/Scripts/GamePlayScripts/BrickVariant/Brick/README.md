# GamePlayScripts/BrickVariant/Brick/ — Concrete Brick Variants

## Module Summary

Concrete `IBrickVariant` implementations that define special brick behaviors when spawned, hit, or destroyed.

## Scripts

| Script | Type | Purpose |
|---|---|---|
| `IDamageRequestReceiver` | Interface | Marker for bricks that can receive cross-brick damage requests. |
| `IDamageBlocker` | Interface | Allows a variant to silently cancel an incoming `DamageRequest` before HP reduction. |
| `SplitBrick` | `MonoBehaviour`, `IBrickVariant` | On death, spawns two new normal bricks at adjacent positions via `SpawnController`. |
| `x2Brick` | `MonoBehaviour`, `IBrickVariant` | Doubles the brick's initial health on spawn. |
| `HorizontalDamageBoost` | `MonoBehaviour`, `IBrickVariant` | On death, deals damage to all bricks in the same row via `BrickManager.RequestHorizontalDamage()`. Raises beam VFX. |
| `VerticalDamageBoost` | `MonoBehaviour`, `IBrickVariant` | On death, deals damage to all bricks in the same column via `BrickManager.RequestVerticalDamage()`. Raises beam VFX. |
| `PlusBallBoost` | `MonoBehaviour`, `IBrickVariant` | On death, grants extra balls via `BallManager.RequestExtraBall()`. |
| `ShieldedBrick` | `MonoBehaviour`, `IBrickVariant`, `IDamageBlocker` | Blocks incoming hits from a configurable face. Uses a dot-product check on the contact normal in `DamageRequest.hitNormal`. Any hit from the shield side is silently cancelled; other directions deal damage normally. |
| `HealingBrick` | `MonoBehaviour`, `IBrickVariant` | Every `healCooldownTurns` turns, searches neighbours within `healRadius` and restores `healAmount` HP to each living brick. Driven by `OnEndTurn`. |
| `TntBrick` | `MonoBehaviour`, `IBrickVariant` | On death, queues radial damage (Chebyshev distance) to all surrounding bricks via `BrickManager.RequestRadialDamage()`. Chain-depth limits are fully respected. |
| `GhostBrick` | `MonoBehaviour`, `IBrickVariant` | Alternates between Solid and Ghost phases every `toggleIntervalTurns` turns. Ghost phase: `Collider2D` disabled, sprite alpha reduced. Solid phase: collidable, full alpha. Driven by `OnEndTurn`. |
| `FogBrick` | `MonoBehaviour`, `IBrickVariant` | Loads a smoke overlay prefab via Addressables on spawn, immediately places `initialSmokeCount` (default 3) child smoke objects. Adds +1 smoke every subsequent turn. Releases handle and destroys smokes on death. |

## External Dependencies

| Dependency | Used By |
|---|---|
| `Manager/BrickManager` | `HorizontalDamageBoost`, `VerticalDamageBoost`, `HealingBrick`, `TntBrick` request damage or read neighbour grid. |
| `Controller/SpawnController` | `SplitBrick` spawns new bricks at adjacent cells. |
| `Manager/BallManager` | `PlusBallBoost` adds extra balls. |
| `VFX/VFXEvent` | Beam variants raise `HorizontalBeamVFXCommand` / `VerticalBeamVFXCommand`. |
| `Addressables` | `FogBrick` loads the smoke prefab asynchronously. Must assign prefab GUID in Inspector. |

## Communication

- **`BrickScript` → `IBrickVariant`**: Lifecycle hooks (`OnSpawn`, `OnHit`, `OnEndTurn`, `OnDie`) are called directly by `BrickScript`.
- **`BrickScript.ApplyDamageInternal`** → **`IDamageBlocker`**: Before any HP change, each variant is asked `TryBlock(req)`. A `true` result cancels the entire hit.
- **`BrickManager.HandleBrickHit`**: Subscribes to `BrickScript.OnHit` (now carrying `Vector2 hitNormal`) and forwards it into `DamageRequest`.
- **Turn ticking**: `BrickManager.MoveBrick()` → `TickBrickEffects(brick)` now also calls `brick.CallVariantEndTurn()`, driving `HealingBrick` and `GhostBrick`.

## Decoupling Notes

> **Heavy**: `HorizontalDamageBoost` and `VerticalDamageBoost` directly reference `BrickManager` (injected). Consider routing damage requests through `BrickScript.OnDie` events instead, letting `BrickManager` interpret the variant type.

> `SplitBrick` holds a reference to `SpawnController` (injected) — tighter coupling than needed. Could emit an event for the manager to handle.

> `GhostBrick` calls `GetComponent<Collider2D>()` once at spawn time (acceptable — not a hot path). All other new variants respect the `[Inject]` + no runtime lookup rule.

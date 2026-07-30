# Testing/ — Experimental & Prototype Scripts

## Module Summary

**Non-production** scratch scripts used for prototyping VFX, UI, and physics experiments. These are not part of the gameplay pipeline and may be outdated.

## Scripts

| Script | Purpose |
|---|---|
| `TestLifeTimeScope` | Minimal VContainer scope for testing scenes. |
| `TestBeam` | Prototype for beam VFX triggering. |
| `TestExplosionVFX` | Prototype for explosion VFX triggering. |
| `Beam` | Beam rendering experiment using `LineRenderer`. |
| `ChangeColor` | Runtime color cycling test. |
| `Curve` | Empty stub. |
| `DontDestroyOnLoad` | Standard `DontDestroyOnLoad` marker. |
| `GameOfLife` | Conway's Game of Life grid simulation (unrelated to gameplay). |
| `LifeTesting` | Empty stub. |
| `ResponseUI` | Responsive UI scaling test. |
| `SpeedUp` | Ball speed manipulation test. |
| `UIPanel` | Minimal UI panel test. |

## Communication

These scripts have **no integration** with the production codebase. They exist solely for isolated experimentation.

> ⚠️ Consider moving these to a `Tests/` or `Sandbox/` assembly to prevent accidental coupling.

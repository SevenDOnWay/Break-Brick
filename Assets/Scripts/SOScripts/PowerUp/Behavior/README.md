# SOScripts/PowerUp/Behavior/ — Behavior-Based Upgrades

## Module Summary

`IBehavior` implementations for **non-stat, non-process upgrades** — these toggle mechanical features in `UpgradeManager` rather than modifying stats or adding on-hit effects.

## Scripts

| Script | Type | Purpose |
|---|---|---|
| `MagnetBehavior` | `ScriptableObject`, `IBehavior` | Activates the magnet feature by calling `UpgradeManager.SetMagnetActive(true)`. |

## Interface Contract

```csharp
public interface IBehavior {
    UpgradeBehaviourType Type { get; }
    void Apply(UpgradeManager upgradeManager);
}
```

## Class Interactions

### Internal

No inter-dependencies.

### External

| Dependency | Relationship |
|---|---|
| `Manager/UpgradeManager` | `Apply()` calls setter methods on the manager. |
| `SOScripts/PowerUp/UpgradeBehaviorSO` | Parent SO holds an array of `IBehavior` implementations. |

## Design Patterns

- **Strategy** — `IBehavior` implementations are interchangeable behavior strategies.

## Decoupling Notes

> Currently only `MagnetBehavior` exists. As more behaviors are added, consider a registry pattern to auto-discover `IBehavior` implementations.

---
trigger: always_on
---

# P2: Unity Engine & Performance

## Performance Standards

- **References:** Strictly prefer `[SerializeField] private` variables over expensive runtime lookups like `GameObject.Find()`, `GetComponent()`, or `FindObjectOfType()`.
- **Events:** For high-frequency game logic, use `System.Action` or `System.Delegate`. Avoid `UnityEvent` in the core game loop due to GC overhead.
- **Memory:** Use `public struct` for small, short-lived data containers instead of classes to reduce Heap allocation.

## Content Management

- **Addressables:** All non-scene assets must be managed via Addressables.
- **Grouping:** Group Addressables by system (e.g., `Characters_Player`, `UI_Global`) rather than individual files.

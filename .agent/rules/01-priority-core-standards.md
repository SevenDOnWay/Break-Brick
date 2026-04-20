---
trigger: always_on
---

# P1: Core Coding Standards

## Formatting (Sync with .editorconfig)

- **Braces:** Always use K&R style. Keep the open brace `{` on the same line as the statement.
- **Keywords:** Place `else`, `catch`, and `finally` on a new line after the closing brace.

## Naming Conventions

- **Public Identifiers:** Use `PascalCase` for all public classes, methods, properties, and events.
- **Static Fields:** Use `s_camelCase` for all static fields (e.g., `private static int s_playerCount`).
- **Local Variables:** Use `camelCase` for all local variables and parameters.
- **Clarity:** Prefer descriptive names over brevity. Avoid abbreviations except `UI`, `FX`, and `AI`.
- **No Prefixes:** Do not use Hungarian notation or type prefixes (e.g., avoid `strName` or `m_health`).

## Clean Code & Constant Management

- **Magic Numbers:** Strictly forbid raw numbers in logic (e.g., `if (health < 10)`). Use `const`, `static readonly`, or `[SerializeField]` variables with descriptive names instead.
- **Hardcoding Strings:** Never hardcode strings for Tags, Layers, Scenes, or Animator parameters.
  - _Requirement:_ Create a `static class` (e.g., `PlayerConstants`) or use `SerializedProperty` for these references.
- **Paths:** Do not hardcode file paths. Use `Path.Combine()` and dynamic references to ensure cross-platform compatibility.
- **Logic Isolation:** If a value is likely to change during balancing (e.g., `MoveSpeed`, `JumpForce`), it **must** be a `[SerializeField]` so it can be edited in the Inspector without recompiling code.

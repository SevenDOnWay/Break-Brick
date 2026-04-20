---
name: unity-breakbrick-standards
description: Enforces Break Brick Unity engineering standards across VContainer dependency injection, performance, data-driven ScriptableObject design, strategy-first planning, and modern C# style. Use when requests involve Unity C# scripts, managers/systems, gameplay architecture, refactors, optimization, ScriptableObjects, or cross-object communication.
---
# Unity Break Brick Standards

## Quick Start
When handling Unity code tasks in this repository:
1. Start with a brief "Think" plan before writing code.
2. Explain why each chosen pattern is used (not only what is changed).
3. For complex systems, ask for user confirmation before full implementation.
4. Apply all rule sections below while coding and reviewing.

## Rule: VContainer Dependency Injection
- **DI First:** Never use `GameObject.Find`, `FindObjectOfType`, or `GetComponent` for cross-object communication.
- **Injection:** Prefer `[Inject]` attributes on fields or constructor injection where applicable.
- **Registration:** When creating a new system or manager, remind the user to register it in the appropriate `LifetimeScope`.
- **Decoupling:** If a class needs a reference, suggest creating an interface first to keep the system modular and testable.

## Rule: Unity Performance Standards
- **Zero Allocations:** Avoid using LINQ in `Update()`, `FixedUpdate()`, or other frequently called loops to prevent GC pressure.
- **Caching:** Cache `WaitUntil`, `WaitForSeconds`, and repeated strings; use `nameof()` where possible.
- **Physics:** Ensure all physics-related logic lives in `FixedUpdate`.
- **Structs over Classes:** For small data containers (for example block data), suggest `readonly struct` to reduce heap allocation.

## Rule: Data-Driven Design
- **Config over Hardcoding:** If magic numbers appear (speed, health, damage), suggest moving them into `ScriptableObject` configuration.
- **Flyweight Pattern:** Use `ScriptableObject` assets to share immutable data across many balls/bricks and reduce memory overhead.
- **Initialization:** When creating a new `ScriptableObject` type, include `[CreateAssetMenu]`.

## Rule: Strategy First
- **The Think Phase:** Before writing code, provide a brief bulleted plan describing how the solution will be implemented.
- **Why, not just What:** Explain the reason for each major pattern choice (for example Observer, Strategy, Factory, Command).
- **Wait for Approval:** For complex systems, ask for user confirmation on the plan before generating the full script.

## Rule: Modern C# Standards
- **Syntactic Sugar:** Use expression-bodied members (`=>`) for simple methods and properties.
- **Null Safety:** Prefer null-conditional (`?.`) and null-coalescing (`??`) operators where they improve readability.
- **Primary Constructors:** If Unity/C# version support is confirmed, use primary constructors for cleaner class definitions.

## Enforcement Checklist
Before finalizing a response or code change:
- [ ] No cross-object lookups via scene search APIs.
- [ ] New systems/managers include a `LifetimeScope` registration reminder.
- [ ] Hot paths avoid LINQ and avoid avoidable allocations.
- [ ] Magic numbers are replaced with or proposed as `ScriptableObject` config.
- [ ] Plan-first behavior and pattern rationale are present.
- [ ] Modern C# syntax is used where it improves clarity.

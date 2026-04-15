---
trigger: model_decision
description: Active during file creation/refactoring
---

# P3: Architecture & Lifecycle

## Object Instantiation (MCP)

- **Dependency Injection:** Use the Model Context Protocol (MCP) to resolve and create objects.
- **Prohibition:** Do not use `new` or `GameObject.Instantiate()` for core gameplay systems or logic controllers.
- **Patterns:** Prioritize DI over the Singleton pattern to keep systems testable.

## Resource Management (Addressables)

- **Primary Loader:** Use the **Addressables System** for all dynamic asset loading (Prefabs, Textures, Audio, Data).
- **Prohibition:** Strictly forbid the use of `Resources.Load()` or direct public `GameObject` references for large assets that stay in memory.
- **Async Pattern:** All loading must be asynchronous. Use `Addressables.LoadAssetAsync<T>()` with `AsyncOperationHandle` or `await` tasks.
- **Lifecycle Safety:** Every loaded asset must have a corresponding `Addressables.Release()` call when the object is destroyed or the scene changes to prevent memory leaks.

## Structural Design

- **ScriptableObjects:** Use ScriptableObject-based architecture for game events and shared data (Game States, Settings).
- **Modularity:** Strictly separate concerns into `Core`, `Gameplay`, `UI`, and `Utilities`.
- **Assembly Isolation:** Create `.asmdef` files for every module. Strictly separate **Runtime** and **Editor** code into distinct assemblies to prevent build-time leakage.

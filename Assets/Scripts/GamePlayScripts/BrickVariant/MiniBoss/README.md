# Boss Brick

- **Role:** Durable boss enemy brick that appears as a scheduled pressure spike.
- **Spawn timing:** Spawned by `GameStateManager` through `SpawnController.SpawnBoss()` every 50 waves.
- **Core behavior:** `BossBrick` multiplies its starting HP on spawn, making it a priority target.
- **Rally behavior:** Every few turns, the boss heals nearby living bricks in a small radius.
- **Prefab:** `Assets/Prefabs/BrickVariant/Boss Brick.prefab` uses the standard `BrickScript` setup plus the `BossBrick` variant component.
- **Balancing knobs:** `healthMultiplier`, `rallyCooldownTurns`, `rallyHealAmount`, and `rallyRadius` are serialized on the prefab.

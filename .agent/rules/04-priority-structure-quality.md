---
trigger: always_on
description: Documentation and Automated Testing
---

# P4: Project Ops & Testings

## Folder Documentation

- **Mandatory Manifests:** Every folder MUST contain a `README.md` file.
- **Content:** The README must include:
  1. The folder's purpose.
  2. Description of each script/subfolder.
  3. A "Communication" section explaining how these scripts interact with other modules.

## Testing Standards

- **Organization:** Categorize tests by type: `Unit`, `Integration`, `Functional`, and `Performance`.
- **Isolation:** Place test code in separate assemblies. Use a `TestUtilities` assembly for shared mocks.
- **Coverage:** Mandatory tests are required for all critical gameplay systems (Combat, Inventory, AI, Database).

## Asset Organization

- **Type-Based:** Organize assets by type folder structure (e.g., `/Prefabs`, `/Textures`), never by file prefixes.

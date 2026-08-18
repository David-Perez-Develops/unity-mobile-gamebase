# Upgrade notes

This fork is derived from `Laputa-Unity/unity-mobile-gamebase` and intentionally diverges toward a genre-neutral Unity 6 Mobile Game Starter Kit.

## Removed

- Puzzle sample runtime classes (`Level`, `Pill`, `Hole`).
- Sample-specific `LevelController`.
- Core Observer events whose signatures depended on the sample `Level` type.

## Changed

- `GameManager` now owns application flow instead of loading numbered sample levels.
- Save loading now handles missing/corrupt primary saves and can recover from a backup.
- `PlayerData` has explicit save versioning and a neutral game-specific data extension point.
- The repository is adopting an explicit `StarterKit -> no Game dependencies` boundary incrementally to avoid a destructive bulk asset/GUID migration.

## Added

- `AUDIT.md` with the initial upstream audit and refactor decisions.
- `Assets/StarterKit/` architectural boundary.
- `Assets/Game/` home for concrete gameplay.
- This upgrade log.

## Upstream update policy

When evaluating a future upstream change:

1. inspect the upstream diff rather than merging blindly;
2. keep useful generic fixes/utilities;
3. do not reintroduce sample puzzle gameplay or Core-to-Game dependencies;
4. preserve this project's save compatibility and configuration contracts;
5. update this document when an upstream change is adopted or intentionally rejected.

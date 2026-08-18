# Upstream Audit — Unity Mobile Gamebase

This document records the initial audit performed before refactoring the fork into the Mobile Game Starter Kit.

## Baseline

- Upstream: `Laputa-Unity/unity-mobile-gamebase`
- Fork at audit time: `David-Perez-Develops/unity-mobile-gamebase`
- Default branch: `main`
- Unity editor recorded by the project: **6000.3.10f1** (`e35f0c77bd8e`).
- The upstream README still states Unity 6000.2.10f1, so `ProjectSettings/ProjectVersion.txt` is treated as the source of truth.

## Current architecture

The project is a small/mid-sized mobile-game base built primarily around persistent singleton controllers, ScriptableObject configuration, a static Observer/event layer, popup-driven UI and an encrypted JSON `PlayerData` save.

The existing separation is only partial:

- `Assets/_Project/Scripts/System/` contains most reusable infrastructure.
- `Assets/_Project/Scripts/Gameplay/Level/` contains sample puzzle gameplay (`Level`, `Pill`, `Hole`).
- `Assets/_Project/Resources/Levels/`, the level configuration/editor and several controllers/data fields are coupled to the sample level loop.
- `Assets/_Project/~ExtensionPackages/` contains reusable editor/runtime utilities.
- Native/plugins, Spine and third-party assets also exist and need individual keep/remove/license decisions before cleanup.

## Package dependencies

Direct package dependencies in `Packages/manifest.json` include:

### Git dependencies

- `com.coffee.softmask-for-ugui` — mob-sakai/SoftMaskForUGUI
- `com.coffee.ui-effect` — mob-sakai/UIEffect
- `com.coffee.ui-particle` — mob-sakai/ParticleEffectForUGUI
- `com.unity.ides` — dunward/com.unity.ides

### Unity packages of particular relevance

- Ads 4.16.4
- iOS Ads Support 1.2.0
- Analytics 3.8.2
- Input System 1.18.0
- Mobile Notifications 2.4.3
- Test Framework 1.6.0
- UGUI 2.0.0
- AI Navigation 2.0.10
- Device Simulator Devices 1.0.1
- Serialization 3.1.3

There are also standard Unity modules plus editor/IDE packages. Localization is **not** currently present despite being a target requirement.

Several currently installed packages (for example multiplayer center, navigation, ads/analytics and notifications) must not automatically be considered Starter Kit requirements. They will be retained only where they provide current value or are deliberately used by a later phase.

## Reusable infrastructure candidates

Strong candidates to retain and adapt rather than rewrite:

- singleton bootstrap/controller pattern;
- `PlayerData` + local JSON persistence/encryption;
- settings data and sound controller;
- popup base, popup controller/config and Popup Creator;
- Observer/event system;
- Safe Area and canvas scaling helpers;
- ScriptableObject configuration pattern;
- vibration/haptic helpers;
- generic resource/UI helpers;
- CustomTween;
- CustomInspector;
- CustomHierarchy;
- CustomFindReference;
- CustomBuildReport;
- CustomPlayerPref / PlayerPrefs tools;
- debug console where it remains independent of sample gameplay.

The singleton architecture will not be replaced merely for architectural purity. It matches the Starter Kit's small/medium mobile-game scope and the explicit preference for minimal change.

## Sample/game-specific candidates

Clear sample gameplay:

- `Assets/_Project/Scripts/Gameplay/Level/Level.cs`
- `Assets/_Project/Scripts/Gameplay/Level/Pill.cs`
- `Assets/_Project/Scripts/Gameplay/Level/Hole.cs`
- sample level prefabs/resources and puzzle-specific art/prefabs;
- the project-specific level editor.

Likely game-specific or requiring generalization:

- `LevelConfig` and level-loop semantics;
- level-index progression stored directly in `PlayerData`;
- `LevelController`;
- energy mechanics if they are tied to level attempts;
- hard-coded Gold/Diamond economy;
- skin-specific item/shop data;
- Lucky Spin if its implementation is product/game-specific;
- debug shortcuts tied to levels/skins.

## LevelController decision

`LevelController` is **not generic infrastructure in its current form**. It directly depends on `Level`, `LevelConfig`, `Data.PlayerData.CurrentLevelIndex`, a `Resources/Levels/Level N` naming convention and recycle/random level-loop behavior. Keeping it in StarterKit Core would make the Core depend on one style of gameplay/progression.

Decision: do not preserve this controller as Core merely because it is a global controller. During sample removal it should be removed or replaced by a neutral game-flow boundary only if the actual remaining flow requires one. The Starter Kit must never depend on `Game` gameplay classes.

## Existing useful behavior to preserve

The upstream documentation describes an encrypted JSON save at `Application.persistentDataPath/player_data.json`, automatic save on pause/quit, popup lifecycle hooks/animations, ScriptableObject configs, settings toggles, shop/items, daily rewards and multiple generic editor utilities. These should be verified against implementation before modification and reused where sound.

## Licensing / attribution finding

No root `LICENSE` file is present in the fork at audit time. The README contains a Third Party section, but the repository-level licensing/redistribution status and licenses/notices for bundled components must be treated as unresolved until each relevant source/component is checked. No upstream copyright/license notice will be removed during refactoring without that review.

This is important: absence of a root license is not permission to relicense upstream code. The Starter Kit documentation must preserve appropriate attribution and bundled third-party notices where required.

## Architectural target

The refactor will establish this dependency direction:

`Game -> StarterKit`

Never:

`StarterKit -> Game`

The target conceptual areas are Core, Data/Save, Settings, Audio, UI, Economy, Rewards, Localization, Monetization, Analytics, Platform and Developer Tools, with concrete gameplay isolated under `Assets/Game/`.

No minigame host/catalog/plugin architecture will be introduced.

## Initial refactor plan

1. Remove the puzzle/sample gameplay and its project-specific level editor/assets while preserving a bootable shell.
2. Separate reusable infrastructure from `Game` with minimal file movement first; avoid a broad rewrite.
3. Generalize save/settings/audio/UI before economy/rewards/shop.
4. Introduce a deliberately tiny placeholder game only after the reusable flow is stable.
5. Add Localization, configuration toggles, debug tooling and focused tests incrementally.
6. Validate in Unity at phase boundaries when an executable Unity environment is available; do not claim compile/play-mode validation from repository inspection alone.

## Audit caveats

- Repository/API inspection can establish source structure and dependencies, but it cannot substitute for opening the project in Unity and checking the Console/play mode/build pipeline.
- Asset licensing requires further component-level review as cleanup proceeds.
- The repository is large and contains binary Unity assets; deletion/movement will be done conservatively to preserve `.meta`/GUID relationships where appropriate.

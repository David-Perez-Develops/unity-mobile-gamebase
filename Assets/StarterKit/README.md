# StarterKit

Reusable mobile-game infrastructure belongs here.

Target areas:

- Core
- Data / Save
- Settings
- Audio
- UI
- Economy
- Rewards
- Localization
- Monetization
- Analytics
- Platform
- DeveloperTools

Dependency rule: code in this area must never depend on concrete code under `Assets/Game`.

The existing `_Project` infrastructure is being migrated incrementally rather than moved wholesale in one risky GUID-breaking refactor.

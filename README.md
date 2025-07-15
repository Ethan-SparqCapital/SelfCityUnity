# SelfCityUnity1

## Project Overview
SelfCityUnity1 is a Unity-based city-building game with a focus on personal growth, daily quests, and resource management. The project is structured for modularity and developer collaboration.

---

## Current Progress

### Core Features Implemented
- **City Building:**
  - Place, remove, and persistently save/load buildings and decorations on a city grid.
  - Buildings are managed via a `CityBuilder` system and are parented to a persistent container in the scene.
- **Inventory System:**
  - Drag-and-drop inventory for decorations and buildings.
  - Inventory filtering, rarity, and premium item support.
- **Resource Management:**
  - Player resources (Energy Crystals, Wisdom Orbs, Heart Tokens, Creativity Sparks, Balance Tickets) are saved/loaded and update the UI in real time.
- **Daily Quests:**
  - Daily quests are generated from a pool and reset every 24 hours.
  - Developer tool to force reset daily quests for testing.
- **UI/UX:**
  - Modern, modular UI using Unity UI and TextMeshPro.
  - Resource bar, inventory, quest panels, and modal dialogs.
- **Persistence:**
  - All major player data (city layout, inventory, resources, quest progress) is saved to PlayerPrefs and loaded on game start.

### Developer Utilities
- **Force Reset Daily Quests:**
  - In the Inspector, right-click the `DailyQuestsButtonHandler` component and select `Force Reset Daily Quests (Developer Only)` to instantly regenerate daily quests for testing.
- **Save/Load Debugger:**
  - The `SaveLoadDebugger` script provides context menu options to print, test, and debug save/load data for city and resources.
- **Manual Save/Load:**
  - Press `F5` to save, `F9` to load, and `F12` to clear all save data during play mode.

### Known Issues / TODO
- **Building Persistence:**
  - Placed buildings are saved/loaded, but visual restoration may require UI refresh or prefab adjustments.
- **Daily Quests UI:**
  - After force reset, the UI may need to be manually refreshed to reflect new quests.
- **Prefab Setup:**
  - Placed item prefabs should have both `SpriteRenderer` and `Image` components for hybrid UI/world support.
- **Advanced Features:**
  - Cloud save, user accounts, and advanced quest logic are planned but not yet implemented.

---

## How to Use Developer Tools
- **Force Reset Daily Quests:**
  - Select the GameObject with `DailyQuestsButtonHandler` in the Inspector.
  - Use the context menu to force reset quests.
- **Save/Load Debugger:**
  - Attach `SaveLoadDebugger` to any GameObject and use its context menu for save/load testing.

---

## Contributing
- Please see the in-code comments and the `README_AI_UI_Tools.md` for UI conventions and collaboration notes.

---

## Contact
For questions or collaboration, contact the project maintainer. 
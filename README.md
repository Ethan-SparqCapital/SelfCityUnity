# SelfCity Unity - City Building Game

A Unity-based city-building game that helps you build healthy real-life habits while constructing your dream city.

## 🎮 Game Overview
SelfCity is a 2D city-building game where players construct and manage their own city while also building healthy habits in real life. The quest system is inspired by real-world wellness, creativity, social, and health activities, encouraging players to take positive actions each day. The game features an intuitive UI system with drag-and-drop functionality, daily quests, a custom quest system, and a comprehensive building system.

## ✨ Features

* **🏗️ City Building System** – Place and manage various building types
* **🌆 City Region Zoom & Edit Mode** – Click a region (Health Harbor, Mind Palace, Social Square, Creative Commons) to smoothly zoom in. In edit mode, a grid overlay appears, and you can drag buildings from your inventory onto the grid. The inventory panel hides during drag and reappears after drop for a clean user experience.
* **📦 Inventory & Shop System**  
   * Decorations and buildings won from chests or purchased from the shop are stored in a persistent inventory  
   * Flexible shop UI for region-specific buildings and currencies  
   * **Drag-and-drop from inventory to city grid** – Placing an item removes it from inventory and places it on the grid. Clicking a placed item removes it from the grid and returns it to your inventory, fully supporting sorting and filtering.
* **📋 Quest & Habit System**  
   * **Static Region Quests**: Each region (Health Harbor, Mind Palace, Social Square, Creative Commons) has its own unique set of static quests, all inspired by healthy habits (physical, mental, social, and creative wellness).  
   * **Daily Quests**: A separate pool of daily quests is generated every 24 hours, with each quest tagged by its region and based on real-life healthy activities.  
   * **Custom Tasks**: Players can create their own custom quests, assign them to a region, and add them to their To-Do list via a dedicated modal window—perfect for tracking your own healthy habits.
* **💰 Resource Management**  
   * Completing quests rewards you with the correct region-based currency (crystals, hearts, magical, stars, etc.).  
   * **Dynamic Rewards:** The reward amount and icon are parsed directly from the quest string (e.g., "+5"), and are displayed and rewarded dynamically in all quest and To-Do list UIs.
* **🎯 Interactive UI**  
   * Modern, responsive user interface with modal windows for quests, custom tasks, and chest opening confirmations  
   * Confirmation modal prevents accidental spending of tickets when opening chests  
   * All quest lists and the To-Do list display both the reward amount and the correct resource icon for each quest.
* **📱 Drag & Drop** – Intuitive building placement system with inventory integration and grid snapping.
* **🎨 Visual Assets** – Rich collection of sprites and visual elements
* **🔧 Modular Architecture** – Well-structured codebase for easy expansion

## 🛠️ Technical Details

* **Engine:** Unity 2022.3 LTS (or newer)
* **Render Pipeline:** Universal Render Pipeline (URP)
* **Input System:** Unity's new Input System
* **UI Framework:** Unity UI with TextMesh Pro
* **Scripting:** C#

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Core/           # Game management and core systems
│   ├── Buildings/      # Building-related scripts
│   ├── Districts/      # District management
│   ├── Systems/        # Quest and unlock systems
│   └── UI/            # User interface scripts (including dynamic quest/To-Do UI)
├── Scenes/            # Unity scenes
├── Prefabs/           # Reusable game objects (including ToDoItemPrefab, QuestItem, etc.)
├── Sprites/           # Visual assets and icons
├── Materials/         # Materials and shaders
└── ScriptableObjects/ # Game data and configurations
```

## 🚀 Getting Started

### Prerequisites

* Unity 2022.3 LTS or newer
* Universal Render Pipeline (URP) - will be auto-installed

### Installation

1. **Clone or Download the Project**  
   * You can clone the repository or download and unzip the project folder.  
   * To zip your project for sharing, include only the following folders:  
         * `Assets/`  
         * `Packages/`  
         * `ProjectSettings/`  
         * (optionally) `UserSettings/`  
   * Do **not** include `Library/`, `Temp/`, or `Logs/` folders.
2. **Open in Unity Hub**  
   * Launch Unity Hub  
   * Click "Open" and select the project folder  
   * Wait for Unity to import and install packages
3. **Open the main scene**  
   * Navigate to `Assets/Scenes/SampleScene.unity`  
   * Press Play to start the game

## 🎯 How to Play

1. **Build Your City** – Use the building menu to place structures and grow your city.
2. **Zoom and Edit Regions** – Click a region to zoom in. Enter edit mode to see a grid overlay and drag items from your inventory onto the grid. The inventory panel hides while dragging and reappears after drop.
3. **Complete Quests**  
   * **Region Quests**: Click a region button to view and add static quests to your To-Do list. Each quest is inspired by a real-life healthy habit (e.g., exercise, mindfulness, social connection, creativity).  
   * **Daily Quests**: Open the Daily Quests modal to view a set of daily quests (refreshes every 24 hours), each encouraging a positive real-world action.  
   * **Custom Tasks**: Click "Add Custom Task" to open a modal where you can create your own healthy habits, assign them to a region, and add them to your To-Do list.
4. **Earn Rewards**  
   * Completing quests in the To-Do list gives you the correct region-based currency and amount, as shown in the UI.
5. **Open Chests with Confirmation**  
   * When opening a Decor Chest or Premium Decor Chest, a confirmation modal appears to prevent accidental spending of Balance Tickets. Confirm to open and receive a random reward, which is added to your inventory.
6. **Manage Your Inventory**  
   * All decorations and buildings you win or purchase are stored in your inventory. Use the inventory UI to view, filter, and drag items into your city. Placing an item removes it from inventory; clicking a placed item on the grid returns it to your inventory, supporting all filters and sorting.
7. **Expand Districts** – Unlock new areas as you progress

## 📝 Custom Tasks Feature

* Click the **Add Custom Task** button to open the custom quest modal.
* Enter your custom activity and select a region (Health Harbor, Mind Palace, Social Square, or Creative Commons).
* Your custom quest will appear in the modal's list, with an "Add To-Do" button next to it.
* You can add any custom quest to your To-Do list, and completing it will reward you with the correct region currency and amount.

## 🔄 Daily Quest Refresh Logic

* Daily quests are generated from a separate pool and are tagged by region.
* The list of daily quests automatically refreshes with new quests every 24 hours.
* The region label at the end of each quest ensures the correct reward is given.

## 🧩 Dynamic Rewards System

* The reward amount and icon for each quest are parsed from the quest string (e.g., "+5").
* Changing the reward in the quest string automatically updates the UI and the actual reward given to the player.
* **Anyone who downloads this repository and opens it in Unity will see the exact same UI, quest system, and dynamic rewards as the project author.**

## 🔧 Development

### Key Scripts

* `GameManager.cs` – Main game controller
* `CityBuilder.cs` – Building placement system
* `QuestManager.cs` – Quest system management (static, daily, and custom quests)
* `ResourceManager.cs` – Resource tracking
* `UIManager.cs` – User interface controller
* `CustomTaskModalHandler.cs` – Handles the custom quest modal logic
* `QuestItemUI.cs` – Handles quest item display and "Add To-Do" logic
* `ToDoListManager.cs` – Manages the To-Do list and quest completion
* `DecorChestManager.cs` – Handles chest opening logic and confirmation modal integration
* `RewardModal.cs` – Displays reward popups for chests and shop purchases
* `PurchaseConfirmModal.cs` – Displays confirmation modal before spending tickets or making purchases
* `InventoryManager.cs` – Manages player inventory for decorations and buildings
* `ShopBuildingItemUI.cs` – Handles shop item display and purchase logic

### Adding New Features

1. Create scripts in appropriate folders
2. Follow the existing naming conventions
3. Use ScriptableObjects for data-driven features
4. Test thoroughly before committing

## 📸 Screenshots

_Add screenshots of your game here_

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

* Unity Technologies for the game engine
* TextMesh Pro for advanced text rendering
* All contributors and testers

---

**Built with ❤️ using Unity** 

## DecorationNames.txt Structure (for Collaborating Developers)

The `Assets/Resources/DecorationNames.txt` file is used to mass-populate the CityBuilder's Building Types list via the CityBuilder auto-populate editor script. The file is structured as follows:

- **Lines 1–307:** Normal Decorations (for free AND premium players)
- **Lines 308–417:** Premium Decorations (for premium players only)
- **Lines 418–437:** Health Harbor Buildings
- **Lines 438–457:** Mind Palace Buildings
- **Lines 458–477:** Creative Commons Buildings
- **Lines 478–497:** Social Square Buildings

### How to Use
1. Select the CityBuilder GameObject in the Unity Hierarchy.
2. In the Inspector, use the "Populate from Name List (.txt)" button (provided by the CityBuilderAutoPopulateEditor script).
3. Select `DecorationNames.txt` when prompted.
4. The Building Types list will be filled in the above order.
5. Assign the correct prefab and sprite for each entry as needed.

**Note:**
- The order of items in the .txt file is important for distinguishing between normal decorations, premium decorations, and buildings by region.
- When adding new items, maintain this order for consistency.

---

# SelfCity Unity Game - Development Progress

## Project Overview
SelfCity is a Unity-based city-building game with persistent saving/loading capabilities, featuring a comprehensive inventory system, resource management, and quest mechanics.

## Core Features Implemented

### 🏗️ City Building System
- **Building Placement**: Drag-and-drop building placement with visual feedback
- **Grid-Based System**: Buildings snap to a grid system for organized city layout
- **Building Types**: Multiple building categories including:
  - Creative Commons Buildings
  - Health Harbor Buildings  
  - Mind Palace Buildings
  - Social Square Buildings
- **Building Removal**: Right-click to remove placed buildings
- **Visual States**: Buildings support both regular and damaged visual states

### 💾 Persistent Save/Load System
- **City Layout Persistence**: All placed buildings are saved and restored between sessions
- **Inventory Persistence**: Player inventory contents are maintained across game sessions
- **Resource Persistence**: Current resource amounts (money, materials, etc.) are saved
- **Quest Progress**: Daily quests and progress are saved and restored
- **Automatic Saving**: Game automatically saves after building placement/removal and resource changes

### 🎒 Inventory Management
- **Comprehensive Inventory**: Track building materials, decorations, and resources
- **Filtering System**: Filter inventory by category (All, Buildings, Decorations, Resources)
- **Visual Inventory**: Grid-based inventory display with item icons and counts
- **Drag-and-Drop**: Intuitive drag-and-drop interface for item management

### 🎯 Quest System
- **Daily Quests**: Rotating daily quests with rewards
- **Quest Types**: Various quest types including building placement, resource collection, and city development
- **Quest Timer**: Visual countdown timer for daily quest refresh
- **Quest Rewards**: Completion rewards including resources and experience
- **Developer Tools**: Force reset functionality for testing quest mechanics

### 🎨 User Interface
- **Modern UI Design**: Clean, modern interface with consistent theming
- **Resource Bar**: Real-time display of current resources (money, materials, etc.)
- **Building Shop**: Browse and purchase new buildings and decorations
- **Quest Panel**: Dedicated quest tracking and management interface
- **Responsive Design**: UI adapts to different screen sizes and resolutions

### 🎮 Input System
- **Unity Input System**: Modern input handling with action-based controls
- **Mouse Controls**: Left-click for selection, right-click for building removal
- **Keyboard Shortcuts**: Hotkeys for common actions
- **Touch Support**: Mobile-friendly touch controls

### 🏪 Shop System
- **Building Shop**: Purchase new buildings and decorations
- **Resource Costs**: Buildings require specific resources for purchase
- **Category Organization**: Shop items organized by building type
- **Purchase Validation**: Checks for sufficient resources before allowing purchases

### 🎨 Visual Assets
- **Isometric Art Style**: Consistent isometric visual design
- **Building Sprites**: 141+ building sprites with regular and damaged states

## 🆕 Recent Updates & New Features

### 🖼️ Enhanced Sprite System
- **Unified Sprite Management**: Consolidated sprite system where all sprites are stored in CityBuilder and referenced by Shop UI, eliminating duplication
- **Automatic Sprite Assignment**: Shop UI now pulls sprites directly from CityBuilder instead of separate BuildingShopDatabase
- **Consistent Sprite Display**: All UI elements (shop, inventory, modals) now use the same sprite source for consistency

### 🎯 Improved Purchase Confirmation System
- **Enhanced PurchaseConfirmModal**: Now displays item sprites in confirmation dialogs
- **Visual Purchase Confirmation**: Players can see the item they're about to purchase before confirming
- **Chest Confirmation**: Decor Chest and Premium Decor Chest buttons now show confirmation modals with chest sprites

### 🎨 Chest System Enhancements
- **Chest Button Sprites**: Decor Chest and Premium Decor Chest buttons now display their respective sprites
- **Faded Button Design**: Chest button sprites are automatically faded (30% opacity) for better text readability
- **Visual Chest Feedback**: Players can see which chest they're opening in both the button and confirmation modal

### 🏗️ Grid Placement Improvements
- **Automatic Grid Sizing**: Placed items automatically resize to match grid cell dimensions (100)
- **Perfect Grid Alignment**: Items placed from inventory now fit exactly within grid cells without overlap
- **Dynamic Sizing**: Grid cell size changes automatically adjust placed item sizes

### 🎨 UI Layout Optimizations
- **Shop Building Item Sizing**: Fixed oversized building icons in shop UI with proper layout controls
- **Grid Layout Management**: Improved grid cell positioning and sizing controls
- **Responsive UI Elements**: Better handling of UI element sizing and positioning

### 🎯 Prize Pool Display System
- **PrizePoolItemUI**: New system for displaying decoration items in prize pools with sprites
- **Visual Prize Pool**: Prize pool now shows decoration sprites alongside names
- **Consistent Sprite Display**: Prize pool items use the same sprite system as other UI elements

### 🔧 Technical Improvements
- **Code Consolidation**: Removed duplicate sprite fields from BuildingShopItem class
- **Improved Error Handling**: Better null checks and error logging throughout the system
- **Performance Optimizations**: More efficient sprite loading and UI updates
- **Maintainability**: Single source of truth for all sprite management

### 🎮 Enhanced User Experience
- **Visual Feedback**: All UI elements now provide clear visual feedback with appropriate sprites
- **Consistent Design**: Unified visual language across all game systems
- **Improved Readability**: Better contrast and sizing for all UI elements
- **Intuitive Interactions**: Clear visual cues for all user actions
- **Vehicle Sprites**: 167+ vehicle sprites for city atmosphere
- **Prop Sprites**: 38+ prop sprites for city decoration
- **Vegetation**: 11+ vegetation sprites for environmental detail
- **People Sprites**: 23+ character sprites for city population

## 🚀 Latest Major Updates (Since Last GitHub Push)

### 🎯 Level-Up UI Integration System (Phase 5D)
- **PlayerLevelManager**: Comprehensive player progression system with level-based building unlocks
- **EXPProgressBarManager**: Real-time EXP progress bar with smooth animations and visual feedback
- **EXPPopupManager**: Floating EXP number system with difficulty-based color coding
- **LevelUpManager**: Level-up celebrations and building unlock notifications
- **Dynamic Building Unlocks**: Buildings unlock based on player level and region unlock sequence
- **EXP Reward System**: Hybrid difficulty-based EXP calculation for quests and building placement
- **Visual Feedback**: Color-coded EXP popups (Green=Easy, Yellow=Medium, Orange=Hard, Red=Expert)
- **Building Unlock Notifications**: Real-time notifications when new buildings become available
- **Region Unlock System**: Automatic region unlocking when first building of a region becomes available

### 🤖 AI-Powered UI System
- **AIUIGenerator**: Revolutionary AI-driven UI generation system that creates dynamic, context-aware user interfaces
- **AIUIIntegrationNoDOTween**: Seamless integration of AI-generated UI elements without external dependencies
- **Smart UI Components**: AI system automatically generates appropriate UI elements based on game context and player actions
- **Dynamic Content Generation**: UI elements are generated on-the-fly based on current game state and player preferences

### 🎯 Enhanced Quest & Assessment System
- **AssessmentQuizManager**: Comprehensive assessment system for tracking player progress and habits
- **MyAssessmentQuizManager**: Personalized assessment tracking with detailed analytics
- **Advanced Quest Logic**: Improved quest generation and completion tracking
- **Progress Analytics**: Detailed tracking of player progress across all regions and activities
- **Hybrid EXP System**: Dynamic EXP calculation based on quest difficulty and type
- **Quest Difficulty Detection**: Automatic difficulty classification for all quest types
- **EXP Popup Animations**: Visual feedback for EXP gains with difficulty-based colors
- **Daily Quest Completion Rewards**: Balance Ticket rewards for completing all daily quests

### 🎨 Advanced Chest & Reward System
- **Enhanced Chest Mechanics**: Improved chest opening animations and reward distribution
- **Visual Reward Feedback**: Better visual feedback for all reward types
- **Chest Confirmation Modals**: Enhanced confirmation dialogs with visual previews
- **Reward Pool Management**: Improved management of reward pools and distribution

### 🏗️ Improved Building & District System
- **District Unlock System**: New system for unlocking different city districts as players progress
- **RegionUnlockSystem**: Comprehensive region unlocking mechanics with progress tracking
- **Enhanced Building Placement**: Improved grid-based building placement with better visual feedback
- **Building Categories**: Better organization of buildings by region and type
- **Level-Based Building Unlocks**: Buildings unlock progressively based on player level (1-40)
- **Region Unlock Sequence**: Dynamic region unlocking based on assessment results
- **Building Unlock Levels**: Each building has a specific unlock level calculated from region sequence
- **EXP Rewards for Building Placement**: Placing buildings rewards EXP based on their unlock level

### 🎮 Enhanced Game Management
- **CityMapZoomController**: Smooth zoom and pan controls for city navigation
- **Improved Camera Controls**: Better camera movement and zoom functionality
- **Enhanced Input Handling**: More responsive and intuitive input system
- **Performance Optimizations**: Improved game performance and loading times

### 🎨 UI/UX Improvements
- **Modern UI Design**: Updated UI with modern design principles and better user experience
- **Responsive Layout**: Better responsive design for different screen sizes
- **Visual Polish**: Enhanced visual effects and animations throughout the game
- **Accessibility Features**: Improved accessibility with better contrast and text sizing
- **EXP Progress Bar**: Real-time visual progress bar showing level progression
- **Level-Up Celebrations**: Animated level-up celebrations with fade effects
- **Unlock Notifications**: Floating notifications for building and region unlocks
- **Dynamic UI Updates**: All UI elements update automatically based on player progress

### 🔧 Technical Enhancements
- **Code Architecture**: Improved code organization and architecture for better maintainability
- **Error Handling**: Enhanced error handling and debugging capabilities
- **Performance Monitoring**: Better performance monitoring and optimization
- **Memory Management**: Improved memory usage and garbage collection
- **Event-Driven Architecture**: Level-up system uses events for loose coupling
- **Singleton Pattern**: Manager classes use singleton pattern for easy access
- **Debug Tools**: LevelUpDebugger script for testing level-up and unlock systems
- **Modular Design**: All level-up components are modular and extensible

### 📊 Data Management
- **Enhanced Save System**: Improved save/load system with better data integrity
- **Resource Management**: Better resource tracking and management
- **Inventory Optimization**: Improved inventory system with better performance
- **Data Persistence**: Enhanced data persistence across game sessions

### 🎨 Asset Management
- **Sprite Organization**: Better organization of sprite assets by category
- **Asset Optimization**: Optimized asset loading and management
- **Visual Consistency**: Improved visual consistency across all game elements
- **Asset Pipeline**: Enhanced asset pipeline for better development workflow

### 🧪 Development Tools
- **Editor Scripts**: New editor scripts for batch operations and asset management
- **BatchAssignPrefab**: Automated prefab assignment for building types
- **BatchAssignSprites**: Automated sprite assignment for UI elements
- **CityBuilderAutoPopulateEditor**: Automated population of building types from text files
- **DecorationlistImporter**: Automated import of decoration lists and assets
- **LevelUpDebugger**: Debug script for testing level-up and building unlock systems
- **Phase 5D Setup Instructions**: Comprehensive setup guide for level-up UI integration

### 📱 Mobile Optimization
- **Touch Controls**: Enhanced touch controls for mobile devices
- **Mobile UI**: Optimized UI for mobile screen sizes and interactions
- **Performance**: Improved performance on mobile devices
- **Responsive Design**: Better responsive design for various screen sizes

## Technical Implementation

### Architecture
- **ScriptableObject-Based**: Game data stored in ScriptableObjects for easy editing
- **Manager Pattern**: Centralized managers for different game systems
- **Event-Driven**: UI updates driven by game events
- **Modular Design**: Systems are loosely coupled for easy maintenance

### Key Scripts
- **CityBuilder**: Core city building and management logic
- **ResourceManager**: Handles resource tracking and persistence
- **InventoryManager**: Manages player inventory and item storage
- **QuestManager**: Handles quest generation, tracking, and rewards
- **UIManager**: Centralized UI management and updates
- **BuildingShopDatabase**: Shop system and item purchasing
- **PlayerLevelManager**: Player progression and level-based building unlocks
- **LevelUpManager**: Level-up celebrations and unlock notifications
- **EXPProgressBarManager**: EXP progress bar with real-time updates
- **EXPPopupManager**: Floating EXP number system with animations

### Data Persistence
- **JSON Serialization**: Save data stored in JSON format
- **File-Based Storage**: Save files stored locally on device
- **Automatic Backup**: Save data backed up to prevent data loss
- **Version Control**: Save data includes version information for compatibility

## Development Progress

### ✅ Completed Features
- [x] Core city building mechanics
- [x] Persistent save/load system
- [x] Inventory management with filtering
- [x] Resource management and tracking
- [x] Daily quest system with rewards
- [x] Building shop and purchasing
- [x] Modern UI with responsive design
- [x] Input system migration to Unity Input System
- [x] Visual asset integration (500+ sprites)
- [x] Developer tools for testing
- [x] AI-powered UI generation system
- [x] Enhanced quest and assessment system
- [x] Advanced chest and reward mechanics
- [x] District unlock system
- [x] Improved building placement
- [x] Enhanced camera controls
- [x] Mobile optimization
- [x] Performance improvements
- [x] Level-up UI integration system (Phase 5D)
- [x] Player progression with EXP system
- [x] Dynamic building unlocks based on level
- [x] Real-time EXP progress bar
- [x] Floating EXP popup animations
- [x] Level-up celebrations and notifications
- [x] Hybrid difficulty-based EXP calculation
- [x] Building unlock notifications
- [x] Region unlock system integration

### 🔄 Recent Improvements
- **Save/Load Reliability**: Fixed issues with buildings disappearing after reload
- **Resource Persistence**: Resolved resource reset issues during loading
- **Quest System**: Added developer reset functionality for testing
- **UI Synchronization**: Improved UI updates after data changes
- **Building Prefabs**: Enhanced prefab management and sprite assignment
- **AI UI Integration**: Seamless integration of AI-generated UI elements
- **Assessment System**: Comprehensive assessment and progress tracking
- **Chest System**: Enhanced chest mechanics and reward distribution
- **District System**: New district unlocking and management system
- **Performance**: Significant performance improvements across all systems
- **Level-Up System**: Complete player progression system with EXP and building unlocks
- **EXP Progress Bar**: Real-time visual feedback for player progression
- **Building Unlock Logic**: Dynamic building unlocks based on player level and region sequence
- **EXP Popup System**: Visual feedback for all EXP gains with difficulty-based colors
- **Notification System**: Real-time notifications for building and region unlocks

### 🚧 Current Development Focus
- **Performance Optimization**: Improving save/load performance
- **UI Polish**: Enhancing visual feedback and animations
- **Content Expansion**: Adding more building types and quests
- **Testing**: Comprehensive testing of all systems
- **AI System Enhancement**: Further development of AI-powered features
- **Mobile Optimization**: Continued mobile platform optimization
- **User Experience**: Ongoing UX improvements and refinements
- **Level-Up System Polish**: Fine-tuning EXP calculations and unlock timing
- **Notification System**: Enhancing visual feedback for unlocks and achievements
- **Progression Balance**: Balancing EXP rewards and building unlock progression

## Getting Started

### Prerequisites
- Unity 2022.3 LTS or later
- Universal Render Pipeline (URP)
- TextMesh Pro

### Installation
1. Clone the repository
2. Open the project in Unity
3. Open the SampleScene in Assets/Scenes/
4. Press Play to start the game

### Controls
- **Left Click**: Select and place buildings
- **Right Click**: Remove buildings
- **Mouse Wheel**: Zoom in/out of city view
- **WASD**: Pan camera around city

## Project Structure
```
Assets/
├── Scripts/           # C# scripts organized by system
├── Prefabs/          # Game object prefabs
├── Sprites/          # Visual assets (500+ sprites)
├── ScriptableObjects/ # Game data and configurations
├── Resources/        # Runtime-loaded assets
└── Scenes/          # Game scenes
```

## Contributing
This is a personal development project. For questions or suggestions, please refer to the development notes and documentation within the codebase.

## License
This project uses various asset packs with their respective licenses. Please refer to individual license files in the Sprites directory for specific licensing information.

---

*Last Updated: July 2025*
*Developed by:* 
*Development Status: Active Development - Core Systems Complete with Level-Up Integration* 
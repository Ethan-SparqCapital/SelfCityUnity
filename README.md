# SelfCity Unity - City Building Game

A Unity-based city-building game that helps you build healthy real-life habits while constructing your dream city.

## 🎮 Game Overview
SelfCity is a 2D city-building game where players construct and manage their own city while building healthy habits in real life. The quest system encourages real-world wellness activities, with quests inspired by physical, mental, social, and creative wellness.

## ✨ Core Features

### 🏗️ City Building & Management
- **Grid-Based Building System**: Place buildings and decorations on a grid with drag-and-drop functionality
- **Region-Based City**: Four distinct regions (Health Harbor, Mind Palace, Social Square, Creative Commons)
- **Building Categories**: 80+ buildings across different regions with unlock progression
- **Visual States**: Buildings support regular and damaged visual states
- **Construction Timer System**: Buildings require 1-6 hours of construction time before becoming functional

### 🎯 Quest & Habit System
- **Static Region Quests**: Each region has unique quests inspired by healthy habits
- **Daily Quests**: Rotating daily quests that refresh every 24 hours
- **Custom Tasks**: Players can create their own healthy habit quests
- **Skip Quest System**: Complete quests to instantly finish building construction
- **Quest Difficulty**: Expert, Hard, Medium, Easy quests based on construction time remaining

### 💰 Resource & Progression System
- **Region-Based Currencies**: Energy Crystals, Wisdom Orbs, Heart Tokens, Creativity Sparks
- **Dynamic Rewards**: Reward amounts and icons parsed from quest strings
- **Level-Up System**: Player progression with EXP-based leveling (1-40 levels)
- **Building Unlocks**: Buildings unlock progressively based on player level and assessment results
- **EXP Rewards**: Building placement and quest completion reward EXP with difficulty-based calculations

### 🎒 Inventory & Shop System
- **Persistent Inventory**: All decorations and buildings stored in persistent inventory
- **Drag-and-Drop**: Intuitive placement system with inventory integration
- **Shop System**: Purchase buildings and decorations with region-specific currencies
- **Visual Confirmations**: Purchase dialogs show building sprites before confirming

### 🔐 Authentication & Subscription
- **Multi-Platform Auth**: Google Sign-In, Apple Sign-In, Email/Password, Guest mode
- **Premium Features**: Exclusive content for premium subscribers
- **Profile Management**: Complete user profile system with data persistence
- **Journal System**: Personal journal with mood tracking and auto-save functionality

### 🎨 User Interface
- **Modern UI Design**: Clean, responsive interface with consistent theming
- **Region Zoom**: Click regions to zoom in with smooth camera transitions
- **Edit Mode**: Grid overlay for precise building placement
- **Visual Feedback**: EXP popups, level-up celebrations, unlock notifications
- **Mobile Optimization**: Touch-friendly controls and responsive design

## 🛠️ Technical Details

### Engine & Architecture
- **Unity 2022.3 LTS**: Universal Render Pipeline (URP)
- **C# Scripting**: Well-structured, modular codebase
- **Event-Driven Design**: Loose coupling between systems
- **Singleton Pattern**: Manager classes for easy access
- **ScriptableObject-Based**: Game data stored in ScriptableObjects

### Key Systems
- **CityBuilder**: Core building placement and management
- **ConstructionManager**: Construction timer and skip quest system
- **PlayerLevelManager**: Player progression and building unlocks
- **QuestManager**: Quest generation, tracking, and rewards
- **AuthenticationManager**: Multi-platform authentication
- **InventoryManager**: Inventory and item management
- **UIManager**: Centralized UI management

### Data Persistence
- **JSON Serialization**: Save data in JSON format
- **Local Storage**: Save files stored locally on device
- **Automatic Saving**: Game saves after key actions
- **Cross-Session Persistence**: All data maintained between sessions

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- Universal Render Pipeline (URP)
- Android Build Support (for mobile development)

### Installation
1. **Clone the Project**: Download and extract the project folder
2. **Open in Unity**: Launch Unity Hub and open the project
3. **Open Scene**: Navigate to `Assets/Scenes/SampleScene.unity`
4. **Press Play**: Start the game

### Authentication Setup
For full authentication functionality, see: `Assets/Scripts/UI/Authentication_Setup_Guide.md`

## 🎯 How to Play

1. **Build Your City**: Use the building menu to place structures
2. **Zoom and Edit**: Click regions to zoom in, enter edit mode for grid placement
3. **Complete Quests**: Add region quests, daily quests, or custom tasks to your To-Do list
4. **Earn Rewards**: Complete quests for region-based currencies and EXP
5. **Level Up**: Gain EXP to unlock new buildings and regions
6. **Manage Construction**: Wait for buildings to complete or complete skip quests to finish instantly

## 🆕 Latest Features

### 🏗️ Construction Timer & Skip Quest System
- **Time-Based Construction**: Buildings require 1-6 hours to complete
- **Skip Quest System**: Complete quests to instantly finish construction
- **Dynamic Difficulty**: Quest difficulty scales with remaining construction time
- **Region-Specific Quests**: Skip quests themed to building regions
- **Progress Persistence**: Construction progress maintained across sessions

### 🎯 Level-Up & Progression System
- **Player Progression**: EXP-based leveling system (1-40 levels)
- **Building Unlocks**: 80+ buildings unlock progressively based on level
- **Region Unlock System**: Regions unlock based on assessment results and level
- **Visual Feedback**: EXP popups, level-up celebrations, unlock notifications
- **Shop Integration**: Shops only show buildings unlocked at current level

### 🔐 Authentication & Profile System
- **Multi-Platform Auth**: Google, Apple, Email, Guest authentication
- **Premium Subscription**: Exclusive content and features for subscribers
- **Profile Management**: Complete user profile with data validation
- **Journal System**: Personal journal with mood tracking and book interface

## 📁 Project Structure
```
Assets/
├── Scripts/
│   ├── Core/           # Game management and core systems
│   ├── Buildings/      # Building-related scripts
│   ├── Systems/        # Quest, construction, authentication systems
│   ├── Shop/           # Shop and subscription management
│   └── UI/            # User interface scripts
├── Prefabs/           # Reusable game objects
├── Sprites/           # Visual assets (500+ sprites)
├── ScriptableObjects/ # Game data and configurations
└── Scenes/           # Game scenes
```

## 🎨 Visual Assets
- **Building Sprites**: 141+ buildings with regular and damaged states
- **Ground & Roads**: 109+ ground and road sprites
- **Vehicles**: 167+ vehicle sprites for city atmosphere
- **Props & Vegetation**: 49+ prop and vegetation sprites
- **People**: 23+ character sprites for city population
- **Icons**: 24+ icon sprites for UI elements

## 🔧 Development Tools
- **BatchAssignPrefab**: Automated prefab assignment for building types
- **BatchAssignSprites**: Automated sprite assignment for UI elements
- **CityBuilderAutoPopulateEditor**: Automated building type population
- **LevelUpDebugger**: Debug script for testing level-up systems
- **ConstructionManager**: Debug tools for testing construction system

## 📊 Development Status

### ✅ Completed Features
- [x] Core city building mechanics with grid system
- [x] Persistent save/load system
- [x] Inventory management with drag-and-drop
- [x] Quest system (static, daily, custom, skip quests)
- [x] Construction timer and skip quest system
- [x] Level-up and progression system
- [x] Multi-platform authentication
- [x] Premium subscription system
- [x] Profile and journal system
- [x] Shop system with visual confirmations
- [x] Region unlock system
- [x] EXP and reward system
- [x] Mobile optimization

### 🔄 Current Development
- **Action Menu System**: Building interaction menu (in progress)
- **UI Integration**: Advanced UI features and polish
- **Performance Optimization**: Ongoing system improvements
- **Cross-Platform Testing**: iOS setup and testing

## 🤝 Contributing
This is a personal development project. For questions or suggestions, please refer to the development notes within the codebase.

## 📄 License
This project uses various asset packs with their respective licenses. Please refer to individual license files in the Sprites directory.

---

**Built with ❤️ using Unity**

*Last Updated: January 2025*
*Developed By: Ethan Le*
*Development Status: Active Development - Core Systems Complete* 
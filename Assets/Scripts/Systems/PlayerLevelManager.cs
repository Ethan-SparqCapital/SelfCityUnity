using System.Collections.Generic;
using UnityEngine;
using LifeCraft.Core;
using LifeCraft.Systems;
using LifeCraft.Shop; // Import the Shop namespace to access BuildingShopItem and BuildingShopDatabase. 

namespace LifeCraft.Systems
{
    /// <summary>
    /// Manages player leveling, EXP, and building unlocks based on level and region unlock sequence.
    /// </summary>
    public class PlayerLevelManager : MonoBehaviour
    {
        [Header("Level Configuration")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentEXP = 0;
        
        [Header("EXP Configuration")]
        [SerializeField] private int baseEXPPerLevel = 100;
        [SerializeField] private float expMultiplier = 1.5f; // Exponential progression
        
        // Events
        public System.Action<int> OnLevelUp;
        public System.Action<int> OnEXPChanged;
        public System.Action<string> OnBuildingUnlocked;
        public System.Action<AssessmentQuizManager.RegionType> OnRegionUnlocked;
        
        // Building unlock data
        private Dictionary<string, int> _buildingUnlockLevels = new Dictionary<string, int>();
        // Dictionary to hold region type and each of their corresponding buildings:
        private Dictionary<AssessmentQuizManager.RegionType, List<string>> _regionBuildings = new Dictionary<AssessmentQuizManager.RegionType, List<string>>();
        private List<string> _allBuildingsInOrder = new List<string>();
        
        // Singleton
        private static PlayerLevelManager _instance;
        public static PlayerLevelManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<PlayerLevelManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("PlayerLevelManager");
                        _instance = go.AddComponent<PlayerLevelManager>();
                    }
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeBuildingUnlockSystem();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initialize the building unlock system based on region unlock sequence
        /// </summary>
        private void InitializeBuildingUnlockSystem()
        {
            // (1) Get the player's region unlock sequence from AssessmentQuizManager
            // (2) Sort all buildings from all regions based on region unlock sequence and excitement level
            // (3) Assign unlock levels to buildings (1-40)

            // 1. Get the player's region unlock sequence:
            var regionUnlockSystem = RegionUnlockSystem.Instance; // Get the instance of RegionUnlockSystem to get the correct unlock sequence. 
            var unlockOrder = regionUnlockSystem.GetUnlockOrder(); // Call the "GetUnlockOrder" method from the RegionUnlockSystem file to get the correct unlock order of regions. 

            if (regionUnlockSystem == null)
            {
                Debug.LogError("RegionUnlockSystem.Instance is null!");
                return; // Exit if the region unlock system is not initialized. 
            }

            if (unlockOrder == null || unlockOrder.Count == 0)
            {
                Debug.LogError("No unlock order found in RegionUnlockSystem!");
                return; // Exit if the unlock order is null or empty. 
            }
            
            // 2. Sort buildings based on region unlock sequence and excitement level:
                var sortedBuildings = SortBuildingsByUnlockSequence(unlockOrder); // Parameters are the unlock order, and the list of all buildings. 

            // 3. Assign unlock levels to buildings (1-40):
            AssignUnlockLevelsToBuildings(sortedBuildings); // Assign levels to buildings based on their position in the sorted list.  
            
            Debug.Log("Building unlock system initialized");
        }

        // Helper Function (2): Sort buildings based on region unlock sequence and excitement level
        private List<BuildingShopItem> SortBuildingsByUnlockSequence(List<AssessmentQuizManager.RegionType> unlockOrder)
        {
            // Sort the buildings based on their region unlock sequence and excitement level (excitement level is already defined in each BuildingShopDatabase; least exciting to most exciting). 
            var sortedBuildings = new List<BuildingShopItem>(); // Create a new list to hold the sorted buildings.

            // First, put the names of the buildings for each region into 4 separate lists. (Use Resources.Load to load the building databases; it is the standard way to load ScriptableObjects in Unity.)
            var healthHarborBuildings = Resources.Load<BuildingShopDatabase>("HealthHarborBuildings"); // Load the Health Harbor buildings database (an existing ScriptableObject of type BuildingShopDatabase in Assets/Resources/ folder on Unity Editor), which is HealthHarborBuildings.asset.
            var mindPalaceBuildings = Resources.Load<BuildingShopDatabase>("MindPalaceBuildings"); // Load the Mind Palace buildings database (an existing ScriptableObject of type BuildingShopDatabase in Assets/Resources/ folder on Unity Editor), which is MindPalaceBuildings.asset. 
            var creativeCommonsBuildings = Resources.Load<BuildingShopDatabase>("CreativeCommonsBuildings"); // Load the Creative Commons buildings database (an existing ScriptableObject of type BuildingShopDatabase in Assets/Resources/ folder on Unity Editor), which is CreativeCommonsBuildings.asset. 
            var socialSquareBuildings = Resources.Load<BuildingShopDatabase>("SocialSquareBuildings"); // Load the Social Square buildings database (an existing ScriptableObject of type BuildingShopDatabase in Assets/Resources/ folder on Unity Editor), which is SocialSquareBuildings.asset. 

            foreach (var region in unlockOrder) // For each region in the unlock order (already determined), 
            {
                // Get the buildings for this region:

                if (region == AssessmentQuizManager.RegionType.HealthHarbor && healthHarborBuildings != null) // If the current region in the loop of unlockOrder is Health Harbor AND the Health Harbor buildings database exists, 
                {
                    sortedBuildings.AddRange(healthHarborBuildings.buildings); // Add the Health Harbor buildings to the sorted master list. The HealthHarborBuildings.asset BuildingShopDatabase contains a list called "buildings" of Health Harbor BuildingShopItem objects, which are added to the sorted master list. 

                    // Track which buildings belong to Health Harbor:
                    if (!_regionBuildings.ContainsKey(region)) // If the region is not already in the dictionary, 
                    {
                        _regionBuildings[region] = new List<string>(); // Create a new list for this region. 
                    }

                    foreach (var building in healthHarborBuildings.buildings) // For each building in the Health Harbor buildings database,
                    {
                        _regionBuildings[region].Add(building.name); // The buildings database holds BuildingShopItem objects, which contain a "name" field for the Building Name. Add the building name to the region's list in the dictionary. 
                    }
                }

                if (region == AssessmentQuizManager.RegionType.MindPalace && mindPalaceBuildings != null) // If the current region in the loop of unlockOrder is Mind Palace AND the Mind Palace buildings database exists, 
                {
                    sortedBuildings.AddRange(mindPalaceBuildings.buildings); // Add the Mind Palace buildings to the sorted master list. The MindPalaceBuildings.asset BuildingShopDatabase contains a list called "buildings" of BuildingShopItem objects, which are added to the sorted master list. 

                    // Track which buildings belong to Mind Palace:
                    if (!_regionBuildings.ContainsKey(region)) // If the region is not already in the dictionary, 
                    {
                        _regionBuildings[region] = new List<string>(); // Create a new list for this region.
                    }

                    foreach (var building in mindPalaceBuildings.buildings) // For each building in the Mind Palace buildings database, 
                    {
                        _regionBuildings[region].Add(building.name); // The buildings database holds BuildingShopItem objects, which contain a "name" field for the Building Name. Add the building name to the region's list in the dictionary.
                    }
                }

                if (region == AssessmentQuizManager.RegionType.CreativeCommons && creativeCommonsBuildings != null) // If the current region in the loop of unlockOrder is Creative Commons AND the Creative Commons buildings database exists, 
                {
                    sortedBuildings.AddRange(creativeCommonsBuildings.buildings); // Add the Creative Commons buildings to the sorted master list. The CreativeCommonsBuildings.asset BuildingShopDatabase contains a list called "buildings" of type BuildingShopItem objects, which are added to the sorted master list. 

                    // Track which buildings belong to Creative Commons:
                    if (!_regionBuildings.ContainsKey(region)) // If the region is not already in the dictionary, 
                    {
                        _regionBuildings[region] = new List<string>(); // Create a new list for this region. 
                    }

                    foreach (var building in creativeCommonsBuildings.buildings) // For each building in the Creative Commons buildings database, 
                    {
                        _regionBuildings[region].Add(building.name); // The buildings database holds BuildingShopItem objects, which contain a "name" field for the Building Name. Add the building name to the region's list in the dictionary. 
                    }
                }

                if (region == AssessmentQuizManager.RegionType.SocialSquare && socialSquareBuildings != null) // If the current region in the loop of unlockOrder is Social Square AND the Social Square buildings database exists, 
                {
                    sortedBuildings.AddRange(socialSquareBuildings.buildings); // Add the Social Square buildings to the sorted master list. The SocialSquareBuildings.asset BuildingShopDatabase contains a list called "buildings" of type BuildingShopItem objects, which are added to the sorted master list. 

                    // Track which buildings belong to Social Square:
                    if (!_regionBuildings.ContainsKey(region)) // If the region is not already in the dictionary, 
                    {
                        _regionBuildings[region] = new List<string>(); // Create a new list for this region. 
                    }

                    foreach (var building in socialSquareBuildings.buildings) // For each building in the Social Square buildings database, 
                    {
                        _regionBuildings[region].Add(building.name); // The buildings database holds BuildingShopItem objects, which contain a "name" field for the Building Name. Add the building name to the region's list in the dictionary.
                    }
                }
            }

            Debug.Log($"Sorted buildings by unlock sequence. Total sorted buildings: {sortedBuildings.Count}");
            return sortedBuildings; // Return the sorted list of buildings based on the unlock sequence. 
        }

        // Helper Function (3): Assign unlock levels to buildings (1-40)
        private void AssignUnlockLevelsToBuildings(List<BuildingShopItem> sortedBuildings)
        {
            int totalBuildings = sortedBuildings.Count; // Get the total number of buildings in the sorted list (currently 80). 
            int maxUnlockLevel = 40; // Maximum unlock level for buildings (1-40). 

            for (int i = 0; i < totalBuildings; i++) // For each building in the sorted list, 
            {
                // Formula: More buildings unlock early, fewer later:
                int unlockLevel = Mathf.RoundToInt(1 + (i * maxUnlockLevel) / (float)totalBuildings); // Calculate the unlock level based on the index of the building in the sorted list. 
                                                                                                      // Example: 
                                                                                                      // First building (i = 0): 1 + (0 * 40) / 80 = Level 1
                                                                                                      // Second building (i = 1): 1 + (1 * 40) / 80 = Level 1.5 (rounds to Level 2)
                                                                                                      // Third building (i = 2): 1 + (2 * 40) / 80 = Level 2 

                // Ensure the level is within bounds (1-40):
                unlockLevel = Mathf.Clamp(unlockLevel, 1, maxUnlockLevel); // Clamp the unlock level to be between 1 and 40 (clamp means to restrict a value to a specific range). 

                _buildingUnlockLevels[sortedBuildings[i].name] = unlockLevel; // Assign the calculated unlock level to the building's name in the dictionary (sortedBuildings is of type BuildingShopItem, which contains a "name" field for the Building Name). 

                Debug.Log($"Building '{sortedBuildings[i].name}' unlocks at level {unlockLevel}"); // Log the building name and its unlock level for debugging purposes. 
            }
        }
        
        /// <summary>
        /// Add EXP to the player
        /// </summary>
        public void AddEXP(int expAmount)
        {
            currentEXP += expAmount;
            OnEXPChanged?.Invoke(currentEXP);

            // Check for level up
            CheckForLevelUp();
        }
        
        /// <summary>
        /// Check if player should level up
        /// </summary>
        private void CheckForLevelUp()
        {
            int expRequiredForNextLevel = GetEXPRequiredForLevel(currentLevel + 1);
            
            while (currentEXP >= expRequiredForNextLevel)
            {
                currentEXP -= expRequiredForNextLevel;
                currentLevel++;
                
                OnLevelUp?.Invoke(currentLevel);
                
                // Check for building unlocks
                CheckForBuildingUnlocks();
                
                // Check for region unlocks
                CheckForRegionUnlocks();
                
                expRequiredForNextLevel = GetEXPRequiredForLevel(currentLevel + 1);
            }
        }
        
        /// <summary>
        /// Calculate EXP required for a specific level (exponential progression)
        /// </summary>
        public int GetEXPRequiredForLevel(int level)
        {
            // Implement exponential EXP calculation:
            // Formula: baseEXPPerLevel * (expMultiplier ^ (level - 1))

            // baseEXPPerLevel (EXP required for Level 1) = 100 EXP required to reach Level 2
            // expMultiplier = 1.5 (50% increase per level) 

            // Example: 
            // Level 1 to Level 2: 100 * (1.5 ^ (1 - 1)) = 100 * 1.5^0 = 100 * 1 = 100
            // Level 2 to Level 3: 100 * (1.5 ^ (2 - 1)) = 100 * 1.5^1 = 150 

            // Level 2 to Level 3 in Code Format: 
            // Mathf.RoundToInt(100 * Mathf.Pow(1.5f, 2 - 1)) = Mathf.RoundToInt(100 * 1.5^1) = 150 
            int expRequiredForNextLevel = Mathf.RoundToInt(baseEXPPerLevel * Mathf.Pow(expMultiplier, level - 1));
            return expRequiredForNextLevel; 
        }

        /// <summary>
        /// Check if any buildings should be unlocked at current level
        /// </summary>
        private void CheckForBuildingUnlocks()
        {
            // (1) Loop through all buildings in _BuildingUnlockLevels. 
            // (2) Check if any buildings unlock at the current level. 
            // (3) Trigger OnBuildingUnlocked event for each newly unlocked building. 

            foreach (var building in _buildingUnlockLevels) // _buildingUnlockLevels is a dictionary (or a HashMap) where the Key is the building name (string) and the Value is the unlock level (int). 
            { // For every building in the _buildingUnlockLevels dictionary, 
                if (building.Value == currentLevel) // If the building's unlock level (Value = int = unlock level) matches the current player level,
                {
                    OnBuildingUnlocked?.Invoke(building.Key); // Trigger the OnBuildingUnlocked event with the building name (Key = string = building name). 
                    Debug.Log($"Building '{building.Key}' unlocked at level {currentLevel}"); // Log the building name and current level for debugging purposes. 
                }
            }
        }

        /// <summary>
        /// Check if any regions should be unlocked at current level
        /// </summary>
        private void CheckForRegionUnlocks()
        {
            // (1) Get the player's region unlock sequence from RegionUnlockSystem instance.
            // (2) Check each region to see if its first building unlocks at current level. 
            // (3) If yes, unlock the region and trigger OnRegionUnlocked event. 

            var regionUnlockSystem = RegionUnlockSystem.Instance; // Get the instance of RegionUnlockSystem to get the correct unlock sequence. 

            if (regionUnlockSystem == null)
            {
                return; // Exit if the region unlock system is not initialized. 
            }

            foreach (var region in regionUnlockSystem.GetUnlockOrder()) // For each region in the player's region unlock order,
            {
                // Skip if region is already unlocked:
                if (regionUnlockSystem.IsRegionUnlocked(region)) continue; // If the region is already unlocked, skip to the next region in the loop. 

                // Otherwise, get the first building for this region (at index 0):
                if (_regionBuildings.ContainsKey(region) && _regionBuildings[region].Count > 0) // If the region exists in the dictionary and has at least one building, 
                {
                    string firstBuilding = _regionBuildings[region][0]; // Get the first building name for this region at index 0. 

                    // Check if this first building unlocks at the current player level:
                    if (_buildingUnlockLevels.ContainsKey(firstBuilding) && _buildingUnlockLevels[firstBuilding] == currentLevel) // If the first building exists in the _buildingUnlockLevels dictionary and its unlock level matches the current player level, 
                    {
                        // Unlock the region:
                        regionUnlockSystem.ForceUnlockRegion(region); // Call the ForceUnlockRegion method from the RegionUnlockSystem to unlock the region. 
                        OnRegionUnlocked?.Invoke(region); // Trigger the OnRegionUnlocked event with the region type in order to notify other systems that the region has been unlocked. 
                        Debug.Log($"Region {region} unlocked at level {currentLevel}!"); // Log the region name and current level for debugging purposes. 
                    }
                }
            }
        }
        
        /// <summary>
        /// Get the unlock level for a specific building
        /// </summary>
        public int GetBuildingUnlockLevel(string buildingName)
        {
            if (_buildingUnlockLevels.ContainsKey(buildingName))
            {
                return _buildingUnlockLevels[buildingName];
            }
            return -1; // Building not found
        }
        
        /// <summary>
        /// Check if a building is unlocked at current level
        /// </summary>
        public bool IsBuildingUnlocked(string buildingName)
        {
            int unlockLevel = GetBuildingUnlockLevel(buildingName);
            return unlockLevel > 0 && currentLevel >= unlockLevel;
        }
        
        /// <summary>
        /// Get current level
        /// </summary>
        public int GetCurrentLevel()
        {
            return currentLevel;
        }
        
        /// <summary>
        /// Get current EXP
        /// </summary>
        public int GetCurrentEXP()
        {
            return currentEXP;
        }
        
        /// <summary>
        /// Get EXP required for next level
        /// </summary>
        public int GetEXPRequiredForNextLevel()
        {
            return GetEXPRequiredForLevel(currentLevel + 1);
        }
        
        /// <summary>
        /// Get all unlocked buildings at current level
        /// </summary>
        public List<string> GetUnlockedBuildings()
        {
            List<string> unlockedBuildings = new List<string>();
            
            foreach (var building in _buildingUnlockLevels)
            {
                if (currentLevel >= building.Value)
                {
                    unlockedBuildings.Add(building.Key);
                }
            }
            
            return unlockedBuildings;
        }
    }
} 
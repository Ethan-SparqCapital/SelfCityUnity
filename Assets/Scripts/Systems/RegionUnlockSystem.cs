using System.Collections.Generic;
using UnityEngine;
using LifeCraft.Core;

namespace LifeCraft.Systems
{
    /// <summary>
    /// Manages region unlocking based on building placement progress.
    /// Extends the base UnlockSystem with region-specific functionality.
    /// </summary>
    [CreateAssetMenu(fileName = "RegionUnlockSystem", menuName = "LifeCraft/Region Unlock System")]
    public class RegionUnlockSystem : ScriptableObject
    {
        // Singleton instance
        private static RegionUnlockSystem _instance;
        public static RegionUnlockSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<RegionUnlockSystem>("RegionUnlockSystem");
                    if (_instance == null)
                    {
                        Debug.LogError("RegionUnlockSystem not found in Resources folder!");
                    }
                }
                return _instance;
            }
        }

        [System.Serializable]
        public class RegionUnlockData
        {
            public AssessmentQuizManager.RegionType regionType;
            public string regionName;
            public bool isUnlocked = false;
            public int buildingsRequiredToUnlock = 3; // Number of buildings needed to unlock next region
            public int currentBuildingCount = 0;
            public Sprite regionIcon;
            public Color regionColor = Color.white;
        }

        [Header("Region Configuration")]
        [SerializeField] private List<RegionUnlockData> regions = new List<RegionUnlockData>();
        
        [Header("Unlock Requirements")]
        [SerializeField] private int defaultBuildingsRequired = 3;

        // Events
        public System.Action<AssessmentQuizManager.RegionType> OnRegionUnlocked;
        public System.Action<AssessmentQuizManager.RegionType, int> OnBuildingCountChanged;

        // Current state
        private AssessmentQuizManager.RegionType _startingRegion = AssessmentQuizManager.RegionType.HealthHarbor;
        private Dictionary<AssessmentQuizManager.RegionType, RegionUnlockData> _regionData = new Dictionary<AssessmentQuizManager.RegionType, RegionUnlockData>();
        private List<AssessmentQuizManager.RegionType> _unlockOrder = new List<AssessmentQuizManager.RegionType>();
        private int _currentUnlockIndex = 0;

        private void Awake()
        {
            InitializeRegions();
        }

        /// <summary>
        /// Ensure regions are initialized (called from public methods)
        /// </summary>
        private void EnsureInitialized()
        {
            if (_regionData.Count == 0)
            {
                Debug.Log("RegionUnlockSystem not initialized, initializing now...");
                InitializeRegions();
            }
        }

        /// <summary>
        /// Initialize the region unlock system
        /// </summary>
        private void InitializeRegions()
        {
            Debug.Log("Initializing RegionUnlockSystem...");
            _regionData.Clear();

            // Create default regions if none are configured
            if (regions.Count == 0)
            {
                regions = new List<RegionUnlockData>
                {
                    new RegionUnlockData 
                    { 
                        regionType = AssessmentQuizManager.RegionType.HealthHarbor,
                        regionName = "Health Harbor",
                        isUnlocked = false,
                        buildingsRequiredToUnlock = defaultBuildingsRequired,
                        regionColor = new Color(0.49f, 0.85f, 0.34f, 1f)
                    },
                    new RegionUnlockData 
                    { 
                        regionType = AssessmentQuizManager.RegionType.MindPalace,
                        regionName = "Mind Palace",
                        isUnlocked = false,
                        buildingsRequiredToUnlock = defaultBuildingsRequired,
                        regionColor = new Color(0.7f, 0.62f, 0.86f, 1f)
                    },
                    new RegionUnlockData 
                    { 
                        regionType = AssessmentQuizManager.RegionType.CreativeCommons,
                        regionName = "Creative Commons",
                        isUnlocked = false,
                        buildingsRequiredToUnlock = defaultBuildingsRequired,
                        regionColor = new Color(1f, 0.88f, 0.4f, 1f)
                    },
                    new RegionUnlockData 
                    { 
                        regionType = AssessmentQuizManager.RegionType.SocialSquare,
                        regionName = "Social Square",
                        isUnlocked = false,
                        buildingsRequiredToUnlock = defaultBuildingsRequired,
                        regionColor = new Color(1f, 0.7f, 0.28f, 1f)
                    }
                };
            }

            // Build lookup dictionary
            foreach (var region in regions)
            {
                _regionData[region.regionType] = region;
                Debug.Log($"Added region: {region.regionName} ({region.regionType})");
            }
            
            Debug.Log($"RegionUnlockSystem initialized with {_regionData.Count} regions");
        }

        /// <summary>
        /// Set the starting region and unlock order (from quiz results)
        /// </summary>
        public void SetStartingRegion(AssessmentQuizManager.RegionType region, Dictionary<AssessmentQuizManager.RegionType, int> quizScores = null) // The GameManager calls this method to set the starting region and unlock order based on quiz scores. 
        {
            Debug.Log($"=== SET STARTING REGION CALLED ===");
            Debug.Log($"Region parameter: {region}");
            Debug.Log($"Quiz scores: {(quizScores != null ? "provided" : "null")}");
            
            EnsureInitialized();
            
            Debug.Log($"SetStartingRegion called with region: {region}");
            Debug.Log($"Current _regionData count: {_regionData.Count}");
            
            _startingRegion = region;
            
            // Set up unlock order based on quiz scores
            if (quizScores != null)
            {
                Debug.Log("Setting unlock order from quiz scores...");
                SetUnlockOrderFromQuizScores(quizScores);
            }
            else
            {
                Debug.Log("Setting default unlock order...");
                // Default unlock order if no quiz scores provided
                SetDefaultUnlockOrder(region);
            }
            
            // Unlock only the starting region
            Debug.Log("Setting unlock flags for all regions...");
            foreach (var kvp in _regionData)
            {
                bool shouldUnlock = (kvp.Key == region);
                Debug.Log($"Processing region {kvp.Key}: shouldUnlock = {shouldUnlock}");
                
                kvp.Value.isUnlocked = shouldUnlock;
                kvp.Value.currentBuildingCount = 0;
                
                Debug.Log($"Region {kvp.Key} ({kvp.Value.regionName}): isUnlocked = {shouldUnlock} (kvp.Key == region: {kvp.Key == region})");
            }

            _currentUnlockIndex = 0; // Start with the first region in unlock order

            Debug.Log($"Starting region set to: {AssessmentQuizManager.GetRegionDisplayName(region)}");
            Debug.Log($"Unlock order: {string.Join(" -> ", _unlockOrder.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
            
            // Force unlock the starting region as a test
            Debug.Log("Calling ForceUnlockRegion as backup...");
            ForceUnlockRegion(region);
            
            // Debug: Log the current state
            LogRegionUnlockState();
            
            // Additional verification
            Debug.Log("=== Verification ===");
            var unlockedRegions = GetUnlockedRegions();
            Debug.Log($"GetUnlockedRegions() returns {unlockedRegions.Count} regions: {string.Join(", ", unlockedRegions.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
            
            bool isStartingRegionUnlocked = IsRegionUnlocked(region);
            Debug.Log($"IsRegionUnlocked({region}) returns: {isStartingRegionUnlocked}");
            Debug.Log("=== End Verification ===");
            Debug.Log($"=== END SET STARTING REGION ===");
        }
        
        /// <summary>
        /// Force unlock a specific region (for testing)
        /// </summary>
        public void ForceUnlockRegion(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            
            Debug.Log($"ForceUnlockRegion called with region: {region}");
            
            if (_regionData.TryGetValue(region, out var data))
            {
                data.isUnlocked = true;
                Debug.Log($"Force unlocked region: {data.regionName} (isUnlocked = {data.isUnlocked})");
            }
            else
            {
                Debug.LogError($"Region {region} not found in _regionData!");
            }
        }

        /// <summary>
        /// Debug method to log the current region unlock state
        /// </summary>
        public void LogRegionUnlockState()
        {
            Debug.Log("=== Region Unlock State ===");
            Debug.Log($"Starting region: {AssessmentQuizManager.GetRegionDisplayName(_startingRegion)}");
            Debug.Log($"Current unlock index: {_currentUnlockIndex}");
            
            var unlockedRegions = GetUnlockedRegions();
            Debug.Log($"Unlocked regions ({unlockedRegions.Count}): {string.Join(", ", unlockedRegions.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
            
            var lockedRegions = GetLockedRegions();
            Debug.Log($"Locked regions ({lockedRegions.Count}): {string.Join(", ", lockedRegions.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
            
            var nextRegion = GetNextRegionToUnlock();
            if (nextRegion.HasValue)
            {
                Debug.Log($"Next region to unlock: {AssessmentQuizManager.GetRegionDisplayName(nextRegion.Value)}");
            }
            else
            {
                Debug.Log("No next region to unlock (all regions unlocked or no regions available)");
            }
            Debug.Log("==========================");
        }

        /// <summary>
        /// Check if a region is unlocked
        /// </summary>
        public bool IsRegionUnlocked(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            return _regionData.TryGetValue(region, out var data) && data.isUnlocked;
        }

        /// <summary>
        /// Get the starting region
        /// </summary>
        public AssessmentQuizManager.RegionType GetStartingRegion()
        {
            EnsureInitialized();
            return _startingRegion;
        }

        /// <summary>
        /// Add a building to a region and check for unlocks
        /// </summary>
        public void AddBuildingToRegion(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            if (!_regionData.TryGetValue(region, out var data))
                return;

            data.currentBuildingCount++;
            OnBuildingCountChanged?.Invoke(region, data.currentBuildingCount);

            // Check if this region has enough buildings to unlock the next region
            if (data.currentBuildingCount >= data.buildingsRequiredToUnlock)
            {
                UnlockNextRegion();
            }

            Debug.Log($"Added building to {data.regionName}. Count: {data.currentBuildingCount}/{data.buildingsRequiredToUnlock}");
        }

        /// <summary>
        /// Remove a building from a region
        /// </summary>
        public void RemoveBuildingFromRegion(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            if (!_regionData.TryGetValue(region, out var data))
                return;

            data.currentBuildingCount = Mathf.Max(0, data.currentBuildingCount - 1);
            OnBuildingCountChanged?.Invoke(region, data.currentBuildingCount);

            Debug.Log($"Removed building from {data.regionName}. Count: {data.currentBuildingCount}/{data.buildingsRequiredToUnlock}");
        }

        /// <summary>
        /// Get building count for a region
        /// </summary>
        public int GetBuildingCount(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            return _regionData.TryGetValue(region, out var data) ? data.currentBuildingCount : 0;
        }

        /// <summary>
        /// Get buildings required to unlock next region
        /// </summary>
        public int GetBuildingsRequired(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            return _regionData.TryGetValue(region, out var data) ? data.buildingsRequiredToUnlock : defaultBuildingsRequired;
        }

        /// <summary>
        /// Get progress (0-1) towards unlocking next region
        /// </summary>
        public float GetUnlockProgress(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            if (!_regionData.TryGetValue(region, out var data))
                return 0f;

            return Mathf.Clamp01((float)data.currentBuildingCount / data.buildingsRequiredToUnlock);
        }

        /// <summary>
        /// Get all unlocked regions
        /// </summary>
        public List<AssessmentQuizManager.RegionType> GetUnlockedRegions()
        {
            EnsureInitialized();
            var unlocked = new List<AssessmentQuizManager.RegionType>();
            foreach (var kvp in _regionData)
            {
                if (kvp.Value.isUnlocked)
                {
                    unlocked.Add(kvp.Key);
                }
            }
            return unlocked;
        }

        /// <summary>
        /// Get all locked regions
        /// </summary>
        public List<AssessmentQuizManager.RegionType> GetLockedRegions()
        {
            EnsureInitialized();
            var locked = new List<AssessmentQuizManager.RegionType>();
            foreach (var kvp in _regionData)
            {
                if (!kvp.Value.isUnlocked)
                {
                    locked.Add(kvp.Key);
                }
            }
            return locked;
        }

        /// <summary>
        /// Get region data for a specific region
        /// </summary>
        public RegionUnlockData GetRegionData(AssessmentQuizManager.RegionType region)
        {
            EnsureInitialized();
            return _regionData.TryGetValue(region, out var data) ? data : null;
        }

        /// <summary>
        /// Get all region data
        /// </summary>
        public List<RegionUnlockData> GetAllRegionData()
        {
            EnsureInitialized();
            return new List<RegionUnlockData>(regions);
        }

        /// <summary>
        /// Check if all regions are unlocked
        /// </summary>
        public bool AreAllRegionsUnlocked()
        {
            EnsureInitialized();
            foreach (var kvp in _regionData)
            {
                if (!kvp.Value.isUnlocked)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Get the unlock order list
        /// </summary>
        public List<AssessmentQuizManager.RegionType> GetUnlockOrder()
        {
            EnsureInitialized();
            return new List<AssessmentQuizManager.RegionType>(_unlockOrder);
        }

        /// <summary>
        /// Get the next region to unlock based on quiz score order
        /// </summary>
        public AssessmentQuizManager.RegionType? GetNextRegionToUnlock()
        {
            EnsureInitialized();
            // If all regions are unlocked, return null
            if (AreAllRegionsUnlocked())
                return null;

            // Find the next locked region in the unlock order
            for (int i = 0; i < _unlockOrder.Count; i++)
            {
                var region = _unlockOrder[i];
                if (!_regionData[region].isUnlocked)
                {
                    return region;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Set unlock order based on quiz scores (highest to lowest)
        /// </summary>
        private void SetUnlockOrderFromQuizScores(Dictionary<AssessmentQuizManager.RegionType, int> quizScores)
        {
            _unlockOrder.Clear();
            
            // Sort regions by quiz score (highest to lowest)
            var sortedRegions = new List<AssessmentQuizManager.RegionType>(quizScores.Keys);
            sortedRegions.Sort((a, b) => quizScores[b].CompareTo(quizScores[a])); // Descending order
            
            _unlockOrder.AddRange(sortedRegions);
        }

        /// <summary>
        /// Set default unlock order if no quiz scores provided
        /// </summary>
        private void SetDefaultUnlockOrder(AssessmentQuizManager.RegionType startingRegion)
        {
            _unlockOrder.Clear();
            _unlockOrder.Add(startingRegion);
            
            // Add remaining regions in default order
            var allRegions = new List<AssessmentQuizManager.RegionType>
            {
                AssessmentQuizManager.RegionType.HealthHarbor,
                AssessmentQuizManager.RegionType.MindPalace,
                AssessmentQuizManager.RegionType.CreativeCommons,
                AssessmentQuizManager.RegionType.SocialSquare
            };
            
            foreach (var region in allRegions)
            {
                if (region != startingRegion)
                {
                    _unlockOrder.Add(region);
                }
            }
        }

        private void UnlockNextRegion()
        {
            var nextRegion = GetNextRegionToUnlock();
            if (nextRegion.HasValue)
            {
                _regionData[nextRegion.Value].isUnlocked = true;
                
                // Update current unlock index to point to the next region in the order
                for (int i = 0; i < _unlockOrder.Count; i++)
                {
                    if (_unlockOrder[i] == nextRegion.Value)
                    {
                        _currentUnlockIndex = i + 1; // Move to next region in unlock order
                        break;
                    }
                }
                
                OnRegionUnlocked?.Invoke(nextRegion.Value);
                
                Debug.Log($"Unlocked new region: {AssessmentQuizManager.GetRegionDisplayName(nextRegion.Value)}");
            }
        }

        /// <summary>
        /// Save region unlock data
        /// </summary>
        public RegionUnlockSaveData GetSaveData()
        {
            EnsureInitialized();
            var saveData = new RegionUnlockSaveData
            {
                startingRegion = _startingRegion,
                regionStates = new Dictionary<AssessmentQuizManager.RegionType, RegionState>(),
                unlockOrder = new List<AssessmentQuizManager.RegionType>(_unlockOrder),
                currentUnlockIndex = _currentUnlockIndex
            };

            foreach (var kvp in _regionData)
            {
                saveData.regionStates[kvp.Key] = new RegionState
                {
                    isUnlocked = kvp.Value.isUnlocked,
                    currentBuildingCount = kvp.Value.currentBuildingCount
                };
            }

            return saveData;
        }

        /// <summary>
        /// Load region unlock data
        /// </summary>
        public void LoadSaveData(RegionUnlockSaveData saveData)
        {
            if (saveData == null) return;

            // Ensure regions are initialized before loading save data
            EnsureInitialized();

            _startingRegion = saveData.startingRegion;
            _unlockOrder = new List<AssessmentQuizManager.RegionType>(saveData.unlockOrder ?? new List<AssessmentQuizManager.RegionType>());
            _currentUnlockIndex = saveData.currentUnlockIndex;

            foreach (var kvp in saveData.regionStates)
            {
                if (_regionData.TryGetValue(kvp.Key, out var data))
                {
                    data.isUnlocked = kvp.Value.isUnlocked;
                    data.currentBuildingCount = kvp.Value.currentBuildingCount;
                }
            }

            Debug.Log($"Loaded region unlock data. Starting region: {AssessmentQuizManager.GetRegionDisplayName(_startingRegion)}");
        }

        /// <summary>
        /// Reset all region unlocks (for testing)
        /// </summary>
        public void ResetRegionUnlocks()
        {
            EnsureInitialized();
            foreach (var kvp in _regionData)
            {
                kvp.Value.isUnlocked = false;
                kvp.Value.currentBuildingCount = 0;
            }
            
            // Only unlock starting region
            if (_regionData.TryGetValue(_startingRegion, out var data))
            {
                data.isUnlocked = true;
            }
        }

        /// <summary>
        /// Reset all regions to locked state (for testing)
        /// </summary>
        public void ResetAllRegionsToLocked()
        {
            Debug.Log("=== RESETTING ALL REGIONS TO LOCKED ===");
            
            EnsureInitialized();
            
            foreach (var kvp in _regionData)
            {
                kvp.Value.isUnlocked = false;
                kvp.Value.currentBuildingCount = 0;
                Debug.Log($"Reset {kvp.Value.regionName} to locked");
            }
            
            _currentUnlockIndex = 0;
            
            Debug.Log("All regions reset to locked state");
            Debug.Log("=== END RESET ===");
        }
        
        /// <summary>
        /// Test method to directly unlock a region (for debugging)
        /// </summary>
        public void TestUnlockRegion(AssessmentQuizManager.RegionType region)
        {
            Debug.Log($"=== TEST UNLOCK REGION ===");
            Debug.Log($"Testing unlock for region: {region}");
            
            EnsureInitialized();
            Debug.Log($"_regionData count: {_regionData.Count}");
            
            if (_regionData.TryGetValue(region, out var data))
            {
                Debug.Log($"Found region data: {data.regionName}");
                Debug.Log($"Before unlock: isUnlocked = {data.isUnlocked}");
                
                data.isUnlocked = true;
                
                Debug.Log($"After unlock: isUnlocked = {data.isUnlocked}");
                
                // Test the IsRegionUnlocked method
                bool isUnlocked = IsRegionUnlocked(region);
                Debug.Log($"IsRegionUnlocked({region}) returns: {isUnlocked}");
                
                // Test GetUnlockedRegions
                var unlockedRegions = GetUnlockedRegions();
                Debug.Log($"GetUnlockedRegions() returns: {string.Join(", ", unlockedRegions.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
                
                Debug.Log($"=== END TEST ===");
            }
            else
            {
                Debug.LogError($"Region {region} not found in _regionData!");
                Debug.Log($"Available regions: {string.Join(", ", _regionData.Keys)}");
            }
        }
    }

    /// <summary>
    /// Save data for region unlock system
    /// </summary>
    [System.Serializable]
    public class RegionUnlockSaveData
    {
        public AssessmentQuizManager.RegionType startingRegion;
        public Dictionary<AssessmentQuizManager.RegionType, RegionState> regionStates;
        public List<AssessmentQuizManager.RegionType> unlockOrder;
        public int currentUnlockIndex;
    }

    /// <summary>
    /// Individual region state for saving
    /// </summary>
    [System.Serializable]
    public class RegionState
    {
        public bool isUnlocked;
        public int currentBuildingCount;
    }
} 
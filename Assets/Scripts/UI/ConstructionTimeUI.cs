using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using LifeCraft.Core;
using LifeCraft.Systems;
using System.Collections.Generic; // Added for List
using System.Linq; // Added for First()

namespace LifeCraft.UI
{
    /// <summary>
    /// Handles the construction time UI for buildings under construction.
    /// Shows progress bar, timer, and skip button.
    /// </summary>
    public class ConstructionTimeUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Enter the name of the construction panel GameObject")]
        public string constructionPanelName = "ConstructionTimeUI_Prefab";
        [Tooltip("Enter the name of the progress bar GameObject")]
        public string progressBarName = "ProgressBar";
        [Tooltip("Enter the name of the timer text GameObject")]
        public string timerTextName = "TimerText";
        [Tooltip("Enter the name of the skip button GameObject")]
        public string skipButtonName = "SkipButton";
        [Tooltip("Enter the name of the skip button text GameObject")]
        public string skipButtonTextName = "Text (TMP)";
        
        [Header("Configuration")]
        public float updateInterval = 1f; // How often to update the timer
        
        private BuildingConstructionData constructionData;
        private Dictionary<string, BuildingConstructionData> constructionProjects; // Track multiple buildings
        private Coroutine updateCoroutine;
        
        private GameObject constructionPanel;
        private Slider progressBar;
        private TextMeshProUGUI timerText;
        private Button skipButton;
        private TextMeshProUGUI skipButtonText;
        
        [System.Serializable]
        public class BuildingConstructionData
        {
            public string buildingName;
            public Vector3Int gridPosition;
            public string buildingId; // Unique ID for the building instance
            public float startTime;
            public float constructionDuration;
            public bool isCompleted;
            public string regionType;
            public List<string> skipQuestIds;
            public List<string> originalQuestTexts;
            public int totalSkipQuests;
            public int completedSkipQuests;
        }
        
        private void Start()
        {
            Debug.Log($"ConstructionTimeUI Start() called on {gameObject.name}");
            
            // Initialize construction projects dictionary
            constructionProjects = new Dictionary<string, BuildingConstructionData>();
            
            // Find UI elements as children of this GameObject
            if (constructionPanelName != null && constructionPanelName != "")
            {
                if (gameObject.name == constructionPanelName)
                {
                    constructionPanel = this.gameObject;
                }
                else
                {
                    Transform panelTransform = transform.Find(constructionPanelName);
                    if (panelTransform != null)
                    {
                        constructionPanel = panelTransform.gameObject;
                    }
                }
                
                if (constructionPanel != null)
                {
                    constructionPanel.SetActive(false);
                }
                else
                {
                    Debug.LogError($"Construction panel GameObject with name '{constructionPanelName}' not found as child of {gameObject.name}.");
                }
            }
                
            // Set up progress bar
            if (progressBarName != null && progressBarName != "")
            {
                Transform progressBarTransform = transform.Find(progressBarName);
                if (progressBarTransform != null)
                {
                    progressBar = progressBarTransform.GetComponent<Slider>();
                    if (progressBar == null)
                    {
                        Debug.LogError($"ProgressBar GameObject '{progressBarName}' found but has no Slider component.");
                    }
                }
                else
                {
                    Debug.LogError($"ProgressBar GameObject with name '{progressBarName}' not found as child of {gameObject.name}.");
                }
            }

            // Set up timer text
            if (timerTextName != null && timerTextName != "")
            {
                Transform timerTextTransform = transform.Find(timerTextName);
                if (timerTextTransform != null)
                {
                    timerText = timerTextTransform.GetComponent<TextMeshProUGUI>();
                    if (timerText == null)
                    {
                        Debug.LogError($"TimerText GameObject '{timerTextName}' found but has no TextMeshProUGUI component.");
                    }
                }
                else
                {
                    Debug.LogError($"TimerText GameObject with name '{timerTextName}' not found as child of {gameObject.name}.");
                }
            }

            // Set up skip button
            if (skipButtonName != null && skipButtonName != "")
            {
                Transform skipButtonTransform = transform.Find(skipButtonName);
                if (skipButtonTransform != null)
                {
                    skipButton = skipButtonTransform.GetComponent<Button>();
                    if (skipButton != null)
                    {
                        skipButton.onClick.AddListener(OnSkipButtonClicked);
                    }
                    else
                    {
                        Debug.LogError($"Skip button GameObject '{skipButtonName}' found but has no Button component.");
                    }
                }
                else
                {
                    Debug.LogError($"Skip button GameObject with name '{skipButtonName}' not found as child of {gameObject.name}.");
                }
            }

            // Set up skip button text
            if (skipButtonTextName != null && skipButtonTextName != "")
            {
                // First find the skip button, then find the text as its child
                Transform skipButtonTransform = transform.Find(skipButtonName);
                if (skipButtonTransform != null)
                {
                    Transform skipButtonTextTransform = skipButtonTransform.Find(skipButtonTextName);
                    if (skipButtonTextTransform != null)
                    {
                        skipButtonText = skipButtonTextTransform.GetComponent<TextMeshProUGUI>();
                        if (skipButtonText == null)
                        {
                            Debug.LogError($"Skip button text GameObject '{skipButtonTextName}' found but has no TextMeshProUGUI component.");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Skip button text GameObject with name '{skipButtonTextName}' not found as child of {skipButtonName}.");
                    }
                }
                else
                {
                    Debug.LogError($"Skip button GameObject with name '{skipButtonName}' not found, so cannot find its text child.");
                }
            }

            // Initial check for completion (e.g., if loaded from save)
            if (constructionData != null && constructionData.isCompleted)
            {
                CompleteConstruction();
            }
        }
        
        /// <summary>
        /// Start construction for a building
        /// </summary>
        public void StartConstruction(string buildingName, Vector3Int gridPosition, float constructionDurationMinutes, string regionType)
        {
            Debug.Log($"Starting construction for {buildingName} at {gridPosition} with {constructionDurationMinutes} minutes");
            
            // Create unique building ID (combination of name and timestamp)
            string buildingId = $"{buildingName}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
            
            // Create new construction data
            var newConstructionData = new BuildingConstructionData
            {
                buildingName = buildingName,
                gridPosition = gridPosition,
                buildingId = buildingId,
                startTime = Time.time,
                constructionDuration = constructionDurationMinutes * 60f, // Convert minutes to seconds
                isCompleted = false,
                regionType = regionType,
                skipQuestIds = new List<string>(),
                originalQuestTexts = new List<string>(),
                totalSkipQuests = 0,
                completedSkipQuests = 0
            };
            
            // Add to construction projects using building ID as key
            constructionProjects[buildingId] = newConstructionData;
            
            // Set as current construction data for UI display
            constructionData = newConstructionData;
            
            // Start the update coroutine immediately - construction should run in background
            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);
            updateCoroutine = StartCoroutine(UpdateConstructionUI());
            
            Debug.Log($"Construction started for {buildingName} with ID {buildingId}. Timer will run in background. Total projects: {constructionProjects.Count}");
        }
        
        /// <summary>
        /// Show construction UI for a specific building
        /// </summary>
        public void ShowConstructionUIForBuilding(string buildingName, Vector3Int gridPosition)
        {
            // Find the construction data for this specific building
            BuildingConstructionData targetConstruction = null;
            foreach (var project in constructionProjects.Values)
            {
                if (project.buildingName == buildingName && !project.isCompleted)
                {
                    targetConstruction = project;
                    break;
                }
            }
            
            if (targetConstruction == null)
            {
                Debug.LogWarning($"No active construction found for {buildingName} at {gridPosition}");
                return;
            }
            
            // Set as current construction data for UI display
            constructionData = targetConstruction;
            
            // Show the panel
            if (constructionPanel != null)
            {
                constructionPanel.SetActive(true);
            }
                
            // Ensure the update coroutine is running
            if (updateCoroutine == null)
            {
                updateCoroutine = StartCoroutine(UpdateConstructionUI());
                Debug.Log("Started UpdateConstructionUI coroutine (was not running)");
            }
            else
            {
                Debug.Log("UpdateConstructionUI coroutine already running, showing UI for specific building");
            }
            
            Debug.Log($"Showing construction UI for {buildingName} with ID {targetConstruction.buildingId}");
        }
        
        /// <summary>
        /// Update the construction UI (progress bar, timer, etc.)
        /// </summary>
        private IEnumerator UpdateConstructionUI()
        {
            Debug.Log($"UpdateConstructionUI coroutine started for {constructionData?.buildingName}");
            
            while (constructionProjects.Count > 0)
            {
                // Update UI for current construction data (if any)
                if (constructionData != null && !constructionData.isCompleted)
                {
                    float elapsedTime = Time.time - constructionData.startTime;
                    float remainingTime = constructionData.constructionDuration - elapsedTime;
                    
                    // Check if construction is complete
                    if (remainingTime <= 0)
                    {
                        Debug.Log($"Construction complete for {constructionData.buildingName}");
                        CompleteConstruction();
                        continue;
                    }
                    
                    // Update progress bar
                    if (progressBar != null)
                    {
                        float progress = elapsedTime / constructionData.constructionDuration;
                        progressBar.value = progress;
                    }
                    
                    // Update timer text
                    if (timerText != null)
                    {
                        timerText.text = FormatTime(remainingTime);
                    }
                    
                    // Update skip button text
                    if (skipButtonText != null)
                    {
                        int questCount = CalculateQuestCount(remainingTime);
                        skipButtonText.text = $"Skip ({questCount} quests)";
                    }
                }
                
                yield return new WaitForSeconds(updateInterval);
            }
            
            Debug.Log($"UpdateConstructionUI coroutine ended - no more construction projects");
        }
        
        /// <summary>
        /// Hide the construction UI (does NOT stop the timer)
        /// </summary>
        public void HideConstructionUI()
        {
            if (constructionPanel != null)
            {
                constructionPanel.SetActive(false);
            }
            // DO NOT stop the updateCoroutine here - it should continue running in background
        }
        
        /// <summary>
        /// Handle skip button click
        /// </summary>
        private void OnSkipButtonClicked()
        {
            if (constructionData == null || constructionData.isCompleted)
                return;
                
            float remainingTime = constructionData.constructionDuration - (Time.time - constructionData.startTime);
            int questCount = CalculateQuestCount(remainingTime);
            
            // Add construction skip quests to To-Do List
            AddConstructionSkipQuests(questCount);
            
            // DO NOT complete construction immediately - it should only complete when quests are finished
            // CompleteConstruction(); // REMOVED THIS LINE
            
            // Navigate to Home page (To-Do List)
            NavigateToHomePage();
            
            // Hide the construction UI
            HideConstructionUI();
            
            Debug.Log($"Skip button clicked for {constructionData.buildingName}. Added {questCount} quests. Construction continues until quests are completed.");
        }
        
        /// <summary>
        /// Calculate how many quests should be added based on remaining time
        /// </summary>
        private int CalculateQuestCount(float remainingTimeMinutes)
        {
            // Base formula: 1 quest per hour of remaining time, minimum 1 quest
            int questCount = Mathf.Max(1, Mathf.RoundToInt(remainingTimeMinutes / 60f));
            
            // Cap at 5 quests maximum
            return Mathf.Min(questCount, 5);
        }
        
        /// <summary>
        /// Add construction skip quests to To-Do List
        /// </summary>
        private void AddConstructionSkipQuests(int questCount)
        {
            var toDoListManager = FindFirstObjectByType<ToDoListManager>();
            if (toDoListManager == null)
            {
                Debug.LogError("ToDoListManager not found!");
                return;
            }
            
            // Get construction quest pool
            var constructionQuestPool = ConstructionQuestPool.Instance;
            if (constructionQuestPool == null)
            {
                Debug.LogError("ConstructionQuestPool not found!");
                return;
            }
            
            // Check if we already have quests for this construction
            if (constructionData.skipQuestIds.Count > 0)
            {
                Debug.Log($"Already have {constructionData.skipQuestIds.Count} skip quests for {constructionData.buildingName}. Re-adding incomplete quests.");
                
                // Re-add only the quests that haven't been completed
                for (int i = 0; i < constructionData.originalQuestTexts.Count; i++)
                {
                    string questId = constructionData.skipQuestIds[i];
                    string questText = constructionData.originalQuestTexts[i];
                    
                    // Check if this quest is still in the To-Do List (not completed)
                    bool questStillExists = false;
                    foreach (Transform item in toDoListManager.toDoListContainer)
                    {
                        string itemText = toDoListManager.GetQuestTextFromItem(item);
                        if (itemText == questText)
                        {
                            questStillExists = true;
                            break;
                        }
                    }
                    
                    // If quest was deleted (not completed), re-add it
                    if (!questStillExists)
                    {
                        toDoListManager.AddToDo(questText, false, 0);
                        Debug.Log($"Re-added deleted quest: {questText}");
                    }
                    else
                    {
                        Debug.Log($"Quest still exists in To-Do List: {questText}");
                    }
                }
                return;
            }
            
            // Clear any existing skip quest IDs
            constructionData.skipQuestIds.Clear();
            constructionData.originalQuestTexts.Clear();
            constructionData.totalSkipQuests = 0;
            constructionData.completedSkipQuests = 0;
            
            // Add quests based on region type and difficulty
            for (int i = 0; i < questCount; i++)
            {
                string questText = constructionQuestPool.GetRandomQuest(constructionData.regionType);
                if (!string.IsNullOrEmpty(questText))
                {
                    // Create a unique quest ID using timestamp and index
                    string questId = $"construction_skip_{constructionData.buildingName}_{Time.time}_{i}";
                    
                    // Add quest to To-Do List with clean text (no ugly prefix)
                    toDoListManager.AddToDo(questText, false, 0);
                    
                    // Store the quest ID and original text for tracking
                    constructionData.skipQuestIds.Add(questId);
                    constructionData.originalQuestTexts.Add(questText);
                    constructionData.totalSkipQuests++;
                    Debug.Log($"Added construction skip quest: {questText} with ID: {questId}");
                }
            }
            
            Debug.Log($"Added {questCount} construction skip quests for {constructionData.buildingName}. Total quests needed: {constructionData.totalSkipQuests}");
        }
        
        /// <summary>
        /// Navigate to Home page (To-Do List)
        /// </summary>
        private void NavigateToHomePage()
        {
            var uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                // This will need to be implemented based on your UI navigation system
                uiManager.ShowHomePanel();
            }
        }
        
        /// <summary>
        /// Complete the construction and clean up
        /// </summary>
        private void CompleteConstruction()
        {
            if (constructionData == null)
                return;
                
            string buildingName = constructionData.buildingName;
            string buildingId = constructionData.buildingId;
            
            // Mark as completed
            constructionData.isCompleted = true;
            
            // Remove from construction projects using building ID
            if (constructionProjects.ContainsKey(buildingId))
            {
                constructionProjects.Remove(buildingId);
            }
            
            // Hide UI
            HideConstructionUI();
            
            // Show completion notification
            Debug.Log($"Construction completed for {buildingName} with ID {buildingId}");
            
            // Clear current construction data if this was the one being displayed
            if (constructionData.buildingId == buildingId)
            {
                constructionData = null;
            }
            
            // If there are other construction projects, set the first one as current
            if (constructionProjects.Count > 0)
            {
                constructionData = constructionProjects.Values.First();
            }
        }
        
        /// <summary>
        /// Format time in MM:SS format
        /// </summary>
        private string FormatTime(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            return string.Format("{0:D2}m {1:D2}s", minutes, seconds);
        }
        
        /// <summary>
        /// Save construction data to persistent storage
        /// </summary>
        private void SaveConstructionData()
        {
            // TODO: Implement save/load system for construction data
            // This should save to PlayerPrefs or a save file
        }
        
        /// <summary>
        /// Load construction data from persistent storage
        /// </summary>
        public void LoadConstructionData()
        {
            // TODO: Implement save/load system for construction data
            // This should load from PlayerPrefs or a save file
        }
        
        /// <summary>
        /// Check if any building is currently under construction
        /// </summary>
        public bool IsUnderConstruction()
        {
            return constructionData != null && !constructionData.isCompleted;
        }

        /// <summary>
        /// Check if a specific building is under construction
        /// </summary>
        public bool IsBuildingUnderConstruction(string buildingName, Vector3Int gridPosition)
        {
            // Check if any construction project matches this building name
            foreach (var project in constructionProjects.Values)
            {
                if (project.buildingName == buildingName && !project.isCompleted)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if all skip quests are completed and complete construction if they are
        /// This should be called when quests are completed in the To-Do List
        /// </summary>
        public void CheckSkipQuestCompletion(string completedQuestText)
        {
            if (constructionData == null || constructionData.isCompleted)
                return;
                
            // Check if this completed quest is one of our skip quests
            if (constructionData.originalQuestTexts.Contains(completedQuestText))
            {
                // Find the index of the completed quest
                int questIndex = constructionData.originalQuestTexts.IndexOf(completedQuestText);
                if (questIndex >= 0)
                {
                    // Remove the completed quest from our tracking
                    constructionData.originalQuestTexts.RemoveAt(questIndex);
                    constructionData.skipQuestIds.RemoveAt(questIndex);
                    constructionData.completedSkipQuests++;
                    
                    Debug.Log($"Skip quest completed for {constructionData.buildingName}: {completedQuestText}. Completed: {constructionData.completedSkipQuests}/{constructionData.totalSkipQuests}");
                    
                    // Check if all skip quests are completed
                    if (constructionData.originalQuestTexts.Count == 0)
                    {
                        Debug.Log($"All skip quests completed for {constructionData.buildingName}! Completing construction.");
                        CompleteConstruction();
                    }
                }
            }
        }
        
        /// <summary>
        /// Get the list of skip quest IDs for this construction
        /// </summary>
        public List<string> GetSkipQuestIds()
        {
            return constructionData?.skipQuestIds ?? new List<string>();
        }
        
        /// <summary>
        /// Get the construction data for external access
        /// </summary>
        public BuildingConstructionData GetConstructionData()
        {
            return constructionData;
        }
    }
} 
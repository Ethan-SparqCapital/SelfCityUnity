using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using LifeCraft.Core;
using LifeCraft.Systems;

namespace LifeCraft.UI
{
    /// <summary>
    /// Individual construction timer component for each building
    /// </summary>
    public class BuildingConstructionTimer : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject constructionPanel;
        public Slider progressBar;
        public TextMeshProUGUI timerText;
        public Button skipButton;
        public TextMeshProUGUI skipButtonText;
        
        [Header("Configuration")]
        public float updateInterval = 1f;
        
        private string buildingName;
        private Vector3Int gridPosition;
        private float constructionDuration;
        private string regionType;
        private float startTime;
        private bool isCompleted = false;
        private Coroutine updateCoroutine;
        
        // Quest tracking
        private List<string> originalQuestTexts = new List<string>();
        private List<string> skipQuestIds = new List<string>();
        private int totalSkipQuests = 0;
        private int completedSkipQuests = 0;
        
        /// <summary>
        /// Start construction for this building
        /// </summary>
        public void StartConstruction(string buildingName, Vector3Int gridPosition, float constructionDurationMinutes, string regionType)
        {
            this.buildingName = buildingName;
            this.gridPosition = gridPosition;
            this.constructionDuration = constructionDurationMinutes * 60f; // Convert to seconds
            this.regionType = regionType;
            this.startTime = Time.time;
            this.isCompleted = false;
            
            // Reset quest tracking
            originalQuestTexts.Clear();
            skipQuestIds.Clear();
            totalSkipQuests = 0;
            completedSkipQuests = 0;
            
            // Show the construction panel
            if (constructionPanel != null)
            {
                constructionPanel.SetActive(true);
            }
            
            // Set up the skip button
            if (skipButton != null)
            {
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(OnSkipButtonClicked);
            }
            
            // Start the update coroutine
            if (updateCoroutine != null)
                StopCoroutine(updateCoroutine);
            updateCoroutine = StartCoroutine(UpdateTimer());
            
            Debug.Log($"Started construction for {buildingName} with {constructionDurationMinutes} minutes");
        }
        
        /// <summary>
        /// Update the construction timer
        /// </summary>
        private IEnumerator UpdateTimer()
        {
            while (!isCompleted)
            {
                float elapsedTime = Time.time - startTime;
                float remainingTime = constructionDuration - elapsedTime;
                
                // Check if construction is complete
                if (remainingTime <= 0)
                {
                    CompleteConstruction();
                    break;
                }
                
                // Update progress bar
                if (progressBar != null)
                {
                    float progress = elapsedTime / constructionDuration;
                    progressBar.value = progress;
                }
                
                // Update timer text
                if (timerText != null)
                {
                    timerText.text = FormatTime(remainingTime);
                }
                
                // Update skip button text (shows remaining quests if skip was clicked, or new quest count if not)
                if (skipButtonText != null)
                {
                    if (originalQuestTexts.Count > 0)
                    {
                        // Skip was clicked - show remaining quests
                        skipButtonText.text = $"Skip ({originalQuestTexts.Count} quests remaining)";
                    }
                    else
                    {
                        // Skip not clicked yet - show quest count needed
                        int questCount = CalculateQuestCount(remainingTime);
                        QuestDifficulty difficulty = CalculateDifficultyForTime(remainingTime / 60f);
                        string difficultyText = difficulty.ToString();
                        skipButtonText.text = $"Skip ({questCount} {difficultyText} quests)";
                    }
                }
                
                yield return new WaitForSeconds(updateInterval);
            }
        }
        
        /// <summary>
        /// Complete construction
        /// </summary>
        private void CompleteConstruction()
        {
            isCompleted = true;
            
            // Hide the construction panel only when construction is actually complete
            if (constructionPanel != null)
            {
                constructionPanel.SetActive(false);
            }
            
            Debug.Log($"Construction completed for {buildingName} - panel hidden");
        }
        
        /// <summary>
        /// Check if this building is under construction
        /// </summary>
        public bool IsUnderConstruction()
        {
            return !isCompleted;
        }
        
        /// <summary>
        /// Format time as MM:SS
        /// </summary>
        private string FormatTime(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        
        /// <summary>
        /// Calculate number of quests needed to skip
        /// </summary>
        private int CalculateQuestCount(float remainingTime)
        {
            // Scale quests based on remaining time
            // 24 hours = 1440 minutes = max 10 quests
            // 1 hour = 60 minutes = ~4 quests
            // 30 minutes = ~2 quests
            // 10 minutes = 1 quest
            
            float remainingMinutes = remainingTime / 60f;
            int questCount = Mathf.CeilToInt(remainingMinutes / 144f); // 1440 minutes / 10 quests = 144 minutes per quest
            
            // Clamp between 1 and 10 quests
            return Mathf.Clamp(questCount, 1, 10);
        }
        
        /// <summary>
        /// Handle skip button click
        /// </summary>
        public void OnSkipButtonClicked()
        {
            Debug.Log($"Skip button clicked for {buildingName}");
            
            // Calculate remaining time and quest count
            float elapsedTime = Time.time - startTime;
            float remainingTime = constructionDuration - elapsedTime;
            int questCount = CalculateQuestCount(remainingTime);
            
            // Add construction skip quests to To-Do List
            AddConstructionSkipQuests(questCount);
            
            // Keep the construction panel visible - it will only hide when ALL quests are completed
            Debug.Log($"Added {questCount} quests for {buildingName}. Construction panel remains visible until all quests are completed.");
        }
        
        /// <summary>
        /// Add construction skip quests to the To-Do List
        /// </summary>
        private void AddConstructionSkipQuests(int questCount)
        {
            if (ConstructionQuestPool.Instance == null)
            {
                Debug.LogError("ConstructionQuestPool.Instance is null!");
                return;
            }
            
            // Find ToDoListManager in the scene
            ToDoListManager toDoListManager = FindObjectOfType<ToDoListManager>();
            if (toDoListManager == null)
            {
                Debug.LogError("ToDoListManager not found in scene!");
                return;
            }
            
            // Clear any existing quests for this building
            RemoveExistingQuests();
            
            // Get quests based on region type and difficulty
            var quests = GetQuestsForRegion(regionType, questCount);
            
            // Add each quest to the To-Do List
            foreach (string questText in quests)
            {
                // Generate a unique quest ID
                string questId = $"construction_skip_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
                
                // Store the original quest text and ID
                originalQuestTexts.Add(questText);
                skipQuestIds.Add(questId);
                
                // Add to To-Do List (without the ugly prefix)
                toDoListManager.AddToDo(questText);
                
                Debug.Log($"Added construction quest: {questText}");
            }
            
            totalSkipQuests = quests.Count;
            completedSkipQuests = 0;
            
            Debug.Log($"Added {quests.Count} construction skip quests for {buildingName}");
        }
        
        /// <summary>
        /// Remove existing quests for this building from the To-Do List
        /// </summary>
        private void RemoveExistingQuests()
        {
            // Find ToDoListManager in the scene
            ToDoListManager toDoListManager = FindObjectOfType<ToDoListManager>();
            if (toDoListManager == null) return;
            
            // Remove quests from To-Do List that match our original texts
            foreach (string questText in originalQuestTexts)
            {
                toDoListManager.RemoveToDo(questText);
            }
            
            originalQuestTexts.Clear();
            skipQuestIds.Clear();
            totalSkipQuests = 0;
            completedSkipQuests = 0;
        }
        
        /// <summary>
        /// Get quests for the specified region and count with difficulty scaling
        /// </summary>
        private List<string> GetQuestsForRegion(string regionType, int questCount)
        {
            var quests = new List<string>();
            
            // Calculate difficulty based on remaining time
            float remainingMinutes = (constructionDuration - (Time.time - startTime)) / 60f;
            QuestDifficulty targetDifficulty = CalculateDifficultyForTime(remainingMinutes);
            
            // Get quests from the ConstructionQuestPool based on region
            var regionQuests = ConstructionQuestPool.Instance.GetQuestsForRegion(regionType);
            
            if (regionQuests != null && regionQuests.Count > 0)
            {
                // Filter quests by difficulty and shuffle
                var difficultyQuests = FilterQuestsByDifficulty(regionQuests, targetDifficulty);
                var shuffledQuests = new List<string>(difficultyQuests);
                
                // Shuffle the quests
                for (int i = shuffledQuests.Count - 1; i > 0; i--)
                {
                    int j = Random.Range(0, i + 1);
                    string temp = shuffledQuests[i];
                    shuffledQuests[i] = shuffledQuests[j];
                    shuffledQuests[j] = temp;
                }
                
                // Take the required number of quests
                for (int i = 0; i < Mathf.Min(questCount, shuffledQuests.Count); i++)
                {
                    quests.Add(shuffledQuests[i]);
                }
            }
            
            // If we don't have enough region-specific quests, add generic ones with appropriate difficulty
            while (quests.Count < questCount)
            {
                quests.Add(GetGenericQuestForDifficulty(targetDifficulty));
            }
            
            Debug.Log($"Generated {quests.Count} quests for {regionType} with difficulty {targetDifficulty} (remaining: {remainingMinutes:F1} minutes)");
            return quests;
        }
        
        /// <summary>
        /// Check if a quest completion matches our construction quests
        /// </summary>
        public void CheckSkipQuestCompletion(string completedQuestText)
        {
            if (originalQuestTexts.Contains(completedQuestText))
            {
                // Remove the completed quest from our tracking
                int index = originalQuestTexts.IndexOf(completedQuestText);
                if (index >= 0)
                {
                    originalQuestTexts.RemoveAt(index);
                    if (index < skipQuestIds.Count)
                    {
                        skipQuestIds.RemoveAt(index);
                    }
                    completedSkipQuests++;
                }
                
                Debug.Log($"Construction quest completed for {buildingName}: {completedQuestText} ({originalQuestTexts.Count} quests remaining)");
                
                // Check if all quests are completed
                if (originalQuestTexts.Count == 0)
                {
                    Debug.Log($"All construction quests completed for {buildingName}. Completing construction and hiding panel.");
                    CompleteConstruction();
                }
            }
        }
        
        /// <summary>
        /// Check if this timer has the specified quest text in its tracking
        /// </summary>
        public bool HasQuest(string questText)
        {
            return originalQuestTexts.Contains(questText);
        }
        
        /// <summary>
        /// Calculate difficulty based on remaining time
        /// </summary>
        private QuestDifficulty CalculateDifficultyForTime(float remainingMinutes)
        {
            // Scale difficulty based on remaining time
            // 24+ hours (1440+ minutes) = Expert (hardest)
            // 12-24 hours (720-1440 minutes) = Hard
            // 2-12 hours (120-720 minutes) = Medium
            // 0-2 hours (0-120 minutes) = Easy
            
            if (remainingMinutes >= 1440f) return QuestDifficulty.Expert;
            if (remainingMinutes >= 720f) return QuestDifficulty.Hard;
            if (remainingMinutes >= 120f) return QuestDifficulty.Medium;
            return QuestDifficulty.Easy;
        }
        
        /// <summary>
        /// Filter quests by difficulty
        /// </summary>
        private List<string> FilterQuestsByDifficulty(List<string> allQuests, QuestDifficulty targetDifficulty)
        {
            var filteredQuests = new List<string>();
            
            foreach (string quest in allQuests)
            {
                QuestDifficulty questDifficulty = DetermineQuestDifficulty(quest);
                if (questDifficulty == targetDifficulty)
                {
                    filteredQuests.Add(quest);
                }
            }
            
            // If no quests found for target difficulty, try easier difficulties
            if (filteredQuests.Count == 0)
            {
                foreach (string quest in allQuests)
                {
                    QuestDifficulty questDifficulty = DetermineQuestDifficulty(quest);
                    if (questDifficulty <= targetDifficulty)
                    {
                        filteredQuests.Add(quest);
                    }
                }
            }
            
            return filteredQuests;
        }
        
        /// <summary>
        /// Determine quest difficulty based on quest text
        /// </summary>
        private QuestDifficulty DetermineQuestDifficulty(string questText)
        {
            string lowerText = questText.ToLower();
            
            // EASY: Short duration, simple actions
            if (lowerText.Contains("5 minute") || lowerText.Contains("2 minute") || 
                lowerText.Contains("quick") || lowerText.Contains("simple") || 
                lowerText.Contains("small") || lowerText.Contains("just") || 
                lowerText.Contains("30 second") || lowerText.Contains("1 minute") ||
                lowerText.Contains("take a") || lowerText.Contains("do a quick") ||
                lowerText.Contains("smile at") || lowerText.Contains("compliment"))
                return QuestDifficulty.Easy;
            
            // MEDIUM: Moderate duration, practice actions
            if (lowerText.Contains("10 minute") || lowerText.Contains("15 minute") ||
                lowerText.Contains("try") || lowerText.Contains("practice") ||
                lowerText.Contains("write down") || lowerText.Contains("read a") ||
                lowerText.Contains("watch a") || lowerText.Contains("learn") ||
                lowerText.Contains("organize") || lowerText.Contains("make a") ||
                lowerText.Contains("create") || lowerText.Contains("design") ||
                lowerText.Contains("call or video") || lowerText.Contains("share something"))
                return QuestDifficulty.Medium;
            
            // HARD: Longer duration, complete actions
            if (lowerText.Contains("20 minute") || lowerText.Contains("30 minute") ||
                lowerText.Contains("complete") || lowerText.Contains("go for a") ||
                lowerText.Contains("drink 8 glass") || lowerText.Contains("get at least 30 minute") ||
                lowerText.Contains("track your") || lowerText.Contains("do a set of"))
                return QuestDifficulty.Hard;
            
            // EXPERT: Very long duration, complex actions
            if (lowerText.Contains("60 minute") || lowerText.Contains("2 hour") ||
                lowerText.Contains("complete a full") || lowerText.Contains("spend the entire") ||
                lowerText.Contains("master") || lowerText.Contains("advanced"))
                return QuestDifficulty.Expert;
            
            // Default to Medium if unclear
            return QuestDifficulty.Medium;
        }
        
        /// <summary>
        /// Get a generic quest for the specified difficulty
        /// </summary>
        private string GetGenericQuestForDifficulty(QuestDifficulty difficulty)
        {
            switch (difficulty)
            {
                case QuestDifficulty.Easy:
                    return "Take a 5-minute break and stretch";
                case QuestDifficulty.Medium:
                    return "Spend 15 minutes on a hobby you enjoy";
                case QuestDifficulty.Hard:
                    return "Complete a 30-minute workout session";
                case QuestDifficulty.Expert:
                    return "Spend 2 hours learning a new skill";
                default:
                    return "Complete a healthy habit task";
            }
        }
    }
} 
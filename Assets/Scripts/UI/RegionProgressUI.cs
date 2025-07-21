using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using LifeCraft.Core;
using LifeCraft.Systems;

namespace LifeCraft.UI
{
    /// <summary>
    /// Displays building count and progress toward unlocking the next region
    /// </summary>
    public class RegionProgressUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject progressPanel;
        [SerializeField] private TMP_Text progressTitleText;
        [SerializeField] private TMP_Text progressDescriptionText;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button toggleButton; // Button to show/hide the panel

        [Header("Settings")]
        [SerializeField] private bool showOnStart = true;
        [SerializeField] private float updateInterval = 1f; // How often to update the display

        private float _lastUpdateTime;

        private void Start()
        {
            Debug.Log("RegionProgressUI Start() called");
            
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(HideProgressPanel);
                Debug.Log("Close button listener added");
            }
            else
            {
                Debug.LogWarning("Close button is null!");
            }

            // Subscribe to building count changes to update progress immediately
            if (GameManager.Instance?.RegionUnlockSystem != null)
            {
                GameManager.Instance.RegionUnlockSystem.OnBuildingCountChanged += OnBuildingCountChanged;
                Debug.Log("Subscribed to RegionUnlockSystem.OnBuildingCountChanged event");
            }

            // Note: toggleButton listener is set up in the scene file, so we don't add it here
            // to avoid duplicate calls

            if (showOnStart)
                ShowProgressPanel();
            else
                HideProgressPanel();
        }

        private void Update()
        {
            // Only update if the panel is visible and enough time has passed
            if (progressPanel != null && progressPanel.activeSelf && Time.time - _lastUpdateTime > updateInterval)
            {
                UpdateProgressDisplay();
                _lastUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Show the progress panel
        /// </summary>
        public void ShowProgressPanel()
        {
            if (progressPanel != null)
                progressPanel.SetActive(true);
            
            UpdateProgressDisplay();
        }

        /// <summary>
        /// Force update the progress display (called from GameManager)
        /// </summary>
        public void ForceUpdateDisplay()
        {
            UpdateProgressDisplay();
        }

        /// <summary>
        /// Hide the progress panel
        /// </summary>
        public void HideProgressPanel()
        {
            if (progressPanel != null)
                progressPanel.SetActive(false);
        }

        /// <summary>
        /// Toggle the progress panel visibility
        /// </summary>
        public void ToggleProgressPanel()
        {
            Debug.Log("ToggleProgressPanel called");
            
            if (progressPanel != null)
            {
                bool isActive = progressPanel.activeSelf;
                Debug.Log($"Progress panel is currently {(isActive ? "active" : "inactive")}");
                
                progressPanel.SetActive(!isActive);
                Debug.Log($"Progress panel set to {(isActive ? "inactive" : "active")}");
                
                if (!isActive)
                {
                    UpdateProgressDisplay(); // Refresh data when showing
                }
            }
            else
            {
                Debug.LogError("Progress panel is null! Check the Inspector assignment.");
            }
        }

        /// <summary>
        /// Update the progress display with current unlock information
        /// </summary>
        private void UpdateProgressDisplay()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager.Instance is null!");
                return;
            }

            if (GameManager.Instance.RegionUnlockSystem == null)
            {
                Debug.LogWarning("RegionUnlockSystem is null!");
                return;
            }

            var unlockSystem = GameManager.Instance.RegionUnlockSystem;
            var unlockedRegions = unlockSystem.GetUnlockedRegions();
            var nextRegion = unlockSystem.GetNextRegionToUnlock();

            // If no regions are unlocked yet, show progress for the starting region
            if (unlockedRegions.Count == 0)
            {
                var startingRegion = unlockSystem.GetStartingRegion();
                
                var startingRegionData = unlockSystem.GetRegionData(startingRegion);
                if (startingRegionData == null)
                {
                    Debug.LogError($"Starting region data is null for {AssessmentQuizManager.GetRegionDisplayName(startingRegion)}!");
                    return;
                }
                
                Debug.Log($"Starting region data: {startingRegionData.regionName}, Buildings: {startingRegionData.currentBuildingCount}/{startingRegionData.buildingsRequiredToUnlock}");
                
                if (progressTitleText != null)
                    progressTitleText.text = $"Complete {AssessmentQuizManager.GetRegionDisplayName(startingRegion)}";
                
                if (progressDescriptionText != null)
                    progressDescriptionText.text = $"Place buildings in {AssessmentQuizManager.GetRegionDisplayName(startingRegion)} to unlock the next region";
                
                if (progressBar != null)
                    progressBar.value = unlockSystem.GetUnlockProgress(startingRegion);
                
                if (progressText != null)
                    progressText.text = $"{startingRegionData.currentBuildingCount} / {startingRegionData.buildingsRequiredToUnlock} buildings";
                
                return;
            }

            // If all regions are unlocked
            if (!nextRegion.HasValue)
            {
                if (progressTitleText != null)
                    progressTitleText.text = "All Regions Unlocked!";
                
                if (progressDescriptionText != null)
                    progressDescriptionText.text = "Congratulations! You've unlocked all regions.";
                
                if (progressBar != null)
                    progressBar.value = 1f;
                
                if (progressText != null)
                    progressText.text = "Complete!";
                
                return;
            }

            // Show progress for next region to unlock
            var nextRegionData = unlockSystem.GetRegionData(nextRegion.Value);
            if (nextRegionData == null)
            {
                Debug.LogError($"Next region data is null for {AssessmentQuizManager.GetRegionDisplayName(nextRegion.Value)}!");
                return;
            }

            // Update title
            if (progressTitleText != null)
                progressTitleText.text = $"Unlock {AssessmentQuizManager.GetRegionDisplayName(nextRegion.Value)}";

            // Find the region that needs to be completed to unlock the next region
            var regionToComplete = GetRegionToCompleteForNextUnlock(unlockSystem, nextRegion.Value);
            
            // Update description
            if (progressDescriptionText != null)
            {
                if (regionToComplete.HasValue)
                {
                    var regionToCompleteData = unlockSystem.GetRegionData(regionToComplete.Value);
                    if (regionToCompleteData != null)
                    {
                        progressDescriptionText.text = $"Complete {AssessmentQuizManager.GetRegionDisplayName(regionToComplete.Value)} to unlock {AssessmentQuizManager.GetRegionDisplayName(nextRegion.Value)}";
                    }
                    else
                    {
                        progressDescriptionText.text = $"Place buildings to unlock {AssessmentQuizManager.GetRegionDisplayName(nextRegion.Value)}";
                    }
                }
                else
                {
                    progressDescriptionText.text = $"Place buildings to unlock {AssessmentQuizManager.GetRegionDisplayName(nextRegion.Value)}";
                }
            }

            // Update progress bar and text
            var progress = unlockSystem.GetUnlockProgress(nextRegion.Value);
            
            if (progressBar != null)
                progressBar.value = progress;
            
            if (progressText != null)
            {
                if (regionToComplete.HasValue)
                {
                    var regionToCompleteData = unlockSystem.GetRegionData(regionToComplete.Value);
                    if (regionToCompleteData != null)
                    {
                        progressText.text = $"{regionToCompleteData.currentBuildingCount} / {regionToCompleteData.buildingsRequiredToUnlock} buildings";
                    }
                    else
                    {
                        progressText.text = $"{nextRegionData.currentBuildingCount} / {nextRegionData.buildingsRequiredToUnlock} buildings";
                    }
                }
                else
                {
                    progressText.text = $"{nextRegionData.currentBuildingCount} / {nextRegionData.buildingsRequiredToUnlock} buildings";
                }
            }
        }

        /// <summary>
        /// Get the region that needs to be completed to unlock the next region
        /// </summary>
        private AssessmentQuizManager.RegionType? GetRegionToCompleteForNextUnlock(RegionUnlockSystem unlockSystem, AssessmentQuizManager.RegionType nextRegion)
        {
            var unlockOrder = unlockSystem.GetUnlockOrder();
            if (unlockOrder == null || unlockOrder.Count == 0) return null;

            // Get the starting region to exclude it from consideration
            var startingRegion = unlockSystem.GetStartingRegion();
            
            // Create a filtered unlock order that excludes the starting region
            var filteredUnlockOrder = new List<AssessmentQuizManager.RegionType>();
            foreach (var region in unlockOrder)
            {
                if (region != startingRegion)
                {
                    filteredUnlockOrder.Add(region);
                }
            }

            // Find the next region in the filtered unlock order
            int nextRegionIndex = -1;
            for (int i = 0; i < filteredUnlockOrder.Count; i++)
            {
                if (filteredUnlockOrder[i] == nextRegion)
                {
                    nextRegionIndex = i;
                    break;
                }
            }

            // If next region is not found or is the first region in filtered order, no prerequisite
            if (nextRegionIndex <= 0) return null;

            // Return the region that comes before the next region in filtered unlock order
            return filteredUnlockOrder[nextRegionIndex - 1];
        }

        /// <summary>
        /// Handle building count changes (called when buildings are placed or removed)
        /// </summary>
        private void OnBuildingCountChanged(AssessmentQuizManager.RegionType region, int newCount)
        {
            Debug.Log($"Building count changed for {AssessmentQuizManager.GetRegionDisplayName(region)}: {newCount}");
            
            // Only update if the progress panel is visible
            if (progressPanel != null && progressPanel.activeSelf)
            {
                UpdateProgressDisplay();
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveListener(HideProgressPanel);
            
            // Unsubscribe from events to prevent memory leaks
            if (GameManager.Instance?.RegionUnlockSystem != null)
            {
                GameManager.Instance.RegionUnlockSystem.OnBuildingCountChanged -= OnBuildingCountChanged;
            }
            
            // No need to remove toggleButton listener here as it's set up in the scene file
        }
    }
} 
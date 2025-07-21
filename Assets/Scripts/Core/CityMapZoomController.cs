using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TMP_Text. 
using System.Collections;
using System.Collections.Generic;
using LifeCraft.Systems;

namespace LifeCraft.Core
{
    public class CityMapZoomController : MonoBehaviour
    {
        [Header("Assign the RectTransform of your city map (e.g., CityScrollView)")]
        public RectTransform cityMapRect;

        [Header("Zoom/Pan Settings")]
        public float zoomDuration = 0.5f;

        [Header("Unlock System Integration")]
        [SerializeField] private GameObject lockedRegionPopup;
        [SerializeField] private TMP_Text lockedRegionText;
        [SerializeField] private Button closeLockedRegionPopupButton;

        // Define target positions and scales for each region (customize as needed)
        [System.Serializable]
        public struct RegionZoomData
        {
            public string regionName;
            public Vector2 anchoredPosition;
            public float scale;
        }

        public RegionZoomData[] regions;

        // Optional: Button references for convenience
        public Button healthHarborButton;
        public Button mindPalaceButton;
        public Button creativeCommonsButton;
        public Button socialSquareButton;

        public GameObject regionButtonGroup; // Group for "Back to Overview" and "Edit Region" buttons; assigned in the Inspector. 

        private Coroutine zoomCoroutine;

        void Start()
        {
            // Hook up buttons if assigned
            if (healthHarborButton != null)
                healthHarborButton.onClick.AddListener(() => ZoomToRegion("Health Harbor"));
            if (mindPalaceButton != null)
                mindPalaceButton.onClick.AddListener(() => ZoomToRegion("Mind Palace"));
            if (creativeCommonsButton != null)
                creativeCommonsButton.onClick.AddListener(() => ZoomToRegion("Creative Commons"));
            if (socialSquareButton != null)
                socialSquareButton.onClick.AddListener(() => ZoomToRegion("Social Square"));

            // Set up locked region popup
            if (closeLockedRegionPopupButton != null)
                closeLockedRegionPopupButton.onClick.AddListener(HideLockedRegionPopup);

            // Hide popup initially
            if (lockedRegionPopup != null)
                lockedRegionPopup.SetActive(false);
                
            // Force refresh unlock state after a delay to ensure GameManager is initialized
            StartCoroutine(RefreshUnlockStateAfterDelay());
        }
        
        /// <summary>
        /// Force refresh the unlock state (call this when regions are unlocked)
        /// </summary>
        public void ForceRefreshUnlockState()
        {
            Debug.Log("CityMapZoomController: Force refreshing unlock state");
            
            var unlockSystem = LifeCraft.Systems.RegionUnlockSystem.Instance;
            if (unlockSystem != null)
            {
                var unlockedRegions = unlockSystem.GetUnlockedRegions();
                Debug.Log($"CityMapZoomController: Current unlocked regions: {string.Join(", ", unlockedRegions.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
            }
            else
            {
                Debug.LogWarning("CityMapZoomController: RegionUnlockSystem.Instance is null");
            }
        }

        /// <summary>
        /// Refresh unlock state after a delay to ensure GameManager is initialized
        /// </summary>
        private System.Collections.IEnumerator RefreshUnlockStateAfterDelay()
        {
            yield return new WaitForSeconds(0.2f);
            Debug.Log("CityMapZoomController: Refreshing unlock state after delay");
            
            // Log current unlock state for debugging
            var unlockSystem = LifeCraft.Systems.RegionUnlockSystem.Instance;
            if (unlockSystem != null)
            {
                var unlockedRegions = unlockSystem.GetUnlockedRegions();
                Debug.Log($"CityMapZoomController: Current unlocked regions: {string.Join(", ", unlockedRegions.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
            }
            else
            {
                Debug.LogWarning("CityMapZoomController: RegionUnlockSystem.Instance is null in delayed refresh");
            }
        }

        public void ZoomToRegion(string regionName)
        {
            // Overview should always be accessible - skip unlock check for it
            if (regionName != "Overview") // If the region is not the Overview, check if it is unlocked. 
            {
                // Check if region is unlocked before allowing zoom
                if (!IsRegionUnlocked(regionName)) // If the region is not unlocked, show the locked region popup. 
                {
                    ShowLockedRegionPopup(regionName);
                    return;
                }
            }

            for (int i = 0; i < regions.Length; i++)
            {
                if (regions[i].regionName == regionName)
                {
                    ZoomTo(regions[i].anchoredPosition, regions[i].scale);

                    // Show the region button group (e.g., "Back to Overview" and "Edit Region" buttons) only if not in Overview:
                    if (regionName == "Overview")
                    {
                        regionButtonGroup.SetActive(false); // Hide the button group when zoomed out to overview. 
                    }
                    else
                    {
                        regionButtonGroup.SetActive(true); // Show the button group when zoomed into a specific region. 
                    }
                    
                    return;
                }
            }
            Debug.LogWarning("Region not found: " + regionName);
        }

        public void ZoomTo(Vector2 targetPosition, float targetScale)
        {
            if (zoomCoroutine != null)
                StopCoroutine(zoomCoroutine);
            zoomCoroutine = StartCoroutine(AnimateZoom(targetPosition, targetScale));
        }

        public void ZoomToOverview()
        {
            // "Overview" region in the CityScrollView GameObject (under "City Map Zoom Controller" script component) is the default position and scale. 
            ZoomToRegion("Overview"); 
        }

        private IEnumerator AnimateZoom(Vector2 targetPosition, float targetScale)
        {
            Vector2 startPos = cityMapRect.anchoredPosition;
            float startScale = cityMapRect.localScale.x;
            float elapsed = 0f;

            while (elapsed < zoomDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / zoomDuration);
                cityMapRect.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
                float scale = Mathf.Lerp(startScale, targetScale, t);
                cityMapRect.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }
            cityMapRect.anchoredPosition = targetPosition;
            cityMapRect.localScale = new Vector3(targetScale, targetScale, 1f);
        }

        /// <summary>
        /// Check if a region is unlocked
        /// </summary>
        private bool IsRegionUnlocked(string regionName)
        {
            var unlockSystem = LifeCraft.Systems.RegionUnlockSystem.Instance;
            if (unlockSystem == null)
            {
                Debug.LogWarning($"IsRegionUnlocked({regionName}): RegionUnlockSystem.Instance is null, defaulting to unlocked");
                return true; // Default to unlocked if system not available
            }

            // Convert region name to RegionType
            var regionType = GetRegionTypeFromName(regionName);
            bool isUnlocked = unlockSystem.IsRegionUnlocked(regionType);
            
            Debug.Log($"IsRegionUnlocked({regionName}): {isUnlocked} (RegionType: {regionType})");
            
            // Also log all unlocked regions for debugging
            var unlockedRegions = unlockSystem.GetUnlockedRegions();
            Debug.Log($"All unlocked regions: {string.Join(", ", unlockedRegions.ConvertAll(r => AssessmentQuizManager.GetRegionDisplayName(r)))}");
            
            return isUnlocked;
        }

        /// <summary>
        /// Convert region name to RegionType
        /// </summary>
        private AssessmentQuizManager.RegionType GetRegionTypeFromName(string regionName)
        {
            switch (regionName)
            {
                case "Health Harbor": return AssessmentQuizManager.RegionType.HealthHarbor;
                case "Mind Palace": return AssessmentQuizManager.RegionType.MindPalace;
                case "Creative Commons": return AssessmentQuizManager.RegionType.CreativeCommons;
                case "Social Square": return AssessmentQuizManager.RegionType.SocialSquare;
                default: return AssessmentQuizManager.RegionType.HealthHarbor;
            }
        }

        /// <summary>
        /// Show locked region popup with unlock requirements
        /// </summary>
        private void ShowLockedRegionPopup(string regionName)
        {
            if (lockedRegionPopup == null || lockedRegionText == null)
                return;

            var regionType = GetRegionTypeFromName(regionName);
            var unlockSystem = LifeCraft.Systems.RegionUnlockSystem.Instance;
            
            if (unlockSystem != null)
            {
                // Find the prerequisite region that needs to be completed first
                var prerequisiteRegion = GetPrerequisiteRegion(regionType);
                if (prerequisiteRegion.HasValue)
                {
                    var prerequisiteRegionData = unlockSystem.GetRegionData(prerequisiteRegion.Value);
                    
                    string message = $"<b>{regionName} is locked!</b>\n\n";
                    message += $"To unlock {regionName}, you must place {prerequisiteRegionData.buildingsRequiredToUnlock} {AssessmentQuizManager.GetRegionDisplayName(prerequisiteRegion.Value)} buildings.\n\n";
                    message += $"Current progress in {AssessmentQuizManager.GetRegionDisplayName(prerequisiteRegion.Value)}:\n";
                    message += $"{prerequisiteRegionData.currentBuildingCount} / {prerequisiteRegionData.buildingsRequiredToUnlock} buildings placed";
                    
                    lockedRegionText.text = message;
                }
                else
                {
                    lockedRegionText.text = $"<b>{regionName} is locked!</b>\n\nThis region will be unlocked as you progress through the game.";
                }
            }
            else
            {
                lockedRegionText.text = $"<b>{regionName} is locked!</b>\n\nThis region is not yet available.";
            }

            lockedRegionPopup.SetActive(true);
        }

        /// <summary>
        /// Get the prerequisite region that needs to be completed to unlock the target region
        /// </summary>
        private AssessmentQuizManager.RegionType? GetPrerequisiteRegion(AssessmentQuizManager.RegionType targetRegion)
        {
            var unlockSystem = LifeCraft.Systems.RegionUnlockSystem.Instance;
            if (unlockSystem == null) return null;

            // Get the unlock order from the system
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

            // Find the target region in the filtered unlock order
            int targetIndex = -1;
            for (int i = 0; i < filteredUnlockOrder.Count; i++)
            {
                if (filteredUnlockOrder[i] == targetRegion)
                {
                    targetIndex = i;
                    break;
                }
            }

            // If target region is not found or is the first region in filtered order, no prerequisite
            if (targetIndex <= 0) return null;

            // Return the region that comes before the target region in filtered unlock order
            return filteredUnlockOrder[targetIndex - 1];
        }

        /// <summary>
        /// Hide the locked region popup
        /// </summary>
        private void HideLockedRegionPopup()
        {
            if (lockedRegionPopup != null)
                lockedRegionPopup.SetActive(false);
        }
    }
}
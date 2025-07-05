using LifeCraft.Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace LifeCraft.UI
{
    /// <summary>
    /// Manages all UI elements and interactions in the game.
    /// Handles resource displays, building UI, and general UI navigation.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Resource Display")]
        [SerializeField] private Transform resourceDisplayContainer;
        [SerializeField] private GameObject resourceDisplayPrefab;
        [SerializeField] private Dictionary<ResourceManager.ResourceType, ResourceDisplay> resourceDisplays;

        [Header("Building UI")]
        [SerializeField] private GameObject buildingPanel;
        [SerializeField] private Transform buildingButtonContainer;
        [SerializeField] private GameObject buildingButtonPrefab;
        [SerializeField] private Button closeBuildingPanelButton;

        [Header("City UI")]
        [SerializeField] private GameObject cityPanel;
        [SerializeField] private Button cityViewButton;
        [SerializeField] private Button dashboardButton;

        [Header("Dashboard UI")]
        [SerializeField] private GameObject dashboardPanel;
        [SerializeField] private Transform habitContainer;
        [SerializeField] private GameObject habitItemPrefab;

        [Header("Notifications")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float notificationDuration = 3f;

        // References
        private ResourceManager resourceManager;
        private CityBuilder cityBuilder;
        private Systems.UnlockSystem unlockSystem;

        private void Awake()
        {
            resourceDisplays = new Dictionary<ResourceManager.ResourceType, ResourceDisplay>();
        }

        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
        }

        /// <summary>
        /// Initialize all UI elements
        /// </summary>
        private void InitializeUI()
        {
            // Get references
            resourceManager = ResourceManager.Instance;
            cityBuilder = FindFirstObjectByType<LifeCraft.Core.CityBuilder>();
            unlockSystem = FindFirstObjectByType<Systems.UnlockSystem>();

            // Setup resource displays
            SetupResourceDisplays();

            // Setup building panel
            SetupBuildingPanel();

            // Setup navigation
            SetupNavigation();

            // Show initial panel
            ShowCityPanel();
        }

        /// <summary>
        /// Setup resource display UI
        /// </summary>
        private void SetupResourceDisplays()
        {
            if (resourceDisplayContainer == null || resourceDisplayPrefab == null)
                return;

            // Clear existing displays
            foreach (Transform child in resourceDisplayContainer)
            {
                Destroy(child.gameObject);
            }

            // Create displays for each resource type
            foreach (ResourceManager.ResourceType resourceType in System.Enum.GetValues(typeof(ResourceManager.ResourceType)))
            {
                GameObject displayObj = Instantiate(resourceDisplayPrefab, resourceDisplayContainer);
                ResourceDisplay display = displayObj.GetComponent<ResourceDisplay>();
                
                if (display != null)
                {
                    display.Initialize(resourceType, resourceManager.GetResourceTotal(resourceType));
                    resourceDisplays[resourceType] = display;
                }
            }
        }

        /// <summary>
        /// Setup building panel with available buildings
        /// </summary>
        private void SetupBuildingPanel()
        {
            if (buildingButtonContainer == null || buildingButtonPrefab == null)
                return;

            // Clear existing buttons
            foreach (Transform child in buildingButtonContainer)
            {
                Destroy(child.gameObject);
            }

            // Create buttons for unlocked buildings
            if (unlockSystem != null)
            {
                var unlockedBuildings = unlockSystem.GetUnlockedBuildings();
                foreach (var building in unlockedBuildings)
                {
                    GameObject buttonObj = Instantiate(buildingButtonPrefab, buildingButtonContainer);
                    BuildingButton buildingButton = buttonObj.GetComponent<BuildingButton>();
                    
                    if (buildingButton != null)
                    {
                        buildingButton.Initialize(building, OnBuildingButtonClicked);
                    }
                }
            }
        }

        /// <summary>
        /// Setup navigation between panels
        /// </summary>
        private void SetupNavigation()
        {
            if (cityViewButton != null)
                cityViewButton.onClick.AddListener(ShowCityPanel);
            
            if (dashboardButton != null)
                dashboardButton.onClick.AddListener(ShowDashboardPanel);
            
            if (closeBuildingPanelButton != null)
                closeBuildingPanelButton.onClick.AddListener(HideBuildingPanel);
        }

        /// <summary>
        /// Setup event listeners
        /// </summary>
        private void SetupEventListeners()
        {
            if (resourceManager != null)
            {
                resourceManager.OnResourceUpdated.AddListener(OnResourceUpdated);
            }
        }

        /// <summary>
        /// Handle resource updates
        /// </summary>
        private void OnResourceUpdated(ResourceManager.ResourceType resourceType, int newAmount)
        {
            if (resourceDisplays.TryGetValue(resourceType, out ResourceDisplay display))
            {
                display.UpdateAmount(newAmount);
            }
        }

        /// <summary>
        /// Handle building button clicks
        /// </summary>
        private void OnBuildingButtonClicked(string buildingType)
        {
            HideBuildingPanel();
            
            // TODO: Enter building placement mode
            Debug.Log($"Selected building: {buildingType}");
        }

        /// <summary>
        /// Show city panel
        /// </summary>
        public void ShowCityPanel()
        {
            if (cityPanel != null)
                cityPanel.SetActive(true);
            
            if (dashboardPanel != null)
                dashboardPanel.SetActive(false);
        }

        /// <summary>
        /// Show dashboard panel
        /// </summary>
        public void ShowDashboardPanel()
        {
            if (dashboardPanel != null)
                dashboardPanel.SetActive(true);
            
            if (cityPanel != null)
                cityPanel.SetActive(false);
        }

        /// <summary>
        /// Show building panel
        /// </summary>
        public void ShowBuildingPanel()
        {
            if (buildingPanel != null)
                buildingPanel.SetActive(true);
        }

        /// <summary>
        /// Hide building panel
        /// </summary>
        public void HideBuildingPanel()
        {
            if (buildingPanel != null)
                buildingPanel.SetActive(false);
        }

        /// <summary>
        /// Show notification
        /// </summary>
        public void ShowNotification(string message)
        {
            if (notificationPanel != null && notificationText != null)
            {
                notificationText.text = message;
                notificationPanel.SetActive(true);
                
                // Auto-hide after duration
                Invoke(nameof(HideNotification), notificationDuration);
            }
        }

        /// <summary>
        /// Hide notification
        /// </summary>
        public void HideNotification()
        {
            if (notificationPanel != null)
                notificationPanel.SetActive(false);
        }

        /// <summary>
        /// Refresh building panel
        /// </summary>
        public void RefreshBuildingPanel()
        {
            SetupBuildingPanel();
        }

        /// <summary>
        /// Update habit display
        /// </summary>
        public void UpdateHabitDisplay(List<HabitData> habits)
        {
            if (habitContainer == null || habitItemPrefab == null)
                return;

            // Clear existing habits
            foreach (Transform child in habitContainer)
            {
                Destroy(child.gameObject);
            }

            // Create habit items
            /**foreach (var habit in habits)
            {
                GameObject habitObj = Instantiate(habitItemPrefab, habitContainer);
                HabitItem habitItem = habitObj.GetComponent<HabitItem>();
                
                if (habitItem != null)
                {
                    habitItem.Initialize(habit);
                }
            }**/
        }

        /// <summary>
        /// Get resource display for a specific resource type
        /// </summary>
        public ResourceDisplay GetResourceDisplay(ResourceManager.ResourceType resourceType)
        {
            return resourceDisplays.TryGetValue(resourceType, out ResourceDisplay display) ? display : null;
        }

        /// <summary>
        /// Set UI interactable state
        /// </summary>
        public void SetUIInteractable(bool interactable)
        {
            // Set all UI elements interactable state
            var buttons = FindObjectsByType<Button>(FindObjectsSortMode.None); 
            foreach (var button in buttons)
            {
                button.interactable = interactable;
            }
        }
    }

    /// <summary>
    /// Data structure for habit information
    /// </summary>
    [System.Serializable]
    public class HabitData
    {
        public string habitName;
        public string displayName;
        public bool isCompleted;
        public Sprite icon;
    }
} 
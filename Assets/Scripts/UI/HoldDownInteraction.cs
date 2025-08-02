using UnityEngine;
using UnityEngine.UI;
using LifeCraft.Core;
using LifeCraft.Systems;
using LifeCraft.Buildings;

namespace LifeCraft.UI
{
    /// <summary>
    /// Handles action menu interactions for placed buildings and decorations.
    /// Shows action menu with 5 options: Store, Sell, Confirm, Rotate, Reset.
    /// Called from PlacedItemUI when in edit mode.
    /// </summary>
    public class HoldDownInteraction : MonoBehaviour
    {
        // Hold configuration removed - no longer needed
        
        [Header("Action Menu")]
        public GameObject actionMenuPrefab;
        [Tooltip("Enter the exact name of the parent GameObject (e.g., 'ActionMenu_Parent')")]
        public string actionMenuParentName = "ActionMenu_Parent";
        
        // Cache for the parent transform
        private Transform cachedActionMenuParent;
        
        [Header("Item Data")]
        public bool isBuilding = true; // Whether this is a building (has construction time) or decoration
        public string itemName;
        public Vector3 originalPosition;
        public Quaternion originalRotation;
        public int originalCost;
        public ResourceManager.ResourceType originalCurrency;
        public string regionType;
        
        // Hold-down variables removed - no longer needed
        private GameObject currentActionMenu;
        private ActionMenuUI actionMenuUI;
        
        private void Start()
        {
            Debug.Log($"HoldDownInteraction Start() called on {gameObject.name}");
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            InitializeItemData();
            
            // Ensure we have the required components for pointer events
            var graphic = GetComponent<Graphic>();
            if (graphic == null)
            {
                Debug.LogWarning($"HoldDownInteraction on {gameObject.name} has no Graphic component. Adding Image component for pointer events.");
                var image = gameObject.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0); // Transparent
            }
        }
        
        /// <summary>
        /// Initialize item data from existing components
        /// </summary>
        private void InitializeItemData()
        {
            // Try to get data from PlacedItemUI
            var placedItemUI = GetComponent<PlacedItemUI>();
            if (placedItemUI != null)
            {
                var decorationItem = placedItemUI.GetDecorationItem();
                if (decorationItem != null)
                {
                    itemName = decorationItem.displayName;
                    // Decorations don't have cost/currency in the current system
                    originalCost = 0;
                    originalCurrency = ResourceManager.ResourceType.EnergyCrystals; // Default
                    regionType = decorationItem.region.ToString();
                    // Check if this is a building by looking at the region type
                    isBuilding = (decorationItem.region != RegionType.Decoration);
                    Debug.Log($"HoldDownInteraction private InitializeItemData: {itemName} isBuilding={isBuilding} (region={decorationItem.region})");
                }
            }
            
            // Try to get data from Building component
            var building = GetComponent<Building>();
            if (building != null)
            {
                itemName = building.BuildingName; // Use property instead of method
                isBuilding = true;
                
                // Get building data from CityBuilder
                if (CityBuilder.Instance != null)
                {
                    var buildingData = CityBuilder.Instance.GetBuildingTypeData(itemName);
                    if (buildingData != null)
                    {
                        originalCost = buildingData.costAmount;
                        originalCurrency = buildingData.costResource;
                    }
                }
            }
        }
        
        /// <summary>
        /// Public method to initialize item data from CityBuilder
        /// </summary>
        public void InitializeItemData(bool isBuildingItem, string itemName, int cost, ResourceManager.ResourceType currency)
        {
            this.isBuilding = isBuildingItem;
            this.itemName = itemName;
            this.originalCost = cost;
            this.originalCurrency = currency;
            
            Debug.Log($"HoldDownInteraction initialized: isBuilding={isBuildingItem}, itemName={itemName}, cost={cost}, currency={currency}");
        }
        
        // Hold-down functionality removed - action menu is now shown immediately in edit mode via PlacedItemUI.OnPointerClick
        
        /// <summary>
        /// Show the action menu for this item
        /// </summary>
        public void ShowActionMenu()
        {
            Debug.Log($"ShowActionMenu called for {gameObject.name}");
            HideActionMenu();
            
            if (actionMenuPrefab != null)
            {
                Debug.Log($"Action menu prefab is assigned: {actionMenuPrefab.name}");
                
                if (cachedActionMenuParent == null)
                {
                    // Find the parent in the Canvas hierarchy
                    Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                    foreach (Canvas canvas in canvases)
                    {
                        Transform parent = canvas.transform.Find(actionMenuParentName);
                        if (parent != null)
                        {
                            cachedActionMenuParent = parent;
                            Debug.Log($"Found action menu parent: {parent.name}");
                            break;
                        }
                    }
                }
                
                if (cachedActionMenuParent != null)
                {
                    currentActionMenu = Instantiate(actionMenuPrefab, cachedActionMenuParent);
                    
                    // Position the action menu at the mouse position
                    var rectTransform = currentActionMenu.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        // Use the new Input System
                        Vector3 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
                        rectTransform.position = mousePos;
                        
                        // Make sure the action menu is visible and on top
                        currentActionMenu.SetActive(true);
                        
                        // Set the canvas to be on top
                        var canvas = currentActionMenu.GetComponent<Canvas>();
                        if (canvas != null)
                        {
                            canvas.sortingOrder = 999;
                        }
                        
                        // Also ensure the parent canvas is on top
                        var parentCanvas = cachedActionMenuParent.GetComponentInParent<Canvas>();
                        if (parentCanvas != null)
                        {
                            parentCanvas.sortingOrder = 998;
                        }
                        
                        Debug.Log($"Action menu positioned at {mousePos} and set active");
                    }
                    
                    actionMenuUI = currentActionMenu.GetComponent<ActionMenuUI>();
                    if (actionMenuUI != null)
                    {
                        actionMenuUI.Initialize(this);
                        Debug.Log($"Action menu created and initialized for {gameObject.name}");
                    }
                    else
                    {
                        Debug.LogError("ActionMenuUI component not found on actionMenuPrefab!");
                    }
                }
                else
                {
                    Debug.LogError($"Action menu parent '{actionMenuParentName}' not found in any Canvas!");
                }
            }
            else
            {
                Debug.LogError($"Action menu prefab is not assigned on {gameObject.name}! Please assign it in the Inspector.");
            }
        }
        
        /// <summary>
        /// Hide the action menu
        /// </summary>
        public void HideActionMenu()
        {
            if (currentActionMenu != null)
            {
                Destroy(currentActionMenu);
                currentActionMenu = null;
                actionMenuUI = null;
            }
        }
        
        /// <summary>
        /// Store item back to inventory
        /// </summary>
        public void StoreToInventory()
        {
            // Get inventory manager
            var inventoryManager = InventoryManager.Instance;
            if (inventoryManager == null)
            {
                Debug.LogError("InventoryManager not found!");
                return;
            }
            
            // Create decoration item for inventory
            var decorationItem = new DecorationItem(itemName, "", DecorationRarity.Common, false);
            
            // Add to inventory
            inventoryManager.AddDecoration(decorationItem);
            
            // Remove from world
            RemoveFromWorld();
            
            Debug.Log($"Stored {itemName} to inventory");
        }
        
        /// <summary>
        /// Sell item for 50% of original cost
        /// </summary>
        public void SellItem()
        {
            int sellAmount = Mathf.RoundToInt(originalCost * 0.5f);
            
            if (isBuilding)
            {
                // Buildings sell for their original currency
                ResourceManager.Instance.AddResources(originalCurrency, sellAmount);
                Debug.Log($"Sold {itemName} for {sellAmount} {originalCurrency}");
                RemoveFromWorld();
            }
            else
            {
                // Decorations sell for random region currency
                var randomCurrency = GetRandomRegionCurrency();
                ResourceManager.Instance.AddResources(randomCurrency, sellAmount);
                Debug.Log($"Sold {itemName} for {sellAmount} {randomCurrency}");
                RemoveFromWorld();
            }
        }
        
        /// <summary>
        /// Confirm the current position (finalize placement)
        /// </summary>
        public void ConfirmPosition()
        {
            Debug.Log($"ConfirmPosition called for {gameObject.name}");
            
            // Store the current position as the finalized position
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            
            Debug.Log($"Position finalized for {gameObject.name} at {originalPosition}");
        }
        
        /// <summary>
        /// Rotate item 90 degrees
        /// </summary>
        public void RotateItem()
        {
            transform.Rotate(0, 0, 90);
            Debug.Log($"Rotated {itemName}");
        }
        
        /// <summary>
        /// Reset to the previous finalized position
        /// </summary>
        public void ResetPosition()
        {
            Debug.Log($"ResetPosition called for {gameObject.name}");
            
            // Reset to the last finalized position
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            
            Debug.Log($"Position reset for {gameObject.name} to {originalPosition}");
        }
        
        /// <summary>
        /// Get random region currency for decoration sales
        /// </summary>
        private ResourceManager.ResourceType GetRandomRegionCurrency()
        {
            var currencies = new ResourceManager.ResourceType[]
            {
                ResourceManager.ResourceType.EnergyCrystals,
                ResourceManager.ResourceType.WisdomOrbs,
                ResourceManager.ResourceType.HeartTokens,
                ResourceManager.ResourceType.CreativitySparks
            };
            
            int randomIndex = Random.Range(0, currencies.Length);
            return currencies[randomIndex];
        }
        
        /// <summary>
        /// Remove item from world
        /// </summary>
        private void RemoveFromWorld()
        {
            // Notify CityBuilder to remove from tracking
            if (CityBuilder.Instance != null)
            {
                Vector3Int gridPosition = CityBuilder.Instance.WorldToMap(transform.position);
                CityBuilder.Instance.RemoveBuilding(gridPosition);
            }
            
            // Destroy the GameObject
            Destroy(gameObject);
        }
        
        // Update method removed - no longer needed since hold-down functionality was removed
        
        /// <summary>
        /// Get item name
        /// </summary>
        public string GetItemName()
        {
            return itemName;
        }
        
        /// <summary>
        /// Check if this is a building
        /// </summary>
        public bool IsBuilding()
        {
            return isBuilding;
        }
    }
} 
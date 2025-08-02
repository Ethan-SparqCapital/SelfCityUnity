using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LifeCraft.Systems;
using LifeCraft.Core;

namespace LifeCraft.UI
{
    /// <summary>
    /// Handles click/removal logic for placed items.
    /// </summary>
    public class PlacedItemUI : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private DecorationItem _decorationItem;
        private bool isDragging = false;
        private Vector3 dragOffset;
        private Vector3 originalDragPosition;
        private bool isFirstPlacement = true; // Track if this is the first time being placed
        private float dragStartTime;

        [Header("EXP Reward Configuration")]
        [SerializeField] public int baseBuildingEXP = 10; // Base EXP for placing a building. 
        [SerializeField] public int decorationEXP = 5; // EXP for placing a decoration. 
        [SerializeField] public float expMultiplier = 1.2f; // Multiplier for EXP based on unlock level. 
        
        // Add debug component for troubleshooting
        private void Start()
        {
            if (GetComponent<PlacedItemDebugger>() == null)
            {
                gameObject.AddComponent<PlacedItemDebugger>();
            }
            
            // Manually initialize HoldDownInteraction if it wasn't done properly
            InitializeHoldDownInteraction();
        }
        
        /// <summary>
        /// Manually initialize the HoldDownInteraction component for existing buildings
        /// </summary>
        private void InitializeHoldDownInteraction()
        {
            var holdDownInteraction = GetComponent<HoldDownInteraction>();
            if (holdDownInteraction != null && _decorationItem != null)
            {
                Debug.Log($"Manually initializing HoldDownInteraction for {_decorationItem.displayName}");
                
                // Check if this is a building by looking at the region type
                bool isBuilding = (_decorationItem.region != RegionType.Decoration);
                
                // For buildings, we'll set a default construction time based on the region
                int constructionTimeMinutes = 60; // Default 1 hour
                if (isBuilding)
                {
                    // Set construction time based on region
                    switch (_decorationItem.region)
                    {
                        case RegionType.HealthHarbor:
                            constructionTimeMinutes = 60; // 1 hour
                            break;
                        case RegionType.MindPalace:
                            constructionTimeMinutes = 90; // 1.5 hours
                            break;
                        case RegionType.CreativeCommons:
                            constructionTimeMinutes = 120; // 2 hours
                            break;
                        case RegionType.SocialSquare:
                            constructionTimeMinutes = 150; // 2.5 hours
                            break;
                        default:
                            constructionTimeMinutes = 60; // Default
                            break;
                    }
                }
                
                holdDownInteraction.InitializeItemData(isBuilding, _decorationItem.displayName, 0, ResourceManager.ResourceType.EnergyCrystals);
                Debug.Log($"Manually initialized HoldDownInteraction for {_decorationItem.displayName}: isBuilding={isBuilding}");
                
                // If this is a building, start construction directly
                if (isBuilding)
                {
                    Debug.Log($"Manually starting construction for {_decorationItem.displayName} with {constructionTimeMinutes} minutes");
                    
                    // Find the ConstructionTimeUI_Prefab and start construction
                    GameObject constructionUIPrefab = null;
                    Transform popupParent = GameObject.Find("PopupParent")?.transform;
                    if (popupParent != null)
                    {
                        Transform constructionParent = popupParent.Find("ConstructionUI_Parent");
                        if (constructionParent != null)
                        {
                            constructionUIPrefab = constructionParent.Find("ConstructionTimeUI_Prefab")?.gameObject;
                        }
                    }
                    
                    if (constructionUIPrefab != null)
                    {
                        var constructionUI = constructionUIPrefab.GetComponent<ConstructionTimeUI>();
                        if (constructionUI != null)
                        {
                            // Use the placed item's position as a unique identifier
                            Vector3Int gridPosition = new Vector3Int(
                                Mathf.RoundToInt(transform.position.x),
                                Mathf.RoundToInt(transform.position.y),
                                0
                            );
                            
                            constructionUI.StartConstruction(_decorationItem.displayName, gridPosition, constructionTimeMinutes, _decorationItem.region.ToString());
                            Debug.Log($"Manually started construction for {_decorationItem.displayName} at {gridPosition}");
                        }
                        else
                        {
                            Debug.LogError("ConstructionTimeUI component not found on prefab");
                        }
                    }
                    else
                    {
                        Debug.LogError("ConstructionTimeUI_Prefab not found");
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the placed item with its DecorationItem data.
        /// Call this right after instantiating the placed item.
        /// </summary>
        public void Initialize(DecorationItem item)
        {
            _decorationItem = item;

            // Get the type for the item (decoration or region, and specific region):
            var itemType = _decorationItem.region;

            // Reward 5 EXP if placing a decoration:
            if (itemType == RegionType.Decoration && PlayerLevelManager.Instance != null)
            {
                PlayerLevelManager.Instance.AddEXP(decorationEXP); // Reward 5 EXP (decorationEXP) for placing a decoration. 
                
                // Show EXP popup animation
                if (EXPPopupManager.Instance != null)
                {
                    Vector3 buildingPosition = transform.position;
                    EXPPopupManager.Instance.ShowEXPPopup(decorationEXP, buildingPosition, QuestDifficulty.Easy);
                }
            }

            // Calculate and reward EXP based on unlock level if placing a building:
            if (itemType == RegionType.HealthHarbor || itemType == RegionType.MindPalace ||
                itemType == RegionType.CreativeCommons || itemType == RegionType.SocialSquare && 
                PlayerLevelManager.Instance != null)
            {
                // Calculate EXP based on the unlock level: EXP = baseBuildingEXP * (expMultiplier ^ (unlockLevel - 1))
                int expReward = baseBuildingEXP * Mathf.RoundToInt(Mathf.Pow(expMultiplier, PlayerLevelManager.Instance.GetBuildingUnlockLevel(_decorationItem.displayName) - 1)); // DecorationItem has displayName for the building name. 

                PlayerLevelManager.Instance.AddEXP(expReward); // Reward calculated EXP for placing a building. 
                
                // Show EXP popup animation
                if (EXPPopupManager.Instance != null)
                {
                    Vector3 buildingPosition = transform.position;
                    EXPPopupManager.Instance.ShowEXPPopup(expReward, buildingPosition, QuestDifficulty.Medium);
                }
            }
        }

        /// <summary>
        /// Sets the sprite for this item (legacy method - now handled by direct assignment).
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            Debug.LogError($"=== LEGACY SETSPRITE CALLED ON {gameObject.name} ===");
            Debug.LogError($"This method is deprecated - sprite assignment is now handled directly in DraggableInventoryItem");
            
            // This method is kept for compatibility but should not be used
            // The actual sprite assignment now happens directly in DraggableInventoryItem.OnEndDrag()
        }

        /// <summary>
        /// Show the action menu for this item
        /// </summary>
        private void ShowActionMenu()
        {
            var holdDownInteraction = GetComponent<HoldDownInteraction>();
            if (holdDownInteraction != null)
            {
                holdDownInteraction.ShowActionMenu();
            }
            else
            {
                Debug.LogError($"HoldDownInteraction component not found on {gameObject.name}");
            }
        }
        
        /// <summary>
        /// Check if this is a building (has construction time)
        /// </summary>
        private bool IsBuilding()
        {
            var decorationItem = GetDecorationItem();
            if (decorationItem != null)
            {
                // If region is not Decoration, it's a building
                return decorationItem.region != RegionType.Decoration;
            }
            return false;
        }
        
        /// <summary>
        /// Get the item name
        /// </summary>
        private string GetItemName()
        {
            var decorationItem = GetDecorationItem();
            if (decorationItem != null)
            {
                return decorationItem.displayName;
            }
            return gameObject.name;
        }
        
        /// <summary>
        /// Handle drag start
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // Only allow dragging in edit mode
            if (RegionEditManager.Instance != null && RegionEditManager.Instance.IsEditModeActive)
            {
                isDragging = true;
                dragStartTime = Time.time;
                originalDragPosition = transform.position;
                
                // Calculate drag offset in world space
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
                dragOffset = transform.position - worldPos;
                dragOffset.z = 0;
                
                Debug.Log($"Started dragging {gameObject.name} from {originalDragPosition}");
            }
        }
        
        /// <summary>
        /// Handle drag
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                // Update position based on mouse/touch position
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
                Vector3 newPos = worldPos + dragOffset;
                
                // Keep z position constant
                newPos.z = originalDragPosition.z;
                
                // Snap to grid during drag
                Vector3 snappedPos = new Vector3(
                    Mathf.Round(newPos.x),
                    Mathf.Round(newPos.y),
                    newPos.z
                );
                
                transform.position = snappedPos;
                
                Debug.Log($"Dragging {gameObject.name} to snapped position {snappedPos}");
            }
        }
        
        /// <summary>
        /// Handle drag end
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (isDragging)
            {
                isDragging = false;
                
                // Snap to grid
                Vector3 snappedPos = new Vector3(
                    Mathf.Round(transform.position.x),
                    Mathf.Round(transform.position.y),
                    transform.position.z
                );
                transform.position = snappedPos;
                
                // Mark that this is no longer the first placement
                isFirstPlacement = false;
                
                Debug.Log($"Finished dragging {gameObject.name} to {snappedPos}. First placement: {isFirstPlacement}");
            }
        }

        /// <summary>
        /// Called when the placed item is clicked.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"PlacedItemUI OnPointerClick() called on {gameObject.name}");
            
            // Simple check: if we were dragging, don't treat as a click
            if (isDragging)
            {
                Debug.Log("Ignoring click because we were dragging");
                return;
            }
            
            // Check if we're in edit mode
            if (RegionEditManager.Instance != null && RegionEditManager.Instance.IsEditModeActive)
            {
                // EDIT MODE: Show action menu immediately
                Debug.Log("Edit mode active - showing action menu immediately");
                ShowActionMenu();
                return;
            }
            
            // NORMAL MODE: Check for construction time (buildings only)
            if (IsBuilding())
            {
                // Check if this building has a construction timer
                var constructionTimer = GetComponent<BuildingConstructionTimer>();
                if (constructionTimer != null && constructionTimer.IsUnderConstruction())
                {
                    Debug.Log($"Building {GetItemName()} is under construction");
                    // The construction timer is already visible on the building
                    return;
                }
            }
            
            Debug.Log("No construction time to show and not in edit mode");
        }
        
        /// <summary>
        /// Get the decoration item data
        /// </summary>
        public DecorationItem GetDecorationItem()
        {
            return _decorationItem;
        }

        /// <summary>
        /// Returns the item to the inventory and removes it from the grid.
        /// </summary>
        private void ReturnToInventory()
        {
            if (_decorationItem != null && InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddDecoration(_decorationItem);
            }
            // --- NEW: Remove the placed item from CityBuilder for persistent saving ---
            if (LifeCraft.Core.CityBuilder.Instance != null)
            {
                LifeCraft.Core.CityBuilder.Instance.RemovePlacedItem(transform.position);
            }
            Destroy(gameObject);

            // Save the city layout immediately after removal to ensure persistence even if the game is closed or crashes.
            if (LifeCraft.Core.GameManager.Instance != null)
            {
                LifeCraft.Core.GameManager.Instance.SaveGame();
            }
        }
    }
}
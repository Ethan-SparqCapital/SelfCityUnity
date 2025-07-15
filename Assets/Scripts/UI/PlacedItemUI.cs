using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // For Image component. Make sure to add this if you use UI images.
using LifeCraft.Core; // For InventoryManager and DecorationItem

namespace LifeCraft.UI
{
    /// <summary>
    /// Handles click/removal logic for a placed item in the city grid.
    /// When removed, returns the item to the player's inventory.
    /// </summary>
    public class PlacedItemUI : MonoBehaviour, IPointerClickHandler
    {
        private DecorationItem _decorationItem;

        /// <summary>
        /// Initialize the placed item with its DecorationItem data.
        /// Call this right after instantiating the placed item.
        /// </summary>
        public void Initialize(DecorationItem item)
        {
            _decorationItem = item;
        }

        /// <summary>
        /// Sets the sprite for this item, using SpriteRenderer if present, otherwise UI Image.
        /// Only one will be enabled at a time.
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            // Try SpriteRenderer first (for world objects)
            var sr = GetComponent<SpriteRenderer>();
            var img = GetComponent<Image>();

            if (sr != null && sprite != null)
            {
                sr.sprite = sprite;
                sr.enabled = true;
                if (img != null) img.enabled = false; // Hide UI image if present
            }
            else if (img != null && sprite != null)
            {
                img.sprite = sprite;
                img.enabled = true;
                if (sr != null) sr.enabled = false; // Hide SpriteRenderer if present
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: No SpriteRenderer or UI Image found to set sprite!");
            }
        }

        /// <summary>
        /// Called when the placed item is clicked.
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Only allow removal if the player is in edit mode:
            if (RegionEditManager.Instance != null && RegionEditManager.Instance.IsEditModeActive)
            {
                ReturnToInventory();
            }
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
using UnityEngine;
using LifeCraft.Core; // Reference to the Core namespace for ResourceManager, which deals with resources. 

public class DecorChestManager : MonoBehaviour
{
    [Header("References")]
    public PrizePoolManager prizePoolManager; // Add the PrizePoolManager field to the Inspector. This will be used to access the prize pools for decorations. 

    // Call this from the Decor Chest button (for free & premium players)
    public void OpenDecorChest()
    {
        if (prizePoolManager == null)
        {
            Debug.LogError("PrizePoolManager reference is missing!");
            return;
        }

        // Check if the player has Balance Tickets:
        int tickets = ResourceManager.Instance.GetResourceTotal(ResourceManager.ResourceType.BalanceTickets); // Get the total number of Balance Tickets the player has using the ResourceManager. 
        if (tickets < 1)
        {
            Debug.LogWarning("Not enough Balance tickets to open a Decor Chest!");
            // TODO: Show UI warning to the player that they need more Balance Tickets to open a Decor chest. 
            return; 
        }

        // If the player has enough Balance Tickets, spend one ticket:
        ResourceManager.Instance.SpendResources(ResourceManager.ResourceType.BalanceTickets, 1); // Spend one Balance Ticket using the ResourceManager.

        string reward = prizePoolManager.GetRandomFreeAndPremiumReward();
        if (!string.IsNullOrEmpty(reward))
        {
            Debug.Log("Player won from Decor Chest: " + reward);
            // TODO: Grant the decoration to the player here (add to inventory, show popup, etc.)
        }
        else
        {
            Debug.LogWarning("No decorations available in today's All: prize pool.");
        }
    }

    // Call this from the Premium Decor Chest button (for premium only)
    public void OpenPremiumDecorChest()
    {
        if (prizePoolManager == null)
        {
            Debug.LogError("PrizePoolManager reference is missing!");
            return;
        }

        // Check if the player has Balance tickets:
        int tickets = ResourceManager.Instance.GetResourceTotal(ResourceManager.ResourceType.BalanceTickets); // Get the total number of Balance Tickets the player has using the ResourceManager. 
        if (tickets < 1)
        {
            Debug.LogWarning("Not enough Balance Tickets to open a Premium Decor Chest!");
            // TODO: Show UI warning to the player that they need more Balance Tickets to open a Premium Decor chest. 
            return; 
        }

        // If the player has enough Balance Tickets, spend one ticket:
        ResourceManager.Instance.SpendResources(ResourceManager.ResourceType.BalanceTickets, 1); // Spend one Balance Ticket using the ResourceManager. 

        string reward = prizePoolManager.GetRandomPremiumOnlyReward();
        if (!string.IsNullOrEmpty(reward))
        {
            Debug.Log("Player won from Premium Decor Chest: " + reward);
            // TODO: Grant the decoration to the player here (add to inventory, show popup, etc.)
        }
        else
        {
            Debug.LogWarning("No decorations available in today's Premium Only: prize pool.");
        }
    }
} 
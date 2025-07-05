// AI Cursor + Ethan Le code with comments and explanations for code comprehension. Language: C# (C Sharp)

using UnityEngine;

namespace LifeCraft.UI
{
    public class ResourceBarManager : MonoBehaviour
    {
        [Header("Assign each ResourceDisplay for the five resources")]
        public ResourceDisplay energyCrystalsDisplay; // Add this field to the Inspector to assign the Energy Crystals ResourceDisplay. 
        public ResourceDisplay wisdomOrbsDisplay; // Add this field to the Inspector to assign the Wisdom Orbs ResourceDisplay.
        public ResourceDisplay heartTokensDisplay; // Add this field to the Inspector to assign the Heart Tokens ResourceDisplay.
        public ResourceDisplay creativitySparksDisplay; // Add this field to the Inspector to assign the Creativity Sparks ResourceDisplay.
        public ResourceDisplay balanceTicketsDisplay; // Add this field to the Inspector to assign the Balance Tickets ResourceDisplay.

        // Private variables that track the player's current resource amounts:
        private int energyCrystals = 100;
        private int wisdomOrbs = 100;
        private int heartTokens = 100;
        private int creativitySparks = 100;
        private int balanceTickets = 0;

        void Start()
        {
            // Initialize each display with its resource type and starting amount (100 for now)
            energyCrystalsDisplay.Initialize(LifeCraft.Core.ResourceManager.ResourceType.EnergyCrystals, energyCrystals);
            wisdomOrbsDisplay.Initialize(LifeCraft.Core.ResourceManager.ResourceType.WisdomOrbs, wisdomOrbs);
            heartTokensDisplay.Initialize(LifeCraft.Core.ResourceManager.ResourceType.HeartTokens, heartTokens);
            creativitySparksDisplay.Initialize(LifeCraft.Core.ResourceManager.ResourceType.CreativitySparks, creativitySparks);
            balanceTicketsDisplay.Initialize(LifeCraft.Core.ResourceManager.ResourceType.BalanceTickets, balanceTickets);
        }

        // Call this method to add resource when a quest is COMPLETED: 
        public void AddResource(string regionName, int amount)
        {
            switch (regionName) // Check which region the quest was completed in: 
            {
                case "Health Harbor":
                    energyCrystals += amount; // Add the amount to the Energy Crystals resource. 
                    energyCrystalsDisplay.UpdateAmount(energyCrystals); // Update the display with the new amount. 
                    break;
                case "Mind Palace":
                    wisdomOrbs += amount; // Add the amount to the Wisdom Orbs resource. 
                    wisdomOrbsDisplay.UpdateAmount(wisdomOrbs); // Update the display with the new amount. 
                    break;
                case "Social Square":
                    heartTokens += amount; // Add the amount to the Heart Tokens resource. 
                    heartTokensDisplay.UpdateAmount(heartTokens); // Update the display with the new amount. 
                    break;
                case "Creative Commons":
                    creativitySparks += amount; // Add the amount to the Creativity Sparks resource. 
                    creativitySparksDisplay.UpdateAmount(creativitySparks); // Update the display with the new amount. 
                    break; 
            }
        }

        // Call this method to add Balance Tickets when all daily quests are completed
        public void AddBalanceTickets(int amount)
        {
            balanceTickets += amount; // Add the amount to the Balance Tickets resource.
            balanceTicketsDisplay.UpdateAmount(balanceTickets); // Update the display with the new amount.
        }
    }
} 
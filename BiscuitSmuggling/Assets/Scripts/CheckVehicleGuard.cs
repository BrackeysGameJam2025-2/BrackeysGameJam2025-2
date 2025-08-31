using System.Collections.Generic;
using UnityEngine;

public class CheckVehicleGuard : MonoBehaviour
{
    public enum GuardType
    {
        Fat,
        Veteran,
        WithDog
    }

    [SerializeField] private ItemMetadata illegalItem;
    [SerializeField] private CheckVehicleSystem vehicleSystem;
    private GuardType guardType;
    public GameObject currentHidingSpotToCheck;


    private void Start()
    {

        // Subscribe to the car search event
        GameManager.OnCarSearchRequested += OnCarSearchRequested;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        GameManager.OnCarSearchRequested -= OnCarSearchRequested;
    }

    private void OnCarSearchRequested()
    {
        // Set guard type based on GameManager's current guard type
        if (GameManager.Instance != null)
        {
            GuardsType gameManagerGuardType = GameManager.Instance.GetCurrentGuardType();
            guardType = (GuardType)System.Enum.Parse(typeof(GuardType), gameManagerGuardType.ToString());
            Debug.Log($"CheckVehicleGuard received search request. Guard type set to: {guardType}");

            // Start the check process
            Check();
        }
        else
        {
            Debug.LogError("GameManager instance not found when car search was requested.");
        }
    }

    public void SetGuardType(GuardType type)
    {
        guardType = type;
    }

    public void Check()
    {
        if (vehicleSystem == null || vehicleSystem.hidingSpots == null)
        {
            Debug.LogError("CheckVehicleSystem or hiding spots list is missing.");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager instance not found.");
            return;
        }

        if (CarInventory.Instance == null)
        {
            Debug.LogError("CarInventory instance not found.");
            return;
        }


        // Get the spots that will be checked based on guard type from GameManager
        List<string> spotsToCheckNames = GameManager.Instance.WillBeCheckedSpots;
        if (spotsToCheckNames == null || spotsToCheckNames.Count == 0)
        {
            Debug.LogWarning("No spots marked for checking by GameManager.");
            return;
        }

        // Determine number of spots to check based on guard type
        int maxSpotsToCheck = guardType switch
        {
            GuardType.Fat => 1,
            GuardType.Veteran => 2,
            GuardType.WithDog => vehicleSystem.hidingSpots.Count,
            _ => 0
        };

        // Limit the number of spots to check
        int actualSpotsToCheck = Mathf.Min(maxSpotsToCheck, spotsToCheckNames.Count);
        bool playerFound = false;

        // Check the spots that are marked for inspection
        for (int i = 0; i < actualSpotsToCheck; i++)
        {
            string spotName = spotsToCheckNames[i];
            // Check if this spot has contraband (items) using CarInventory system
            List<CarItem> itemsInSpot = CarInventory.Instance.GetItemsInSpot(spotName);

            bool hasContraband = false;

            if (guardType == GuardType.WithDog)
            {
                foreach (var item in itemsInSpot)
                {
                    if (item.Item != illegalItem)
                    {
                        hasContraband = false;
                        break;
                    }
                }
            }


            foreach (var item in itemsInSpot)
            {
                if (item.Item == illegalItem)
                {
                    hasContraband = true;
                    break;
                }
            }

            if (hasContraband)
            {
                Debug.Log($"Guard found contraband in hiding spot: {spotName}. PLAYER CAUGHT!");
                playerFound = true;
                break; // Player found, no need to check more spots
            }
            else
            {
                Debug.Log($"Guard checked hiding spot: {spotName}. No contraband found.");
            }
        }

        // Determine game result
        if (playerFound)
        {
            Debug.Log("GAME OVER - Player was caught by the guard!");
            SetGameResult(false);
        }
        else
        {
            Debug.Log("SUCCESS - Player was not found by the guard!");
            SetGameResult(true);
        }
    }

    private void SetGameResult(bool playerWon)
    {
        // Since there's no explicit win/lose method in GameManager,
        // we'll use a simple approach to notify about the game result
        if (playerWon)
        {
            Debug.Log("Setting game result: PLAYER WINS");
            // You could trigger an event here or call a method on GameManager
            // For now, just logging the result
        }
        else
        {
            Debug.Log("Setting game result: PLAYER LOSES");
            // You could trigger an event here or call a method on GameManager
            // For now, just logging the result
        }

        // The BorderGuard.Reject() method will handle the final outcome
        // based on dialog variables set during the interaction
    }
}

using UnityEngine;

public class HidingSpotScript : MonoBehaviour
{
    public bool IsThisHidingSpotChosen; // Keep for backward compatibility
    public bool IsThisHidingSpotChecked; // Keep for backward compatibility

    [SerializeField] private CheckVehicleGuard guard;

    public void OnPlayerClick()
    {
        // Mark as chosen for backward compatibility
        IsThisHidingSpotChosen = true;

        // The actual player interaction should now work through the CarInventory system
        // Items should be added to the CarInventory when player hides contraband
        Debug.Log($"Player interacted with hiding spot: {gameObject.name}");
        
        // Note: In the updated system, contraband detection is handled through CarInventory
        // rather than the IsThisHidingSpotChosen boolean
        
        if (guard == null)
        {
            Debug.LogError("CheckVehicleGuard reference is missing.");
            return;
        }

        // Notify the guard to check hiding spots
        guard.Check();
    }
}

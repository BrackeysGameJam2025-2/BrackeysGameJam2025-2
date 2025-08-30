using UnityEngine;

public class HidingSpotScript : MonoBehaviour
{
    public bool IsThisHidingSpotChosen;
    public bool IsThisHidingSpotChecked;

    [SerializeField] private CheckVehicleGuard guard;

    public void OnPlayerClick()
    {
        IsThisHidingSpotChosen = true;

        if (guard == null)
        {
            Debug.LogError("CheckVehicleGuard reference is missing.");
            return;
        }

        // Notify the guard to check this hiding spot
        guard.QueueCheckedHidingSpots(); 
    }
}

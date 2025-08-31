using System.Collections.Generic;
using UnityEngine;
public class CheckVehicleSystem : MonoBehaviour
{
    [Tooltip("List of hiding spots to choose from.")]
    public List<Transform> hidingSpots;


    private Dictionary<Transform, string> hidingSpotNameMap;

    private void Start()
    {
        InitializeHidingSpotNameMap();
    }

    private void InitializeHidingSpotNameMap()
    {
        hidingSpotNameMap = new Dictionary<Transform, string>();

        // Map hiding spots to their names based on GameManager's hidden spots
        var hiddenSpotNames = GameManager.Instance.HiddenSpots;

        foreach (var hidingSpot in hidingSpots)
        {
            string spotName = hidingSpot.name;
            if (hiddenSpotNames.Contains(spotName))
            {
                hidingSpotNameMap[hidingSpot] = spotName;
            }
        }

        if (hidingSpotNameMap.Count == 0)
        {
            Debug.LogWarning("No hiding spots match the hidden spots from GameManager.");
        }
    }
}



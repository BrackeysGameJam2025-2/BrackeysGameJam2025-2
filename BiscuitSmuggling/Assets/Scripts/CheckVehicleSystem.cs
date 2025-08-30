using System.Collections.Generic;
using UnityEngine;
public class CheckVehicleSystem : MonoBehaviour
{
    [Tooltip("List of hiding spots to choose from.")]
    public List<Transform> hidingSpots;

    [Tooltip("Custom names for each hiding spot.")]
    public List<string> hidingSpotNames;

    [Tooltip("Chance of checking two hiding spots (0 to 1).")]
    [Range(0f, 1f)]
    public float chanceToCheckTwoSpots = 0.3f;

    private Dictionary<Transform, string> hidingSpotNameMap;

    private void Start()
    {
        InitializeHidingSpotNameMap();
        ChooseHidingSpots();
    }

    private void InitializeHidingSpotNameMap()
    {
        hidingSpotNameMap = new Dictionary<Transform, string>();

        if (hidingSpots.Count != hidingSpotNames.Count)
        {
            Debug.LogError("The number of hiding spots and hiding spot names must match.");
            return;
        }

        for (int i = 0; i < hidingSpots.Count; i++)
        {
            hidingSpotNameMap[hidingSpots[i]] = hidingSpotNames[i];
        }
    }

    private void ChooseHidingSpots()
    {
        if (hidingSpots == null || hidingSpots.Count == 0)
        {
            Debug.LogWarning("No hiding spots available to choose from.");
            return;
        }

        // Determine if one or two hiding spots should be checked
        bool checkTwoSpots = Random.value < chanceToCheckTwoSpots;

        if (checkTwoSpots)
        {
            // Choose two random hiding spots
            Transform firstSpot = hidingSpots[Random.Range(0, hidingSpots.Count)];
            Transform secondSpot;

            // Ensure the second spot is different from the first
            do
            {
                secondSpot = hidingSpots[Random.Range(0, hidingSpots.Count)];
            } while (secondSpot == firstSpot);

            Debug.Log($"Two hiding spots chosen: {hidingSpotNameMap[firstSpot]} and {hidingSpotNameMap[secondSpot]}");
        }
        else
        {
            // Choose one random hiding spot
            Transform chosenSpot = hidingSpots[Random.Range(0, hidingSpots.Count)];
            Debug.Log($"One hiding spot chosen: {hidingSpotNameMap[chosenSpot]}");
        }
    }
}

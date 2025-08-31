using System.Collections.Generic;
using UnityEngine;

public class MoneySpawn : MonoBehaviour
{
    [SerializeField] GameObject MoneyPrefab;
    public List<Transform> MoneySpots;

    [Range(0f, 1f)]
    public float chanceToGet3MoneySpawn = 0.4f;

    private void Start()
    {
        ChooseMoneySpots();
    }

    private void ChooseMoneySpots()
    {
        if (MoneySpots == null || MoneySpots.Count == 0)
        {
            Debug.LogWarning("No money spots available to choose from.");
            return;
        }

        // Determine if one or two hiding spots should be checked
        bool checkTwoSpots = Random.value < chanceToGet3MoneySpawn;

        if (checkTwoSpots)
        {
            // Choose two random hiding spots
            Transform firstSpot = MoneySpots[Random.Range(0, MoneySpots.Count)];
            Transform secondSpot;

            // Ensure the second spot is different from the first
            do
            {
                secondSpot = MoneySpots[Random.Range(0, MoneySpots.Count)];
            } while (secondSpot == firstSpot);

            // Use position property to access x, y, z components
            Instantiate(MoneyPrefab, new Vector3(firstSpot.position.x, firstSpot.position.y, firstSpot.position.z), Quaternion.identity);
            Instantiate(MoneyPrefab, new Vector3(secondSpot.position.x, secondSpot.position.y, secondSpot.position.z), Quaternion.identity);

        }
        else
        {
            // Choose one random hiding spot
            Transform chosenSpot = MoneySpots[Random.Range(0, MoneySpots.Count)];
            Instantiate(MoneyPrefab, new Vector3(chosenSpot.position.x, chosenSpot.position.y, chosenSpot.position.z), Quaternion.identity);
        }
    }
}

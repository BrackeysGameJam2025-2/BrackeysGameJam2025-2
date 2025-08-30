using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CheckVehicleGuard : MonoBehaviour
{
    [SerializeField] private CheckVehicleSystem vehicleSystem;
    public GameObject currentHidingSpotToCheck;

    private NavMeshAgent navMeshAgent;
    private Queue<GameObject> hidingSpotsToCheck = new Queue<GameObject>();

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the guard.");
        }
    }

    public void QueueCheckedHidingSpots()
    {
        if (vehicleSystem == null || vehicleSystem.hidingSpots == null)
        {
            Debug.LogError("CheckVehicleSystem or hiding spots list is missing.");
            return;
        }

        foreach (Transform hidingSpot in vehicleSystem.hidingSpots)
        {
            HidingSpotScript hidingSpotScript = hidingSpot.GetComponent<HidingSpotScript>();
            if (hidingSpotScript != null && hidingSpotScript.IsThisHidingSpotChecked)
            {
                hidingSpotsToCheck.Enqueue(hidingSpot.gameObject);
            }
        }

        // Start processing the queue if there are hiding spots to check
        if (hidingSpotsToCheck.Count > 0)
        {
            StartCoroutine(ProcessHidingSpots());
        }
    }

    private IEnumerator ProcessHidingSpots()
    {
        while (hidingSpotsToCheck.Count > 0)
        {
            currentHidingSpotToCheck = hidingSpotsToCheck.Dequeue();

            // Find the child point of the hiding spot
            Transform checkPoint = currentHidingSpotToCheck.transform.Find("CheckPoint");
            if (checkPoint == null)
            {
                Debug.LogError("CheckPoint child is missing on the hiding spot.");
                continue;
            }

            // Move the guard to the check point
            navMeshAgent.SetDestination(checkPoint.position);

            // Wait until the guard reaches the check point
            while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                yield return null;
            }

            // Wait for 3 seconds at the check point
            yield return new WaitForSeconds(3f);

            // Check if the hiding spot is chosen
            HidingSpotScript hidingSpotScript = currentHidingSpotToCheck.GetComponent<HidingSpotScript>();
            if (hidingSpotScript != null)
            {
                if (hidingSpotScript.IsThisHidingSpotChosen)
                {
                    Debug.Log($"Guard has checked hiding spot: {currentHidingSpotToCheck.name}. It is CHOSEN.");
                }
                else
                {
                    Debug.Log($"Guard has checked hiding spot: {currentHidingSpotToCheck.name}. It is NOT CHOSEN.");
                }
            }
            else
            {
                Debug.LogError($"HidingSpotScript is missing on {currentHidingSpotToCheck.name}.");
            }
        }
    }
}

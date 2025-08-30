using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CheckVehicleGuard : MonoBehaviour
{
    [SerializeField] private CheckVehicleSystem vehicleSystem;
    public GameObject currentHidingSpotToCheck;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on the guard.");
        }
    }

    public void CheckHidingSpot(GameObject hidingSpot)
    {
        if (hidingSpot == null)
        {
            Debug.LogError("Hiding spot is null.");
            return;
        }

        // Set the current hiding spot to check
        currentHidingSpotToCheck = hidingSpot;

        // Find the child point of the hiding spot
        Transform checkPoint = hidingSpot.transform.Find("CheckPoint");
        if (checkPoint == null)
        {
            Debug.LogError("CheckPoint child is missing on the hiding spot.");
            return;
        }

        // Move the guard to the check point
        navMeshAgent.SetDestination(checkPoint.position);

        // Start coroutine to wait and check the hiding spot
        StartCoroutine(WaitAndCheckHidingSpot(checkPoint));
    }

    private IEnumerator WaitAndCheckHidingSpot(Transform checkPoint)
    {
        // Wait until the guard reaches the check point
        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            yield return null;
        }

        // Wait for 3 seconds at the check point
        yield return new WaitForSeconds(3f);

        // Check if the hiding spot is marked as checked
        HidingSpotScript hidingSpotScript = currentHidingSpotToCheck.GetComponent<HidingSpotScript>();
        if (hidingSpotScript == null)
        {
            Debug.LogError("HidingSpotScript is missing on the hiding spot.");
            yield break;
        }

        if (hidingSpotScript.IsThisHidingSpotChecked)
        {
            Debug.Log($"Hiding spot {currentHidingSpotToCheck.name} is chosen.");
        }
        else
        {
            Debug.Log($"Hiding spot {currentHidingSpotToCheck.name} is not chosen.");
        }
    }
}

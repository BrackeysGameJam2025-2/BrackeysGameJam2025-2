using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolNPCScript : MonoBehaviour
{
    public List<Transform> patrolPoints; // List of patrol points
    public float visionRange = 10f; // Vision range of the NPC
    public float visionAngle = 90f; // Vision angle of the NPC
    public Transform player; // Reference to the player
    public LayerMask visionMask; // Layer mask for vision detection
    public MeshFilter visionMeshFilter; // MeshFilter to display the vision area

    private NavMeshAgent agent;
    private int currentPointIndex = 0; // Index of the current patrol point
    private bool isChasing = false;
    private Mesh visionMesh;

    void Start()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogError("No patrol points assigned!");
            return;
        }

        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(patrolPoints[currentPointIndex].position);

        // Initialize the vision mesh
        visionMesh = new Mesh();
        visionMeshFilter.mesh = visionMesh;
    }

    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            CheckForPlayer();
        }

        UpdateVisionMesh();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Move to the next patrol point
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    void CheckForPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= visionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= visionAngle / 2f)
            {
                // Check if there is a clear line of sight to the player
                if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, visionRange, visionMask))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        isChasing = true;
                    }
                }
            }
        }
    }

    void ChasePlayer()
    {
        agent.SetDestination(player.position);

        // Stop chasing if the player is out of vision range
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > visionRange)
        {
            isChasing = false;
            agent.SetDestination(patrolPoints[currentPointIndex].position); // Resume patrol
        }
    }

    void UpdateVisionMesh()
    {
        int rayCount = 50; // Number of rays to draw the vision cone
        float angleStep = visionAngle / rayCount;
        float currentAngle = -visionAngle / 2f;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero; // Origin of the vision cone

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 vertex = direction * visionRange;

            // Check for obstacles
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, visionRange, visionMask))
            {
                vertex = direction * hit.distance;
            }

            vertices[i + 1] = transform.InverseTransformPoint(transform.position + vertex);

            if (i < rayCount)
            {
                int startIndex = i * 3;
                triangles[startIndex] = 0;
                triangles[startIndex + 1] = i + 1;
                triangles[startIndex + 2] = i + 2;
            }

            currentAngle += angleStep;
        }

        visionMesh.Clear();
        visionMesh.vertices = vertices;
        visionMesh.triangles = triangles;
        visionMesh.RecalculateNormals();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Draw vision angle
        Vector3 forward = transform.forward * visionRange;
        Quaternion leftRayRotation = Quaternion.Euler(0, -visionAngle / 2f, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, visionAngle / 2f, 0);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftRayRotation * forward);
        Gizmos.DrawRay(transform.position, rightRayRotation * forward);
    }
}

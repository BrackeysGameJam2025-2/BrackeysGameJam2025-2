using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PatrolNPCScript : MonoBehaviour
{
    private NavMeshAgent _agent;
    private int _currentPointIndex = 0; // Index of the current patrol point
    private bool _isChasing = false;
    private Mesh _visionMesh;

    private Vector3[] _vertices;
    private int[] _triangles;

    [SerializeField]
    [FormerlySerializedAs("patrolPoints")]
    private PatrolPath m_PatrolPath;
    [SerializeField]
    [FormerlySerializedAs("visionRange")]
    private float m_VisionRange = 10f; // Vision range of the NPC
    [SerializeField]
    [FormerlySerializedAs("visionAngle")]
    private float m_VisionAngle = 90f; // Vision angle of the NPC

    [SerializeField]
    [FormerlySerializedAs("playerLayer")]
    private LayerMask m_PlayerLayer; // Layer mask for vision detection
    [SerializeField]
    [FormerlySerializedAs("obstacleLayer")]
    private LayerMask m_ObstacleLayers; // Layers considered as obstacles

    [SerializeField]
    [FormerlySerializedAs("interactiveObject")]
    private InteractiveObject m_InteractiveObject; // Reference to the InteractiveObject component

    [SerializeField]
    [FormerlySerializedAs("visionMeshFilter")]
    private MeshFilter m_VisionMeshFilter; // MeshFilter to display the vision area

    [SerializeField]
    private int m_RaysCount = 50;

    [SerializeField]
    private float m_VisionUpdateRate = 1 / 60f;

    private void Start()
    {
        if (m_PatrolPath == null || m_PatrolPath.Points.Count == 0)
        {
            Debug.LogError("No patrol path is properly assigned!");
            return;
        }

        _vertices = new Vector3[m_RaysCount + 2];
        _triangles = new int[m_RaysCount * 3];

        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(m_PatrolPath.Points[_currentPointIndex]);

        // Initialize the vision mesh
        _visionMesh = new Mesh();
        m_VisionMeshFilter.mesh = _visionMesh;

        UpdateVisionMesh();
        _visionMesh.RecalculateNormals();
        _visionMesh.RecalculateBounds();

        // The nearest point
        _currentPointIndex = m_PatrolPath.FindNearestPointIndex(transform.position);
    }

    private void Update()
    {
        if (_isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            CheckForPlayer();
        }
    }

    private void OnEnable()
    {
        StartCoroutine(VisionUpdate());
    }

    private IEnumerator VisionUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_VisionUpdateRate);

            UpdateVisionMesh();
        }
    }

    private void Patrol()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            // Move to the next patrol point
            _currentPointIndex = (_currentPointIndex + 1) % m_PatrolPath.Points.Count;
            _agent.SetDestination(m_PatrolPath.Points[_currentPointIndex]);
        }
    }

    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = Player.Transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= m_VisionRange)
        {
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            if (angleToPlayer <= m_VisionAngle / 2f)
            {
                Vector3 rayOrigin = transform.position + (Vector3.up * 0.5f);
                Vector3 playerCenter = Player.Transform.position + (Vector3.up * 1f);
                Vector3 rayDirection = (playerCenter - rayOrigin).normalized;
                float rayDistance = Vector3.Distance(rayOrigin, playerCenter);

                // Check for obstacles first
                if (Physics.Raycast(rayOrigin, rayDirection, rayDistance, m_ObstacleLayers))
                {
                    return;
                }

                // Check if we can see the player (no obstacles in the way)
                if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayDistance, m_PlayerLayer))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        _isChasing = true;
                        Debug.Log("Player spotted! Clear line of sight.");
                    }
                }
            }
        }
    }

    private void ChasePlayer()
    {
        _agent.SetDestination(Player.Transform.position);

        // Stop chasing if the player is out of vision range
        Vector3 directionToPlayer = Player.Transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > m_VisionRange)
        {
            _isChasing = false;
            _agent.SetDestination(m_PatrolPath.Points[_currentPointIndex]); // Resume patrol
        }
    }

    private void UpdateVisionMesh()
    {
        float angleStep = m_VisionAngle / m_RaysCount;
        float currentAngle = -m_VisionAngle / 2f;

        _vertices[0] = Vector3.zero; // Origin of the vision cone

        for (int i = 0; i <= m_RaysCount; i++)
        {
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            Vector3 vertex = direction * m_VisionRange;

            // Check for obstacles
            Vector3 rayOrigin = transform.position + (Vector3.up * 0.5f);
            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, m_VisionRange, m_ObstacleLayers | m_PlayerLayer))
            {
                vertex = direction * hit.distance;
            }

            _vertices[i + 1] = transform.InverseTransformPoint(transform.position + vertex);

            if (i < m_RaysCount)
            {
                int startIndex = i * 3;
                _triangles[startIndex] = 0;
                _triangles[startIndex + 1] = i + 1;
                _triangles[startIndex + 2] = i + 2;
            }

            currentAngle += angleStep;
        }

        _visionMesh.SetVertices(_vertices);
        _visionMesh.SetTriangles(_triangles, 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_VisionRange);

        // Draw vision angle
        Vector3 forward = transform.forward * m_VisionRange;
        Quaternion leftRayRotation = Quaternion.Euler(0, -m_VisionAngle / 2f, 0);
        Quaternion rightRayRotation = Quaternion.Euler(0, m_VisionAngle / 2f, 0);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, leftRayRotation * forward);
        Gizmos.DrawRay(transform.position, rightRayRotation * forward);
    }
#endif
}

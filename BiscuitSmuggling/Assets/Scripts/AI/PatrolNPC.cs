using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class PatrolNPC : MonoBehaviour
{
    private Material _visionMaterial;
    private Color _targetVisionColor;

    private static readonly Collider[] s_buffer = new Collider[32];

    private bool _isAlarmed = false;

    private float _communicationTimer;
    private float _bustTimer;
    private float _waitTimer;
    private float _stopSearchTimer = 0f;
    private float _spotPlayerTimer;

    private NavMeshAgent _agent;
    private int _currentPointIndex = 0; // Index of the current patrol point
    private Mesh _visionMesh;

    private Vector3[] _vertices;
    private int[] _triangles;

    [SerializeField]
    private bool m_IgnoreAlarm = false;

    [SerializeField]
    private EventReference m_InRange;
    [SerializeField]
    private EventReference m_Spotted;

    [SerializeField]
    private Color m_DefaultVisionColor;
    [SerializeField]
    private Color m_SearchVisionColor;
    [SerializeField]
    private Color m_ChaseVisionColor;

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
    private float m_NormalSpeed = 3.5f;
    [SerializeField]
    private float m_SearchSpeed = 5f;
    [SerializeField]
    private float m_ChaseSpeed = 6f;

    [SerializeField]
    [Tooltip("How long it takes to spot the player outside of chase.")]
    private float m_TimeToSpotPlayer = 0.8f;
    [SerializeField]
    [Tooltip("For how long to search for the player.")]
    private float m_TimeToStopSearch = 12f;
    [SerializeField]
    [Tooltip("How long to wait during chase if player is out of the vision.")]
    private float m_TimeToStopChase = 2f;
    [SerializeField]
    private float m_CommunicationRadius = 10f;
    [SerializeField]
    private float m_SearchRadius = 15f;
    [SerializeField]
    private float m_BustDistance = 2f;
    [SerializeField]
    private float m_TimeToBust = 0.3f;

    [SerializeField]
    private float m_MinWaitTime = 1f;
    [SerializeField]
    private float m_MaxWaitTime = 3f;

    [SerializeField]
    private float m_AfterChaseCommunicationCooldown = 3f;
    [SerializeField]
    private float m_CommunicationCooldown = 10f;

    [SerializeField]
    [FormerlySerializedAs("playerLayer")]
    private LayerMask m_PlayerLayer; // Layer mask for vision detection
    [SerializeField]
    [FormerlySerializedAs("obstacleLayer")]
    private LayerMask m_ObstacleLayers; // Layers considered as obstacles

    [SerializeField]
    private InteractiveObjectBehavior m_BustBehavior;

    [SerializeField]
    [FormerlySerializedAs("visionMeshFilter")]
    private MeshFilter m_VisionMeshFilter; // MeshFilter to display the vision area

    [SerializeField]
    private int m_RaysCount = 50;

    [SerializeField]
    private float m_AiUpdateRate = 1 / 30f;

    [SerializeField]
    private float m_VisionUpdateRate = 1 / 50f;

    [SerializeField, Range(0f, 1f)]
    private float m_ChanceToCommunicate = 0.25f;

    private AIState _currentState;
    public AIState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState == value) return;

            var oldState = _currentState;
            _currentState = value;

            switch (_currentState)
            {
                case AIState.Patrol:
                    InitiatePatrol();
                    break;
                case AIState.Search:
                    InitiateSearch(); break;
                case AIState.Chase:
                    InitiateChase();
                    break;
                case AIState.Frozen:
                    Freeze();
                    break;
            }

            StateChanged?.Invoke(this, oldState);
        }
    }

    public bool IsAlarmed
    {
        get => _isAlarmed;
        set
        {
            if (m_IgnoreAlarm) return;

            _isAlarmed = value;
            if (_isAlarmed && CurrentState == AIState.Patrol)
            {
                CurrentState = AIState.Search;
            }
        }
    }

    /// <summary>
    /// The second argument is the OLD state!
    /// </summary>
    public event Action<PatrolNPC, AIState> StateChanged;

    public event Action<PatrolNPC, Vector3> SawPlayer;

    private void Start()
    {
        if (!ClankerHivemind.Exists)
        {
            Debug.LogError("AIError: Add the ClankerHivemind prefab.");
            return;
        }

        if (m_PatrolPath == null || m_PatrolPath.Points.Count == 0)
        {
            Debug.LogError("No patrol path is properly assigned!", this);
            return;
        }

        _vertices = new Vector3[m_RaysCount + 2];
        _triangles = new int[m_RaysCount * 3];

        _agent = GetComponent<NavMeshAgent>();
        _agent.SetDestination(m_PatrolPath.Points[_currentPointIndex]);

        // Initialize the vision mesh
        _visionMesh = new Mesh();
        m_VisionMeshFilter.mesh = _visionMesh;
        _visionMaterial = m_VisionMeshFilter.GetComponent<MeshRenderer>().material;
        _visionMaterial.color = _targetVisionColor = m_DefaultVisionColor;

        UpdateVisionMesh();
        _visionMesh.RecalculateNormals();
        _visionMesh.RecalculateBounds();
    }

    private void OnEnable()
    {
        StartCoroutine(VisionUpdate());
        StartCoroutine(AIUpdate());
    }

    private void LateUpdate()
    {
        if (_communicationTimer > 0f)
        {
            _communicationTimer -= Time.deltaTime;
        }

        Color color = _visionMaterial.color;
        Vector4 colorVector = new(color.r, color.g, color.b, color.a);
        Vector4 targetVector = new(_targetVisionColor.r, _targetVisionColor.g, _targetVisionColor.b, _targetVisionColor.a);

        _visionMaterial.color = Vector4.MoveTowards(colorVector, targetVector, 1f / m_TimeToSpotPlayer * Time.deltaTime);
    }

    private IEnumerator AIUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_AiUpdateRate);

            switch (CurrentState)
            {
                case AIState.Patrol:
                    Patrol();
                    break;
                case AIState.Search:
                    Search(); break;
                case AIState.Chase:
                    Chase(); break;
            }
        }
    }

    private IEnumerator VisionUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_VisionUpdateRate);
            UpdateVisionMesh();
        }
    }

    private void InitiatePatrol()
    {
        _couldSeePlayer = false;
        _spotPlayerTimer = 0f;
        _targetVisionColor = m_DefaultVisionColor;

        _agent.speed = m_NormalSpeed;
        _agent.ResetPath();

        // Find the nearest point
        _currentPointIndex = m_PatrolPath.FindNearestPointIndex(transform.position);
    }

    private void InitiateSearch()
    {
        _couldSeePlayer = false;
        _waitTimer = 0f;
        _stopSearchTimer = m_TimeToStopSearch;
        _spotPlayerTimer = 0f;
        _targetVisionColor = m_SearchVisionColor;

        _agent.speed = m_SearchSpeed;
        _agent.ResetPath();
    }

    private void InitiateChase()
    {
        RuntimeManager.PlayOneShot(m_Spotted, transform.position);

        _bustTimer = 0f;
        _waitTimer = 0f;
        _targetVisionColor = m_ChaseVisionColor;

        _agent.speed = m_ChaseSpeed;
        _agent.ResetPath();
    }

    private void Freeze()
    {
        _agent.ResetPath();
        _agent.isStopped = true;
    }

    private bool HasReachedDestination()
    {
        return !_agent.pathPending && _agent.remainingDistance < 0.5f;
    }

    private void Patrol()
    {
        if (HasReachedDestination())
        {
            // Move to the next patrol point
            _currentPointIndex = (_currentPointIndex + 1) % m_PatrolPath.Points.Count;
            _agent.SetDestination(m_PatrolPath.Points[_currentPointIndex]);
        }
        TrySpotPlayer(m_DefaultVisionColor);
    }

    private void Search()
    {
        _stopSearchTimer -= m_AiUpdateRate;
        if (_stopSearchTimer <= 0f && !IsAlarmed)
        {
            CurrentState = AIState.Patrol;
            _communicationTimer = m_CommunicationCooldown;
        }

        if (HasReachedDestination())
        {
            _waitTimer -= m_AiUpdateRate;

            if (_waitTimer <= 0f)
            {
                // Try to find the player
                Vector3 position = ClankerHivemind.Instance.LastPlayerPosition;
                Vector2 circlePoint = UnityEngine.Random.insideUnitCircle * m_SearchRadius;
                Vector3 randomOffset = new(circlePoint.x, 0f, circlePoint.y);
                position += randomOffset;

                if (NavMesh.SamplePosition(position, out var hit, 20f, -1))
                {
                    _agent.SetDestination(hit.position);
                }

                _waitTimer = UnityEngine.Random.Range(m_MinWaitTime, m_MaxWaitTime);
            }
        }
        TrySpotPlayer(m_SearchVisionColor);

        AlarmOthers(AIState.Search);
    }

    private void Chase()
    {
        if (NavMesh.SamplePosition(Player.Transform.position, out var hit, 5f, -1))
        {
            _agent.SetDestination(hit.position);
        }

        if (!CanSeePlayer())
        {
            _waitTimer += m_AiUpdateRate;
            if (_waitTimer >= m_TimeToStopChase)
            {
                _communicationTimer = m_AfterChaseCommunicationCooldown;
                CurrentState = AIState.Search;
            }
        }
        else
        {
            _waitTimer = 0f;
        }

        // Try to bust
        float distanceToPlayer = Vector3.Distance(transform.position, Player.Transform.position);
        if (distanceToPlayer < m_BustDistance)
        {
            _bustTimer += m_AiUpdateRate;
            if (_bustTimer >= m_TimeToBust)
            {
                ClankerHivemind.Instance.Busted();
                m_BustBehavior.Interact();
            }
        }
        else
        {
            _bustTimer -= m_AiUpdateRate;
        }

        AlarmOthers(AIState.Chase);
    }

    private bool CanSeePlayer()
    {
        Transform player = Player.Transform;
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (distanceToPlayer <= m_VisionRange && angleToPlayer <= m_VisionAngle / 2f)
        {
            Vector3 visionOrigin = transform.position + (Vector3.up * 1f);
            Vector3 playerCenter = player.position + (Vector3.up * 1f);

            // Check for obstacles between the enemy and the player
#if UNITY_EDITOR
            Debug.DrawLine(visionOrigin, playerCenter, Color.red);
#endif
            if (!Physics.Linecast(visionOrigin, playerCenter, out RaycastHit hit, m_ObstacleLayers))
            {
                SawPlayer?.Invoke(this, player.position);
                return true;
            }
        }
        return false;
    }

    private bool _couldSeePlayer = false;
    private void TrySpotPlayer(Color defaultVisionColor)
    {
        if (CanSeePlayer())
        {
            if (!_couldSeePlayer)
            {
                RuntimeManager.PlayOneShot(m_InRange, transform.position);
            }
            _couldSeePlayer = true;

            _targetVisionColor = m_ChaseVisionColor;

            _spotPlayerTimer += m_AiUpdateRate;

            if (_spotPlayerTimer >= m_TimeToSpotPlayer)
            {
                CurrentState = AIState.Chase;
            }
        }
        else
        {
            _couldSeePlayer = false;
            _spotPlayerTimer -= m_AiUpdateRate;
            _spotPlayerTimer = Mathf.Max(0, _spotPlayerTimer);
            _targetVisionColor = defaultVisionColor;
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
            if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, m_VisionRange, m_ObstacleLayers))
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

    private void AlarmOthers(AIState state)
    {
        if (UnityEngine.Random.value >= m_ChanceToCommunicate) return;

        int count = Physics.OverlapSphereNonAlloc(transform.position, m_CommunicationRadius, s_buffer);
        for (int i = 0; i < count; i++)
        {
            if (s_buffer[i].TryGetComponent(out PatrolNPC clanker) && clanker.CurrentState < state)
            {
                clanker.SuggestTo(state);
            }
        }
    }

    public void SuggestTo(AIState state)
    {
        if (_communicationTimer > 0f) return;

        CurrentState = state;
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

        Gizmos.DrawWireSphere(transform.position, m_BustDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_CommunicationRadius);
    }
#endif
}

using UnityEngine;

public class FreeFormMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementSpeed = 10f;
    public float fastMovementMultiplier = 3f;
    public float smoothness = 2f;
    
    [Header("Control Settings")]
    public bool enableMovement = true;
    
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        targetPosition = transform.position;
        
        // Lock Y position to maintain strategy game perspective
        targetPosition.y = transform.position.y;
    }
    
    void Update()
    {
        HandleMovement();
        ApplySmoothTransforms();
    }
    
    void HandleMovement()
    {
        if (!enableMovement) return;
        
        Vector3 movement = Vector3.zero;
        
        // WASD movement - only on X and Z axes
        if (Input.GetKey(KeyCode.W))
            movement += Vector3.forward;  // World forward (Z+)
        if (Input.GetKey(KeyCode.S))
            movement += Vector3.back;     // World backward (Z-)
        if (Input.GetKey(KeyCode.A))
            movement += Vector3.left;     // World left (X-)
        if (Input.GetKey(KeyCode.D))
            movement += Vector3.right;    // World right (X+)
        
        // Normalize to prevent faster diagonal movement
        if (movement.magnitude > 1f)
            movement.Normalize();
        
        // Apply speed multiplier
        float currentSpeed = movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= fastMovementMultiplier;
        
        // Update target position (only X and Z, preserve Y)
        Vector3 movementDelta = movement * currentSpeed * Time.deltaTime;
        targetPosition.x += movementDelta.x;
        targetPosition.z += movementDelta.z;
        // Y remains unchanged for strategy camera
    }
    
    void ApplySmoothTransforms()
    {
        // Smooth position movement - only interpolate X and Z
        Vector3 currentPos = transform.position;
        Vector3 smoothedPosition = Vector3.SmoothDamp(currentPos, targetPosition, ref velocity, 1f / smoothness);
        
        // Ensure Y position stays locked
        smoothedPosition.y = currentPos.y;
        transform.position = smoothedPosition;
    }
    
    // Public methods for external control
    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }
    
    public void SetPosition(Vector3 newPosition)
    {
        // Only allow setting X and Z, preserve current Y
        targetPosition.x = newPosition.x;
        targetPosition.z = newPosition.z;
        targetPosition.y = transform.position.y;
    }
    
    public void MoveToPoint(Vector3 point)
    {
        // Move to a specific point, preserving Y coordinate
        targetPosition.x = point.x;
        targetPosition.z = point.z;
        targetPosition.y = transform.position.y;
    }
}

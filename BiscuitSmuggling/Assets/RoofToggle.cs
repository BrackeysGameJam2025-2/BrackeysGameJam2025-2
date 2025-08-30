using UnityEngine;
using System.Collections.Generic;

public class RoofToggle : MonoBehaviour
{
    [Header("Objects to Toggle")]
    [SerializeField]
    private List<GameObject> objectsToToggle = new List<GameObject>();
    
    [Header("Collider Settings")]
    [SerializeField]
    [Tooltip("List of colliders to use as trigger areas. If empty, will use collider on this GameObject.")]
    private List<Collider> triggerColliders = new List<Collider>();
    
    [Header("Settings")]
    [Tooltip("Tag to identify the player")]
    public string playerTag = "Player";
    
    [Tooltip("Should objects be visible when player is inside?")]
    public bool showWhenPlayerInside = false;
    
    private bool playerIsInside = false;
    private HashSet<Collider> playersInside = new HashSet<Collider>();
    
    void Start()
    {
        // If no colliders assigned, use the collider on this GameObject
        if (triggerColliders.Count == 0)
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                triggerColliders.Add(col);
            }
        }
        
        // Validate all colliders
        foreach (Collider col in triggerColliders)
        {
            if (col == null) continue;
            
            if (!col.isTrigger)
            {
                Debug.LogWarning($"RoofToggle collider '{col.name}' should be set as Trigger for proper detection.", this);
            }
        }
        
        if (triggerColliders.Count == 0)
        {
            Debug.LogError("RoofToggle requires at least one Collider component!", this);
        }
        
        // Set initial state based on settings
        SetObjectsVisibility(!showWhenPlayerInside);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if the trigger is one of our assigned colliders
        Collider triggeredCollider = GetComponent<Collider>();
        if (triggeredCollider != null && triggerColliders.Contains(triggeredCollider))
        {
            HandlePlayerEnter(other);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Check if the trigger is one of our assigned colliders
        Collider triggeredCollider = GetComponent<Collider>();
        if (triggeredCollider != null && triggerColliders.Contains(triggeredCollider))
        {
            HandlePlayerExit(other);
        }
    }
    
    private void HandlePlayerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            playersInside.Add(other);
            
            if (!playerIsInside)
            {
                playerIsInside = true;
                OnPlayerEntered();
            }
        }
    }
    
    private void HandlePlayerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            playersInside.Remove(other);
            
            // Only trigger exit if no players remain inside
            if (playersInside.Count == 0 && playerIsInside)
            {
                playerIsInside = false;
                OnPlayerExited();
            }
        }
    }
    
    private bool IsPlayer(Collider other)
    {
        // Check by tag first
        if (other.CompareTag(playerTag))
            return true;
        
        // Fallback: check for FreeFormMovement component
        if (other.GetComponent<FreeFormMovement>() != null)
            return true;
        
        return false;
    }
    
    private void OnPlayerEntered()
    {
        Debug.Log("Player entered roof area", this);
        SetObjectsVisibility(showWhenPlayerInside);
    }
    
    private void OnPlayerExited()
    {
        Debug.Log("Player exited roof area", this);
        SetObjectsVisibility(!showWhenPlayerInside);
    }
    
    private void SetObjectsVisibility(bool isVisible)
    {
        foreach (GameObject obj in objectsToToggle)
        {
            if (obj != null)
            {
                obj.SetActive(isVisible);
            }
        }
    }
    
    // Public methods for external control
    public void AddObjectToToggle(GameObject obj)
    {
        if (obj != null && !objectsToToggle.Contains(obj))
        {
            objectsToToggle.Add(obj);
        }
    }
    
    public void RemoveObjectFromToggle(GameObject obj)
    {
        objectsToToggle.Remove(obj);
    }
    
    public void ClearToggleList()
    {
        objectsToToggle.Clear();
    }
    
    // Force refresh the toggle state
    public void RefreshToggleState()
    {
        SetObjectsVisibility(playerIsInside ? showWhenPlayerInside : !showWhenPlayerInside);
    }
    
    // Editor helper to preview the toggle effect
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    void OnDrawGizmos()
    {
        // Draw gizmos for all assigned colliders
        List<Collider> collidersToShow = triggerColliders.Count > 0 ? triggerColliders : new List<Collider> { GetComponent<Collider>() };
        
        foreach (Collider col in collidersToShow)
        {

            if (col == null) continue;
            
            Gizmos.color = playerIsInside ? Color.green : Color.yellow;
            Gizmos.matrix = col.transform.localToWorldMatrix;
            
            if (col is BoxCollider box)
                Gizmos.DrawWireCube(box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            else if (col is CapsuleCollider capsule)
                Gizmos.DrawWireCube(capsule.center, new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2));
        }
    }
}

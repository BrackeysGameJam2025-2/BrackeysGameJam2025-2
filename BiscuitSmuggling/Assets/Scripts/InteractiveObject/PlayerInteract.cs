using cherrydev;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError($"There multiple instance of {this} point {this.gameObject}");
            Destroy(gameObject);
        }
    }
    private List<InteractiveObject> objectsToInteract = new();

    private InteractiveObject lastInteractedObject;

    [SerializeField]
    private DialogNodeGraph info;

    private void Update()
    {
        // Check for the "Interact" key press in the old Input System
        if (Input.GetKeyDown(KeyCode.E)) // Replace KeyCode.E with your desired key
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (lastInteractedObject != null)
        {
            lastInteractedObject.Interact();
        }
    }

    public void PlayerInArea(InteractiveObject interact)
    {
        DialogManager.Instance.ShowInteractInfo(info);
        objectsToInteract.Add(interact);
        lastInteractedObject = interact; // Remember the last interacted object

    }

    public void PlayerOutArea(InteractiveObject interact)
    {
        objectsToInteract.Remove(interact);

        // Update the last interacted object to the previous one in the list, if any
        if (objectsToInteract.Count > 0)
        {
            lastInteractedObject = objectsToInteract[objectsToInteract.Count - 1];
        }
        else
        {
            lastInteractedObject = null; // No objects left to interact with
            DialogManager.Instance.HideInteractInfo();
        }
    }

    public void Teleport()
    {
        this.transform.position = TeleportationPoint.Instance.GetPosition();
    }
}

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
            Debug.LogError($"There multiple instance of {this} point {gameObject}");
            Destroy(gameObject);
        }
    }
    private List<InteractiveObject> objectsToInteract = new();

    private InteractiveObject lastInteractedObject;

    [SerializeField]
    private DialogNodeGraph info;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
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

    public void PlayerInArea(InteractiveObject interact, DialogNodeGraph infoOverride = null)
    {
        DialogManager.Instance.ShowInteractInfo(infoOverride ? infoOverride : info);
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
        transform.position = TeleportationPoint.Instance.GetPosition();
    }
}

using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    protected InteractiveObjectBehavior behavior;

    [SerializeField]
    protected bool waitForPlayerToInteract = true;

    [SerializeField]
    protected bool preventMultipleInteractions = false;

    [SerializeField] private Collider interactionArea;

    protected PlayerInteract playerInArea;

    private void OnTriggerEnter(Collider other)
    {
        playerInArea = other.gameObject.GetComponent<PlayerInteract>();
        if (playerInArea != null)
        {
            if (waitForPlayerToInteract)
            {
                playerInArea.PlayerInArea(this);
            }
            else
            {
                Interact();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerInteract>();
        if (player != null)
        {
            player.PlayerOutArea(this);
            playerInArea = null;
        }
    }

    public void Interact()
    {
        behavior.OnInteractionResult += OnAccept;
        behavior.Interact();
    }

    protected virtual void OnAccept(bool obj)
    {
        behavior.OnInteractionResult -= OnAccept;
        if (!preventMultipleInteractions)
            return;
        if (obj)
        {
            playerInArea.PlayerOutArea(this);
            interactionArea.enabled = false;
        }
    }
}

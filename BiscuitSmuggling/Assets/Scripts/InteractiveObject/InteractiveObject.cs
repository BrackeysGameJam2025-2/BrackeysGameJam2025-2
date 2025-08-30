using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private InteractiveObjectBehavior behavior;

    [SerializeField]
    private bool waitForPlayerToInteract = true;

    [SerializeField]
    private bool preventMultipleInteractions = false;

    [SerializeField] private Collider interactionArea;

    private PlayerInteract playerInArea;

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

    private void OnAccept(bool obj)
    {
        behavior.OnInteractionResult -= OnAccept;
        if (!preventMultipleInteractions)
            return;
        if (obj)
        {
            interactionArea.enabled = false;
            playerInArea.PlayerOutArea(this);
        }
        else
        {
            interactionArea.enabled = true;
        }
    }
}

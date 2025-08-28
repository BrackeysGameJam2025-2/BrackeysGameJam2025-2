using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private InteractiveObjectBehavior behavior;

    [SerializeField]
    private bool waitForPlayerToInteract = true;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerInteract>();
        if (player != null)
        {
            if (waitForPlayerToInteract)
            {
                player.PlayerInArea(this);
            }
            else
                Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerInteract>();
        if (player != null)
        {
            player.PlayerOutArea(this);
        }
    }

    public void Interact()
    {
        behavior.Interact();
    }
}

using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    private InteractiveObjectBehavior behavior;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerInteract>();
        if (player != null)
        {
            player.PlayerInArea(this);
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

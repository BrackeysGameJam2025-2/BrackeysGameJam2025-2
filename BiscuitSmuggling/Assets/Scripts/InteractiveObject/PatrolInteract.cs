using UnityEngine;

[CreateAssetMenu(fileName = "FridgeInteraction", menuName = "Scriptable Objects/InteraciveObjectBechavior/Patrol")]
public class PatrolInteract : InteractiveObjectBehavior
{
    [SerializeField] private Transform exitPoint;
    public new void Interact()
    {
        if (dialogGraphs != null && dialogGraphs.Length > 0)
        {
            int index = Random.Range(0, dialogGraphs.Length);
            DialogManager.Instance.ShowDialog(dialogType, dialogGraphs[index], this);
        }
        else
        {
            Debug.LogWarning("DialogGraph is empty or not assigned.");
        }
    }

    public override void Accept()
    {
        Debug.LogWarning("Need to leave the building");
    }

    public override void Reject()
    {

    }
}

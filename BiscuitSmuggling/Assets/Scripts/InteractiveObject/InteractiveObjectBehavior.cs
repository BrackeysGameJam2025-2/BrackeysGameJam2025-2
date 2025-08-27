using cherrydev;
using UnityEngine;

[CreateAssetMenu(fileName = "InteraciveObjectBechavior", menuName = "Scriptable Objects/InteraciveObjectBechavior")]
public class InteractiveObjectBehavior : ScriptableObject
{
    [SerializeField]
    private DialogNodeGraph dialogGraph;
    [SerializeField]
    private DialogType dialogType;

    public void Interact()
    {
        DialogManager.Instance.ShowDialog(dialogType, dialogGraph);
    }
}

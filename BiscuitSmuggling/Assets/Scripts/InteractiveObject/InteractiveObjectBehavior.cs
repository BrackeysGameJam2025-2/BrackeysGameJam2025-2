using cherrydev;
using UnityEngine;

public abstract class InteractiveObjectBehavior : ScriptableObject
{
    [SerializeField]
    private DialogNodeGraph dialogGraph;
    [SerializeField]
    private DialogType dialogType;

    public void Interact()
    {
        DialogManager.Instance.ShowDialog(dialogType, dialogGraph, this);
    }

    public abstract void Accept();

    public abstract void Reject();
}

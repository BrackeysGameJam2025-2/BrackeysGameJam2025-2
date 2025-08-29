using cherrydev;
using UnityEngine;

public abstract class InteractiveObjectBehavior : ScriptableObject
{

    [SerializeField]
    protected DialogNodeGraph[] dialogGraphs;
    [SerializeField]
    protected DialogType dialogType;

    public void Interact()
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

    public abstract void Accept();

    public abstract void Reject();
}

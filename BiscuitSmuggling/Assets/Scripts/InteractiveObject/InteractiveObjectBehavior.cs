using cherrydev;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class InteractiveObjectBehavior : ScriptableObject
{

    [SerializeField]
    protected DialogNodeGraph[] dialogGraphs;
    [SerializeField]
    protected DialogType dialogType;


    public event Action<bool> OnInteractionResult;

    protected DialogNodeGraph chosenGraph;

    public void Interact()
    {
        if (dialogGraphs != null && dialogGraphs.Length > 0)
        {
            int index = Random.Range(0, dialogGraphs.Length);
            chosenGraph = dialogGraphs[index];
            DialogManager.Instance.ShowDialog(dialogType, dialogGraphs[index], this);
        }
        else
        {
            Debug.LogWarning("DialogGraph is empty or not assigned.");
        }
    }

    protected void TriggerInteractionResult(bool result)
    {
        OnInteractionResult?.Invoke(result);
    }

    public abstract void Prepare(DialogBehaviour dialogBehaviour);

    public abstract void Accept();

    public abstract void Reject();
}

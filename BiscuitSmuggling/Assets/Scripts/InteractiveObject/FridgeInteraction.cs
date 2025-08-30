using cherrydev;
using UnityEngine;

[CreateAssetMenu(fileName = "FridgeInteraction", menuName = "Scriptable Objects/InteraciveObjectBechavior/Fridge")]
public class FridgeInteraction : InteractiveObjectBehavior
{
    [SerializeField]
    private ItemMetadata food;
    public override void Prepare(DialogBehaviour dialogBehaviour)
    {
        if (Inventory.Instance.HasItem(food, 1))
            dialogBehaviour.SetVariableValue("haveFood", true);
        else
            dialogBehaviour.SetVariableValue("haveFood", false);
    }

    public override void Accept()
    {
        TriggerInteractionResult(true);
        Inventory.Instance.AddItem(food, 1);
    }

    public override void Reject()
    {
        TriggerInteractionResult(false);
    }

}

using cherrydev;
using UnityEngine;

[CreateAssetMenu(fileName = "FridgeInteraction", menuName = "Scriptable Objects/InteraciveObjectBechavior/Fridge")]
public class FridgeInteraction : InteractiveObjectBehavior
{
    public override void Prepare(DialogBehaviour dialogBehaviour)
    {

    }

    public override void Accept()
    {
        TriggerInteractionResult(true);
    }

    public override void Reject()
    {
        TriggerInteractionResult(false);
    }

}

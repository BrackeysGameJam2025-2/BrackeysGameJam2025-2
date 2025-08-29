using UnityEngine;

[CreateAssetMenu(fileName = "FridgeInteraction", menuName = "Scriptable Objects/InteraciveObjectBechavior/Fridge")]
public class FridgeInteraction : InteractiveObjectBehavior
{
    public override void Accept()
    {
        Debug.LogWarning("Accept is pressed, but it is not yet implemented.");
    }

    public override void Reject()
    {
        Debug.LogWarning("Reject is pressed, but it is not yet implemented.");
    }
}

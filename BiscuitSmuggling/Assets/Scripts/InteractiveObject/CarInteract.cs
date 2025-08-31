using cherrydev;
using UnityEngine;

[CreateAssetMenu(fileName = "CarInteract", menuName = "Scriptable Objects/InteraciveObjectBechavior/CarInteract")]
public class CarInteract : InteractiveObjectBehavior
{
    public override void Accept()
    {
        Debug.Log("Accepted!");
        foreach (var place in GameManager.Instance.CarSpots)
        {
            var placeItem = DialogManager.Instance.CurrentDialog.GetVariableValue<int>(place);

            // Add items to CarInventory based on the place and item code
            CarInventory.Instance.AddItem(place, placeItem);
        }
    }

    public override void Prepare(DialogBehaviour dialogBehaviour)
    {
        foreach (var place in GameManager.Instance.CarSpots)
        {
            DialogManager.Instance.CurrentDialog.SetVariableValue(place, 0);
        }
        Inventory.Instance.PrepareInventoryInfo.PrepareBartender(dialogBehaviour);
    }

    public override void Reject()
    {

    }
}

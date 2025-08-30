using cherrydev;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInteractionToPickUp", menuName = "Scriptable Objects/InteraciveObjectBechavior/Item")]
public class PickUpItemInteraction : InteractiveObjectBehavior
{
    [SerializeField] private ItemCount[] ItemToAdd;

    [Serializable]
    private class ItemCount
    {
        public ItemMetadata itemToAdd;
        public int count;

        public bool dialogVariable;
        public string name;
    }
    public override void Prepare(DialogBehaviour dialogBehaviour)
    {
        foreach (var item in ItemToAdd)
        {
            if (item.dialogVariable)
                dialogBehaviour.SetVariableValue(item.name, Inventory.Instance.GetItem(item.itemToAdd));
        }
    }

    public override void Accept()
    {
        foreach (var item in ItemToAdd)
            Inventory.Instance.AddItem(item.itemToAdd, item.count);

        TriggerInteractionResult(true);
    }

    public override void Reject()
    {
        TriggerInteractionResult(true);
    }
}

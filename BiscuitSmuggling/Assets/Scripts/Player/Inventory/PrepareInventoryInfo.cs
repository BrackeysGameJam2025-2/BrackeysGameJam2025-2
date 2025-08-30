using cherrydev;
using System;
using UnityEngine;

public class PrepareInventoryInfo : MonoBehaviour
{
    [SerializeField] private InventoryCheck[] itemsInInventory;

    public void PrepareBartender(DialogBehaviour dialog)
    {
        foreach (var itemInInventory in itemsInInventory)
        {
            switch (itemInInventory.itemVariableType)
            {
                case VariableType.Bool:

                    if (Inventory.Instance.HasItem(itemInInventory.itemMetadata, itemInInventory.itemQuantity))
                        dialog.SetVariableValue(itemInInventory.itemVariableName, true);
                    else
                        dialog.SetVariableValue(itemInInventory.itemVariableName, false);
                    break;
                case VariableType.Int:
                    dialog.SetVariableValue(itemInInventory.itemVariableName, itemInInventory.itemQuantity);
                    break;
                case VariableType.Float:
                case VariableType.String:
                    Debug.LogWarning(
                        $"There is stupid name, because I don't think any one would use this type {itemInInventory.itemVariableType}!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
[Serializable]
public class InventoryCheck
{
    public string itemVariableName;
    public VariableType itemVariableType;
    public ItemMetadata itemMetadata;
    public int itemQuantity;
}
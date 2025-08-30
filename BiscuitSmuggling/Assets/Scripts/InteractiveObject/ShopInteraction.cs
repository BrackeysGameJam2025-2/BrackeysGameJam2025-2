using cherrydev;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopInteraction", menuName = "Scriptable Objects/InteraciveObjectBechavior/ShopInteraction")]
public class ShopInteraction : InteractiveObjectBehavior
{
    [SerializeField]
    private ShopItemCheck[] itemsForSale;

    public override void Prepare(DialogBehaviour dialogBehaviour)
    {
        Inventory.Instance.PrepareInventoryInfo.PrepareBartender(dialogBehaviour);
    }

    public override void Accept()
    {
        foreach (var shopItem in itemsForSale)
        {
            switch (shopItem.itemVariableType)
            {
                case VariableType.Bool:
                    var haveBeenBought = DialogManager.Instance.CurrentDialog.GetVariableValue<bool>(shopItem.itemVariableName);

                    if (haveBeenBought == null)
                        return;

                    if (haveBeenBought)
                        Inventory.Instance.AddItem(shopItem.itemMetadata, 1);

                    break;
                case VariableType.Int:
                    var count = DialogManager.Instance.CurrentDialog.GetVariableValue<int>(shopItem.itemVariableName);
                    if (count == null)
                        return;
                    Inventory.Instance.AddItem(shopItem.itemMetadata, -count);
                    break;
                case VariableType.Float:
                case VariableType.String:
                    Debug.LogWarning($"There is stupid name, because I don't think any one would use this type {shopItem.itemVariableType}!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }

    public override void Reject()
    {
        Accept();
    }
}

[Serializable]
public class ShopItemCheck
{
    public string itemVariableName;
    public VariableType itemVariableType;
    public ItemMetadata itemMetadata;
}
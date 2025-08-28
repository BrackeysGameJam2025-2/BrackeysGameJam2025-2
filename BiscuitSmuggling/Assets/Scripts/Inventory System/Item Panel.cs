using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Rendering;

public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    public Inventory inventory;
    public ItemSlotInfo itemSlot;
    public Mouse mouse;
    public Image itemImage;
    public TextMeshProUGUI stacksText;
    private bool click;

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerPress = this.gameObject;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        click = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnClick();
        click = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
        }
    }

    public void SwapItems(ItemSlotInfo slotA, ItemSlotInfo slotB)
    {
        ItemSlotInfo tempItem = new ItemSlotInfo(slotA.item, slotB.stacks);
        slotA.item = slotB.item;
        slotA.stacks = slotB.stacks;

        slotB.item = tempItem.item;
        slotB.stacks = tempItem.stacks;

    }
    public void DropItem()
    {
        itemSlot.item = mouse.itemSlotInfo.item;
        itemSlot.stacks = mouse.itemSlotInfo.stacks;
        inventory.ClearSlot(mouse.itemSlotInfo);
    }
    public void PickUp()
    {
        mouse.itemSlotInfo = itemSlot;
        mouse.SetUi();
    }

    public void FadeOut()
    {
        itemImage.CrossFadeAlpha(0.3f, 0.05f, true);
    }
    public void OnClick()
    {   if (inventory != null)
        {
            mouse = inventory.mouse;
            if (mouse.itemSlotInfo.item == null)
            {
                if (itemSlot.item != null)
                {
                    PickUp();
                    FadeOut();
                }
            }
            else
           {
            if (itemSlot == mouse.itemSlotInfo)
             {
                inventory.RefreshInventory();
             }
            else if (itemSlot.item == null)
             {
                DropItem();
                inventory.RefreshInventory();
             }
            else if (itemSlot.item.GiveName() != mouse.itemSlotInfo.item.GiveName())
             {
                SwapItems(itemSlot, mouse.itemSlotInfo);
                inventory.RefreshInventory();
             }
           }
        
        }  
    }
}

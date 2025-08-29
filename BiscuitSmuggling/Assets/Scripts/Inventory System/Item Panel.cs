using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    public Inventory inventory;
    public ItemSlotInfo itemSlot;
    public Mouse mouse;
    public Image itemImage;
    public TextMeshProUGUI stacksText;
    private bool click;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICKED" + gameObject.name);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerPress = this.gameObject;


    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Item clicked");
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
        //ItemSlotInfo tempItem = new ItemSlotInfo(slotA.item, slotA.stacks);
        Item tempItem = slotA.item;
        int tempStack = slotA.stacks;
        slotA.item = slotB.item;
        slotA.stacks = slotB.stacks;

        slotB.item = tempItem;
        slotB.stacks = tempStack;

    }
    public void DropItem()
    {
        itemSlot.item = mouse.itemSlotInfo.item;
        itemSlot.stacks = mouse.itemSlotInfo.stacks;
        inventory.ClearSlot(mouse.itemSlotInfo);
        inventory.RefreshInventory();
    }
    public void PickUp()
    {
        mouse.itemSlotInfo = new ItemSlotInfo(itemSlot.item, itemSlot.stacks);
        mouse.SetUi();
        inventory.ClearSlot(itemSlot);
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

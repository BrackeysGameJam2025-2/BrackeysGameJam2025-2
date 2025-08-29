using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeReference] public List<ItemSlotInfo> items = new List<ItemSlotInfo>();

    [Space]
    [Header("Inventory Menu Components")]
    public GameObject inventoryMenu;
    public GameObject itemPanel;
    public GameObject itemPanelGrid;
    public Mouse mouse;

    private List<ItemPanel> existingPanels = new List<ItemPanel>();
    Dictionary<string, Item> allItemDictionary = new Dictionary<string, Item>();
    [Space]
    public int inventorySize = 8;

    void Start()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            items.Add(new ItemSlotInfo(null, 0));
        }

        List<Item> allItems = GetAllItems().ToList();
        string itemsInDictionary = "Item in Dictionary";
        foreach (Item item in allItems)
        {
            if (!allItemDictionary.ContainsKey(item.GiveName()))
            {
                allItemDictionary.Add(item.GiveName(), item);
                itemsInDictionary += ", " + item.GiveName();
            }
            else
            {
                Debug.Log("" + item + "already exists in dictionary" + allItemDictionary[item.GiveName()]);
            }
        }
        itemsInDictionary += ".";
        Debug.Log(itemsInDictionary);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryMenu.activeSelf)
            {
                inventoryMenu.SetActive(false);
                mouse.EmptySlot();
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                inventoryMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                RefreshInventory();
            }
        }
    }


    public void RefreshInventory()
    {
        existingPanels = itemPanelGrid.GetComponentsInChildren<ItemPanel>().ToList();

        if (existingPanels.Count < inventorySize)
        {
            int amountToCreate = inventorySize - existingPanels.Count;
            for (int i = 0; i < amountToCreate; i++)
            {
                GameObject newPanel = Instantiate(itemPanel, itemPanelGrid.transform);
                existingPanels.Add(newPanel.GetComponent<ItemPanel>());
            }
        }

        int index = 0;

        foreach (ItemSlotInfo itemInfo in items)
        {
            //name items in list





            itemInfo.name = "" + (index + 1);


            if (itemInfo.item != null)
            {
                itemInfo.name += ": " + itemInfo.item.GiveName();
            }

            else
            {
                itemInfo.name += ": -";

            }

            //Update panel
            ItemPanel panel = existingPanels[index];
            panel.name = itemInfo.name + "Panel";
            if (panel != null)
            {
                panel.inventory = this;
                panel.itemSlot = itemInfo;
                if (itemInfo.item != null)
                {
                    panel.itemImage.gameObject.SetActive(true);
                    panel.itemImage.sprite = itemInfo.item.GiveItemImage();
                    panel.itemImage.CrossFadeAlpha(1, 0.05f, true);
                    panel.stacksText.gameObject.SetActive(true);
                    panel.stacksText.text = itemInfo.stacks.ToString(); //" " + i.stacks;
                }
                else
                {
                    panel.itemImage.gameObject.SetActive(false);
                    panel.stacksText.gameObject.SetActive(false);

                }

            }
            index++;
        }
        mouse.EmptySlot();
    }

    public int AddItem(string itemName, int amount)
    {
        // find item to add
        Item item = null;
        allItemDictionary.TryGetValue(itemName, out item);
        //Exit the method if item not found 
        if (item == null)
        {
            Debug.Log("Could not found item in dictionary to add to inventory");

        }
        foreach (ItemSlotInfo itemInfo in items)
            {
                if (itemInfo.item != null)
                {
                    if (itemInfo.item.GiveName() == item.GiveName())
                    {
                        if (amount > itemInfo.item.MaxStacks() - itemInfo.stacks)
                        {
                            amount -= itemInfo.item.MaxStacks() - itemInfo.stacks;
                            itemInfo.stacks = itemInfo.item.MaxStacks();
                        }
                        else
                        {
                            itemInfo.stacks += amount;
                            if (inventoryMenu.activeSelf) RefreshInventory();
                            return 0;
                        }
                    }

                }
            }

        foreach (ItemSlotInfo itemInfo in items)
        {
            if (itemInfo.item == null)
            {
                if (amount > item.MaxStacks())
                {
                    itemInfo.item = item;
                    itemInfo.stacks = item.MaxStacks();
                    amount -= item.MaxStacks();
                }
                else
                {
                    itemInfo.item = item;
                    itemInfo.stacks = amount;
                    if (inventoryMenu.activeSelf) RefreshInventory();
                    return 0;
                }
            }
        }

        Debug.Log("No Space in Inventory for " + item.GiveName());
        if (inventoryMenu.activeSelf) RefreshInventory();
        return amount;
    }

    public void ClearSlot(ItemSlotInfo slot)
    {
        slot.item = null;
        slot.stacks = 0;
    }

    IEnumerable<Item> GetAllItems()
    {
        return System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Item)))
        .Select(type => System.Activator.CreateInstance(type) as Item);
    }
}

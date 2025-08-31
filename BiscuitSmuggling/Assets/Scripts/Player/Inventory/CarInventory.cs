using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarItem
{
    public ItemMetadata Item;
    public string HiddenSpotName;
}

[Serializable]
public class ItemIndex
{
    public int Index;
    public ItemMetadata[] Items;
}

public class CarInventory : MonoBehaviour
{
    public static CarInventory Instance;

    private List<CarItem> carItems = new();

    // List to map integers to specific items
    [SerializeField] private List<ItemIndex> itemIndexList = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.LogError("Multiple instances of CarInventory detected. Destroying duplicate.");
        }
        else
        {
            Instance = this;
        }
    }

    public void AddItem(string hiddenSpotName, int itemCode)
    {
        var itemIndex = itemIndexList.Find(index => index.Index == itemCode);
        if (itemIndex != null)
        {
            foreach (var item in itemIndex.Items)
            {
                var existingItem = carItems.Find(carItem => carItem.Item == item && carItem.HiddenSpotName == hiddenSpotName);
                if (existingItem != null)
                {
                    Debug.LogWarning($"Item {item.name} already exists in hidden spot {hiddenSpotName}.");
                }
                else
                {
                    carItems.Add(new CarItem { Item = item, HiddenSpotName = hiddenSpotName });
                    Debug.Log($"Added item {item.name} to hidden spot {hiddenSpotName}.");
                }
            }
        }
    }

    public bool HasItemInSpot(ItemMetadata item, string hiddenSpotName)
    {
        return carItems.Exists(carItem => carItem.Item == item && carItem.HiddenSpotName == hiddenSpotName);
    }

    public List<CarItem> GetItemsInSpot(string hiddenSpotName)
    {
        return carItems.FindAll(carItem => carItem.HiddenSpotName == hiddenSpotName);
    }
}
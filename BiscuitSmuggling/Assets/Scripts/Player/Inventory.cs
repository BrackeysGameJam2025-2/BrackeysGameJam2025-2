using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    [SerializeField]
    private PrepareInventoryInfo prepareInventoryInfo;

    public PrepareInventoryInfo PrepareInventoryInfo => prepareInventoryInfo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.LogError("Multiple instances of Inventory detected. Destroying duplicate.");
        }
        else
        {
            Instance = this;
        }
    }

    private Dictionary<ItemMetadata, int> _items = new();

    public void AddItem(ItemMetadata item, int quantity)
    {
        if (_items.ContainsKey(item))
        {
            _items[item] += quantity;
            Debug.Log($"Updated {item.name} quantity to {_items[item]} in inventory.");
        }
        else
        {
            _items.Add(item, quantity);
            Debug.Log($"Added {quantity} of {item.name} to inventory.");
        }
    }

    public bool HasItem(ItemMetadata item, int quantity)
    {
        foreach (var invItem in _items)
        {
            if (invItem.Key == item && invItem.Value >= quantity)
            {
                return true;
            }
        }
        return false;
    }

    public int GetItem(ItemMetadata itemToGet)
    {
        foreach (var invItem in _items)
        {
            if (invItem.Key == itemToGet)
            {
                return invItem.Value;
            }
        }
        return 0;
    }
}


using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] public string itemToDrop;
    [SerializeField] public int amount;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Inventory playerInventory = other.GetComponentInChildren<Inventory>();
            if (playerInventory != null) PickUpItem(playerInventory);
        }
    }
    public void PickUpItem(Inventory inventory)
    {
        amount = inventory.AddItem(itemToDrop, amount);
        if (amount < 1) Destroy(this.gameObject);
    }
}

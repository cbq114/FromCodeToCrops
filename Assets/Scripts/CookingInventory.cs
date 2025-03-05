using UnityEngine;
using System.Collections.Generic;


public class CookingInventory : MonoBehaviour
{
    [System.Serializable]
    public struct CookedItem
    {
        public Item item;
        public int amount;

        public CookedItem(Item item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    public static CookingInventory instance;
    private List<CookedItem> inventory = new List<CookedItem>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void AddItem(Item item, int amount)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == item)
            {
                inventory[i] = new CookedItem(item, inventory[i].amount + amount);
                return;
            }
        }

        inventory.Add(new CookedItem(item, amount));
    }
    public bool RemoveItem(Item item, int amount)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == item)
            {
                if (inventory[i].amount >= amount)
                {
                    // Lay phan tu ra bien tam
                    CookedItem updatedItem = inventory[i];

                    // Cap nhat so luong
                    updatedItem.amount -= amount;

                    // Gans lai vao danh sach
                    inventory[i] = updatedItem;

                    // Neu het so luong, xoa khoi danh sach
                    if (inventory[i].amount <= 0)
                        inventory.RemoveAt(i);

                    return true;
                }
            }
        }
        return false;
    }

    public int GetItemAmount(Item item)
    {
        foreach (CookedItem cookedItem in inventory)
        {
            if (cookedItem.item == item)
                return cookedItem.amount;
        }
        return 0;
    }
}

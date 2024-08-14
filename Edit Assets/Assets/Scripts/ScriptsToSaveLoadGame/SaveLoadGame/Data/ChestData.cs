using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestData
{
    public List<InventorySlot> ChestInventory;
    public bool looted;

    public ChestData(string id, bool isLootChest, ItemDatabaseObject ItemDatabase)
    {
        ChestInventory = new List<InventorySlot>();
        looted = false;
        if (isLootChest)
        {
            for (int i = 0; i < 5; i++)
            {
                ItemObject newItem = ItemDatabase.ItemList[Random.Range(0, ItemDatabase.ItemList.Count - 1)];
                ChestInventory.Add(new InventorySlot(newItem.itemId, newItem, 1));
            }
        }
    }
}

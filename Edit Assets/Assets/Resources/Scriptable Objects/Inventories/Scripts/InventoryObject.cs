using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Default Inventory")]
public class InventoryObject : ScriptableObject
{
    public ItemDatabaseObject ItemDatabase;
    public List<InventorySlot> InventoryList = new List<InventorySlot>();


    public void AddItem(ItemObject itemAdded, int amountAdded)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            //if item already in inventory, add amount and go next
            if(InventoryList[i].item == itemAdded)
            {
                InventoryList[i].AddAmount(amountAdded);
                return;
            } 
        }
        InventoryList.Add(new InventorySlot(ItemDatabase.GetItemId[itemAdded], itemAdded, amountAdded));
        SortItems();
    }

    public void RemoveItem(ItemObject itemRemoved)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i].item == itemRemoved)
            {
                //InventoryList.RemoveAt(i);
                InventoryList.Remove(InventoryList[i]);
            }
        }
        InventoryList.Sort(SortById);
    }
    public void RemoveItemAmount(ItemObject item, int amountRemoved)
    {
        for (int i = 0; i < InventoryList.Count; i++)
        {
            if (InventoryList[i].item == item)
            {
                //InventoryList.RemoveAt(i);
                if (InventoryList[i].amount <= amountRemoved)
                {
                    InventoryList.Remove(InventoryList[i]);
                    return;
                }
                InventoryList[i].RemoveAmount(amountRemoved);
            }
        }
        InventoryList.Sort(SortById);
    }

    public void SortItems()
    {
        InventoryList.Sort(SortById);
    }
    public int SortById(InventorySlot item1, InventorySlot item2)
    {
        return item1.item.itemId.CompareTo(item2.item.itemId);
    }
}

[System.Serializable]
public class InventorySlot
{
    public int id;
    public ItemObject item;
    public int amount;
    public InventorySlot(int hasId, ItemObject hasItem, int hasAmount)
    {
        id = hasId;
        item = hasItem;
        amount = hasAmount;
    }
    public void AddAmount(int added)
    {
        amount += added;
    }
    public void RemoveAmount(int removed)
    {
        amount -= removed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "New InventoryExchanger", menuName = "Inventory System/InventoryExchanger")]
public class ExchangeItemsObject : ScriptableObject
{
    public InventoryObject PlayerInventory;
    public InventoryObject OtherInventory;

    public ItemDatabaseObject Itemdatabase;
    public GameObject ItemDisplayPrefab;

    public void TransferItem(GameObject TransferItem)
    {
        if (OtherInventory != null)
        {
            var Item = TransferItem.GetComponent<AttachedItem>();
            //OtherInventory = TransferItem.transform.GetComponent<Item>().Inventory;
            if (TransferItem.transform.parent.name == "Content Loot")
            {
                PlayerInventory.AddItem(Item.item, Item.amount);
                OtherInventory.RemoveItem(Item.item);
                GameObject.Destroy(TransferItem);
            }
            else if (TransferItem.transform.parent.name == "Content Player")
            {
                OtherInventory.AddItem(Item.item, Item.amount);
                PlayerInventory.RemoveItem(Item.item);
                GameObject.Destroy(TransferItem);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Player Inventory")]
public class PlayerInventoryObject : InventoryObject
{
    public List<InventorySlot> OneItemTypeList = new List<InventorySlot>();
    public InventorySlot HelmetSlot;
    public ItemObject ChestSlot;

}

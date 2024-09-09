using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachedItem : MonoBehaviour
{
    public ItemObject item;
    public int amount;
    public InventoryObject InventoryObj;
    public List<InventorySlot> InventoryList;

    private void OnMouseEnter()
    {
        Debug.Log(item.description);
    }
    private void OnMouseExit()
    {
        Debug.Log("exit hover");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsTester : MonoBehaviour
{
    public int itemsPickedUp;
    public bool test;
    [SerializeField] private ItemObject itemObject;
    [SerializeField] private int itemAmount;
    void Awake()
    {
        //EventsManager.Instance.pickUpItemEvent.onItemPickUp -= ItemPickUp;
        EventsManager.Instance.pickUpItemEvent.onItemPickUp += ItemPickUp;
    }
    private void OnDisable()
    {
        EventsManager.Instance.pickUpItemEvent.onItemPickUp -= ItemPickUp;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //EventsManager.Instance.pickUpItemEvent.ItemPickUp(itemObject, itemAmount);
        }
    }
    private void ItemPickUp(ItemObject item, int amount)
    {
        itemsPickedUp++;
        itemObject = item;
        itemAmount = amount;
        //Debug.Log(itemObject.itemName + " : " + amount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemDisplay : MonoBehaviour
{
    public GameObject ItemDisplayPrefab;
    public LookAtTarget LookTargetScript;

    public void CreateDisplay(List<InventorySlot> inventory)
    {
        LookTargetScript = GameObject.FindGameObjectWithTag("Player").GetComponent<LookAtTarget>();
        //detroy all old itemdisplays in the inventory screen
        DeleteOldDisplay();
        if (inventory != null && inventory.Count > 0)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                CreateNewItemDisplay(inventory, i);
            }
        }
    }

    private void CreateNewItemDisplay(List<InventorySlot> inv, int i)
    {
        GameObject ItemPrefab = Instantiate(ItemDisplayPrefab, transform);
        Image image = ItemPrefab.transform.GetChild(0).GetComponent<Image>();
        TextMeshProUGUI amount = ItemPrefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var ItemScript = ItemPrefab.transform.GetComponent<AttachedItem>();
        //display these in the itemdisplay
        image.sprite = inv[i].item.Picture;
        amount.text = inv[i].amount.ToString();
        //atatch itemdata to itemdisplay
        ItemScript.item = inv[i].item;
        ItemScript.amount = inv[i].amount;
        ItemScript.InventoryList = inv;
        ItemPrefab.GetComponent<Button>().onClick.AddListener(delegate
        {
            //LookTargetScript.SwapItems(ItemScript.InventoryList, ItemScript.item, ItemScript.amount);
            InventoryManager.Instance.SwapItems(ItemScript.InventoryList, ItemScript.item, ItemScript.amount);
        });
    }

    void DeleteOldDisplay()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }
}

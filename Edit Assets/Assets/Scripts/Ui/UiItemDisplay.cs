using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiItemDisplay : MonoBehaviour
{
    [SerializeField] private Image ItemImange;
    [SerializeField] private TextMeshProUGUI ItemAmountText;
    [SerializeField] private UiItemName ItemNamePrefab;
    private ItemObject ItemObject;
    private int ItemAmount;
    private List<InventorySlot> InventoryOfItem;
    public void GetItemInfos(ItemObject item, int amount, List<InventorySlot> inventory)
    {
        ItemObject = item;
        ItemAmount = amount;
        InventoryOfItem = inventory;

        ItemImange.sprite = item.Picture;
        ItemAmountText.text = amount.ToString();
    }
    public void OnHoverEnter()
    {
        InventoryManager.Instance.ShowItemName(ItemObject, transform);
    }
    public void OnHoverExit()
    {
        InventoryManager.Instance.HideItemName();
    }
    public void OnMouseClick()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            TranferItemStack();
        }
        else
        {
            TransferOneItem();
        }
    }
    private void TranferItemStack()
    {
        InventoryManager.Instance.SwapItems(InventoryOfItem, ItemObject, ItemAmount);
    }
    private void TransferOneItem()
    {
        InventoryManager.Instance.SwapItems(InventoryOfItem, ItemObject, 1);
    }
}

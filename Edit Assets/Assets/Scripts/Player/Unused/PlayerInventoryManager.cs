using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventoryManager : MonoBehaviour, IDataPersistence
{
    public InventoryObject PlayerInventory;
    public ItemDatabaseObject Itemdatabase;

    [SerializeField] private GameObject PlayerInventoryContainer;
    [SerializeField] private GameObject PlayerItemDisplayPrefab;
    [SerializeField] private GameObject PlayerInventoryScrollView;


    public void LoadData(GameData data)
    {
        /*
        PlayerInventoryScrollView.SetActive(false);
        PlayerInventory.InventoryList.Clear();
        for (int i = 0; i < data.PlayerInventory.Count; i++)
        {
            PlayerInventory.InventoryList.Add(new InventorySlot(data.PlayerInventory[i].id, Itemdatabase.GetItem[data.PlayerInventory[i].id], data.PlayerInventory[i].amount));
        }
        */
    }

    public void SaveData(ref GameData data)
    {
        /*
        data.PlayerInventory.Clear();
        for (int i = 0; i < PlayerInventory.InventoryList.Count; i++)
        {
            data.PlayerInventory.Add(PlayerInventory.InventoryList[i]);
        }
        */
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!PlayerInventoryScrollView.activeSelf)
            {
                OpenPlayerInventory();
            }
            else
            {
                ClosePlayerInventory();
            }
        }
    }
    public void OpenPlayerInventory()
    {
        //detroy all old itemdisplays in the inventory screen
        for (int i = 0; i < PlayerInventoryContainer.transform.childCount; i++)
        {
            GameObject.Destroy(PlayerInventoryContainer.transform.GetChild(i).gameObject);
        }
        //display the items in the inventory screen
        for (int i = 0; i < PlayerInventory.InventoryList.Count; i++)
        {
            //instantiate an itemdisplay
            var Item = Instantiate(PlayerItemDisplayPrefab, PlayerInventoryContainer.transform);
            //get image, name and amount of the item
            var image = Item.transform.Find("ItemImage").GetComponent<Image>();
            var name = Item.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var amount = Item.transform.Find("ItemAmount").GetComponent<TextMeshProUGUI>();
            //display these in the itemdisplay
            image.sprite = PlayerInventory.InventoryList[i].item.Picture;
            name.text = PlayerInventory.InventoryList[i].item.itemName;
            amount.text = PlayerInventory.InventoryList[i].amount.ToString();
        }
        PlayerInventoryScrollView.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void ClosePlayerInventory()
    {
        PlayerInventoryScrollView.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

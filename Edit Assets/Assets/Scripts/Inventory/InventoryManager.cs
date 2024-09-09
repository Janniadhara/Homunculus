using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private ItemDatabaseObject Itemdatabase;
    //public GameObject ItemDisplayPrefab;
    [SerializeField] private UiItemDisplay ItemDisplayPrefab;
    [SerializeField] private UiItemName ItemNamePrefab;
    private UiItemName itemName;
    [SerializeField] private Canvas PlayerCanvas;

    [Header("Player Inventory")]
    [SerializeField] private InventoryObject PlayerInventory;
    [SerializeField] private GameObject PlayerInvScreen;
    [SerializeField] private GameObject PlayerItemDisplay;

    [Header("Player Equipment")]
    [SerializeField] private EquipmentObject PlayerEquipment;
    [SerializeField] private Transform MainWeaponSlot;
    [SerializeField] private Transform MainWeaponHand;
    private GameObject CurrentWeapon;

    [Header("Chest")]
    [SerializeField] private GameObject ChestInvScreen;
    [SerializeField] private GameObject ChestItemDisplay;
    private List<InventorySlot> ChestInventoryList;

    public static InventoryManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Inventory Manager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        PlayerInvScreen.SetActive(false);
        ChestInvScreen.SetActive(false);
        EventsManager.Instance.pauseGameEvent.onPauseGame += PauseGame;
        EventsManager.Instance.pickUpItemEvent.onItemPickUp += PickUpItem;
    }
    //load and save player inventory
    public void LoadData(GameData data)
    {
        Debug.Log("Loading player inventory.");

        PlayerInventory.InventoryList.Clear();
        for (int i = 0; i < data.PlayerInventory.Count; i++)
        {
            PlayerInventory.InventoryList.Add(new InventorySlot(data.PlayerInventory[i].id, 
                Itemdatabase.GetItem[data.PlayerInventory[i].id], 
                data.PlayerInventory[i].amount));
        }
        Debug.Log("Loading player inventory completed.");
        EquipWeapon();
    }
    public void SaveData(ref GameData data)
    {
        data.PlayerInventory.Clear();
        for (int i = 0; i < PlayerInventory.InventoryList.Count; i++)
        {
            data.PlayerInventory.Add(PlayerInventory.InventoryList[i]);
        }
    }
    private void PickUpItem(ItemObject item, int amount)
    {
        PlayerInventory.AddItem(item, amount);
    }
    private void PauseGame(bool isPaused)
    {
    }
    //open or close depending on screen state and return that value
    public void OpenPlayerInventory()
    {
        RemoveChildren(PlayerItemDisplay);
        CreateNewItemDisplay(PlayerInventory.InventoryList, PlayerItemDisplay);
        PlayerInvScreen.SetActive(true);
    }
    public void ClosePlayerInventory()
    {
        PlayerInvScreen.SetActive(false);
        HideItemName();
    }
    public void OpenChestInventory(List<InventorySlot> chestInventory)
    {
        ChestInventoryList = chestInventory;
        RemoveChildren(ChestItemDisplay);
        CreateNewItemDisplay(ChestInventoryList, ChestItemDisplay);
        ChestInvScreen.SetActive(true);
    }
    public void CloseChestInventory()
    {
        ChestInvScreen.SetActive(false);
        HideItemName();
    }
    public void SwapItems(List<InventorySlot> inventory, ItemObject item, int amount)
    {
        bool isInChest = false;
        if (ChestInvScreen.activeSelf)
        {
            if (inventory == PlayerInventory.InventoryList)
            {
                PlayerInventory.RemoveItemAmount(item, amount);
                //check if item already exists in the chest
                for (int i = 0; i < ChestInventoryList.Count; i++)
                {
                    if (ChestInventoryList[i].item == item)
                    {
                        ChestInventoryList[i].AddAmount(amount);
                        isInChest = true;
                        //EventsManager.Instance.pickUpItemEvent.ItemPickUp(item, -amount);
                        break;
                    }
                }
                if (!isInChest)
                {
                    ChestInventoryList.Add(new InventorySlot(item.itemId, item, amount));
                    //EventsManager.Instance.pickUpItemEvent.ItemPickUp(item, -amount);
                }
            }
            else
            {
                PlayerInventory.AddItem(item, amount);
                for (int i = 0; i < ChestInventoryList.Count; i++)
                {
                    if (ChestInventoryList[i].item == item)
                    {
                        ChestInventoryList[i].RemoveAmount(amount);
                        if (ChestInventoryList[i].amount <= 0)
                        {
                            ChestInventoryList.RemoveAt(i);
                        }
                    }
                }
                //EventsManager.Instance.pickUpItemEvent.ItemPickUp(item, amount);
            }
            RemoveChildren(PlayerItemDisplay);
            CreateNewItemDisplay(PlayerInventory.InventoryList, PlayerItemDisplay);
            RemoveChildren(ChestItemDisplay);
            CreateNewItemDisplay(ChestInventoryList, ChestItemDisplay);
            HideItemName();
        }
        EventsManager.Instance.pickUpItemEvent.PlayerInvChanged();
    }
    public void EquipWeapon()
    {
        if (CurrentWeapon != null)
        {
            Destroy(CurrentWeapon);
        }
        if (PlayerEquipment.Weapon != null)
        {
            CurrentWeapon = Instantiate(PlayerEquipment.Weapon.Prefab, MainWeaponSlot, false);
            CurrentWeapon.GetComponent<Rigidbody>().useGravity = false;
            CurrentWeapon.GetComponent<Rigidbody>().isKinematic = true;
            CurrentWeapon.GetComponentInChildren<MeshCollider>().enabled = false;
        }
    }
    void RemoveChildren(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }
    private void CreateNewItemDisplay(List<InventorySlot> inventoryList, GameObject itemDisplayParent)
    {
        for (int i = 0; i < inventoryList.Count; i++)
        {
            UiItemDisplay itemDisplay = Instantiate(ItemDisplayPrefab, itemDisplayParent.transform);
            itemDisplay.GetItemInfos(inventoryList[i].item, inventoryList[i].amount, inventoryList);
        }
        /*
         * GameObject ItemPrefab = Instantiate(ItemDisplayPrefab, itemDisplay.transform);
        Image image = ItemPrefab.transform.GetChild(1).GetComponent<Image>();
        TextMeshProUGUI amount = ItemPrefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        var ItemScript = ItemPrefab.transform.GetComponent<AttachedItem>();
        //display these in the itemdisplay
        image.sprite = inv.item.Picture;
        amount.text = inv.amount.ToString();
        //attatch itemdata to itemdisplay
        ItemScript.item = inv.item;
        ItemScript.amount = inv.amount;
        ItemScript.InventoryList = invName;
        ItemPrefab.GetComponent<Button>().onClick.AddListener(delegate
        {
            //LookTargetScript.SwapItems(ItemScript.InventoryList, ItemScript.item, ItemScript.amount);
            InventoryManager.Instance.SwapItems(ItemScript.InventoryList, ItemScript.item, ItemScript.amount);
        });
        */
    }
    public void ShowItemName(ItemObject item, Transform itemPic)
    {
        Vector3 itemNamePos = itemPic.position + new Vector3(-27.5f, 0, 0);
        if (itemName != null)
        {
            Destroy(itemName.gameObject);
        }
        itemName = Instantiate(ItemNamePrefab, itemNamePos, Quaternion.identity, PlayerCanvas.transform);
        itemName.SetItemName(item.itemName);
    }
    public void HideItemName()
    {
        if (itemName != null)
        {
            Destroy(itemName.gameObject);
        }
    }
}

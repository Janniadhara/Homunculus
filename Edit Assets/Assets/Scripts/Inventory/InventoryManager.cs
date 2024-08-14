using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private ItemDatabaseObject Itemdatabase;

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
        PlayerInvScreen.SetActive(true);
        PlayerItemDisplay.GetComponent<InventoryItemDisplay>().CreateDisplay(PlayerInventory.InventoryList);
    }
    public void ClosePlayerInventory()
    {
        PlayerInvScreen.SetActive(false);
    }
    public void OpenChestInventory(List<InventorySlot> chestInventory)
    {
        ChestInvScreen.SetActive(true);
        ChestItemDisplay.GetComponent<InventoryItemDisplay>().CreateDisplay(chestInventory);
        ChestInventoryList = chestInventory;
    }
    public void CloseChestInventory()
    {
        ChestInvScreen.SetActive(false);
    }
    public void SwapItems(List<InventorySlot> inventory, ItemObject item, int amount)
    {
        if (ChestInvScreen.activeSelf)
        {
            if (inventory == PlayerInventory.InventoryList)
            {
                PlayerInventory.RemoveItem(item);
                //check if item already exists in the chest
                for (int i = 0; i < ChestInventoryList.Count; i++)
                {
                    if (ChestInventoryList[i].item == item)
                    {
                        ChestInventoryList[i].amount += amount;
                        RefreshItemDisplays();
                        //EventsManager.Instance.pickUpItemEvent.ItemPickUp(item, -amount);
                        return;
                    }
                }
                ChestInventoryList.Add(new InventorySlot(item.itemId, item, amount));
                //EventsManager.Instance.pickUpItemEvent.ItemPickUp(item, -amount);
            }
            else
            {
                PlayerInventory.AddItem(item, amount);
                for (int i = 0; i < ChestInventoryList.Count; i++)
                {
                    if (ChestInventoryList[i].item == item)
                    {
                        ChestInventoryList.RemoveAt(i);
                    }
                }
                //EventsManager.Instance.pickUpItemEvent.ItemPickUp(item, amount);
            }
            RefreshItemDisplays();
        }
        EventsManager.Instance.pickUpItemEvent.PlayerInvChanged();
    }
    private void RefreshItemDisplays()
    {
        PlayerItemDisplay.GetComponent<InventoryItemDisplay>().CreateDisplay(PlayerInventory.InventoryList);
        ChestItemDisplay.GetComponent<InventoryItemDisplay>().CreateDisplay(ChestInventoryList);
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
}

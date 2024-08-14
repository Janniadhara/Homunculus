using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestInventoryManager : MonoBehaviour, IDataPersistence
{
    public InventoryObject InventorySO;
    public List<InventorySlot> InventoryList = new List<InventorySlot>();
    public ItemDatabaseObject ItemDatabase; //needed for loading cause item instance random shit
    public bool looted;

    [Header("Config")]
    [SerializeField] private bool lootChest;
    [SerializeField] private string id;
    [ContextMenu("Generate custom ID")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }
    private ChestData chestData;

    public void LoadData(GameData data)
    {
        data.ChestInventory.TryGetValue(id, out chestData);
        //if !null = not a new game = load inventory from save into list
        if (chestData != null)
        {
            InventoryList.Clear();
            if (chestData.ChestInventory.Count > 0)
            {
                for (int i = 0; i < chestData.ChestInventory.Count; i++)
                {
                    /*InventorySO.InventoryList.Add(
                        new InventorySlot(chestData.CInventory[i].id,
                        ItemDatabase.GetItem[chestData.CInventory[i].id],
                        chestData.CInventory[i].amount)
                        );
                    */
                    InventoryList.Add(
                        new InventorySlot(chestData.ChestInventory[i].id,
                        ItemDatabase.GetItem[chestData.ChestInventory[i].id],
                        chestData.ChestInventory[i].amount)
                        );
                }
            }
        }
        //if null = new game = load from scriptable object
        if (chestData == null)
        {
            chestData = new ChestData(id, lootChest, ItemDatabase);
            for (int i = 0; i < InventorySO.InventoryList.Count; i++)
            {
                InventoryList.Add(
                    new InventorySlot(InventorySO.InventoryList[i].id,
                    ItemDatabase.GetItem[InventorySO.InventoryList[i].id],
                    InventorySO.InventoryList[i].amount)
                    );
            }
            //Debug.Log(chestData.ToString());
        }
    }

    public void SaveData(ref GameData data)
    {
        if (chestData != null)
        {
            chestData.ChestInventory.Clear();
        }
        if (InventoryList.Count > 0)
        {
            for (int i = 0; i < InventoryList.Count; i++)
            {
                chestData.ChestInventory.Add(InventoryList[i]);
            }
        }
        if (data.ChestInventory.ContainsKey(id))
        {
            data.ChestInventory.Remove(id);
        }
        data.ChestInventory.Add(id, chestData);
    }
}

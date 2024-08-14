using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadItems : MonoBehaviour, IDataPersistence
{
    public ItemDatabaseObject Itemdatabase; 
    public Object[] LoadedItems;
    private void Start()
    {
        LoadedItems = Resources.LoadAll("", typeof(ItemObject));
        Itemdatabase.ItemList.Clear();
        Itemdatabase.GetItemId.Clear();
        Itemdatabase.GetItem.Clear();
        for (int i = 0; i < LoadedItems.Length; i++)
        {
            Itemdatabase.ItemList.Add((ItemObject)LoadedItems[i]);
            Itemdatabase.GetItemId.Add(Itemdatabase.ItemList[i], Itemdatabase.ItemList[i].itemId);
            Itemdatabase.GetItem.Add(Itemdatabase.ItemList[i].itemId, Itemdatabase.ItemList[i]);
        }
    } 
    public void LoadData(GameData data)
    {
        
    }

    public void SaveData(ref GameData data)
    {

    }
}

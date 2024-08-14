using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    //public ItemObject[] Items;
    public List<ItemObject> ItemList = new List<ItemObject>();
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();
    public Dictionary<ItemObject, int> GetItemId = new Dictionary<ItemObject, int>();

    //load (only in editor. works only with the items array
    public void OnAfterDeserialize()
    {
        /*
        GetItemId.Clear();
        GetItem.Clear();
        TestList.Clear();
        for (int i = 0; i < Items.Length; i++)
        {
            GetItemId.Add(Items[i], i);
            GetItem.Add(i, Items[i]);
        }
        */
    }

    //save  
    public void OnBeforeSerialize()
    {
        
    }
}

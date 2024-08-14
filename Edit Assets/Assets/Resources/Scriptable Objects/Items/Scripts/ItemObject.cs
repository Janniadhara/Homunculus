using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Default,
    Food,
    Material,
    Weapon,
    Shield,
    Helmet,
    Chestplate,
    Pants,
    Shoes,
    Gloves,
    Armor,
    Ring,
    Money
}
public abstract class ItemObject : ScriptableObject
{
    public string itemName;
    public int itemId;
    public GameObject Prefab;
    public Sprite Picture;
    public ItemType type;
    private string id;
    [ContextMenu("Generate custom ID")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }
    [TextArea(15, 20)]
    public string description;
    //public int dmgValue;
}

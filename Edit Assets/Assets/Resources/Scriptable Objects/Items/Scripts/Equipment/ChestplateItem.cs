using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chestplate Item", menuName = "Inventory System/Items/Chestplate")]
public class ChestplateItem : ItemObject
{
    public float healthBoost;
    public float manaBoost;
    public float staminaBoost; 
    
    public int Armor;
    public int Magicresist;
    private void Awake()
    {
        type = ItemType.Chestplate;
    }
}

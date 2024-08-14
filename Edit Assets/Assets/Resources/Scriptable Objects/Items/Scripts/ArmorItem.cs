using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armor Item", menuName = "Inventory System/Items/Armor")]
public class ArmorItem : ItemObject
{
    public int armorValue;
    //1 = 100%
    public float healthBoost;
    public float manaBoost;
    public float staminaBoost;
    private void Awake()
    {
        type = ItemType.Armor;
    }
}

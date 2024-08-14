using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gloves Item", menuName = "Inventory System/Items/Shield")]
public class ShieldItem : ItemObject
{
    public float healthBoost;
    public float manaBoost;
    public float staminaBoost;

    public int Armor;
    public int Magicresist;
    private void Awake()
    {
        type = ItemType.Shield;
    }
}

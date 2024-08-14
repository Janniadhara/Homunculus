using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shoes Item", menuName = "Inventory System/Items/Shoes")]
public class ShoesItem : ItemObject
{
    public float healthBoost;
    public float manaBoost;
    public float staminaBoost;

    public int Armor;
    public int Magicresist;
    private void Awake()
    {
        type = ItemType.Shoes;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pants Item", menuName = "Inventory System/Items/Pants")]
public class PantsItem : ItemObject
{
    public float healthBoost;
    public float manaBoost;
    public float staminaBoost;

    public int Armor;
    public int Magicresist;
    private void Awake()
    {
        type = ItemType.Pants;
    }
}

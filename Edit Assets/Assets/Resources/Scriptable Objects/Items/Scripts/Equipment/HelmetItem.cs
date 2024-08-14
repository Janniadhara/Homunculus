using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Helmet Item", menuName = "Inventory System/Items/Helmet")]
public class HelmetItem : ItemObject
{
    public float healthBoost;
    public float manaBoost;
    public float staminaBoost;

    public int Armor;
    public int Magicresist;
    private void Awake()
    {
        type = ItemType.Helmet;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Equipment Object")]
public class EquipmentObject : ScriptableObject
{
    public WeaponItem Weapon;
    public ShieldItem Shield;

    public HelmetItem Helmet;
    public ChestplateItem Chestplate;
    public PantsItem Pants;
    public ShoesItem Shoes;
    public GlovesItem Gloves;

    public RingItem Ring;
    public int testInt;
}

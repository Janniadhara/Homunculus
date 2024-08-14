using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeaponType
{
    OneHanded,
    TwoHanded,
    Spear,
    Bow,
    Staff
}
[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Inventory System/Items/Weapon")]
public class WeaponItem : ItemObject
{
    public int damageValue;
    public AnimatorOverrideController weaponAnimations;
    public WeaponType weaponType;
    private void Awake()
    {
        type = ItemType.Weapon;
    }
}

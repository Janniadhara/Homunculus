using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ring Item", menuName = "Inventory System/Items/Ring")]
public class RingItem : ItemObject
{
    public float healthBoost;
    public float manaBoost;
    public float staminaBoost;
    private void Awake()
    {
        type = ItemType.Ring;
    }
}

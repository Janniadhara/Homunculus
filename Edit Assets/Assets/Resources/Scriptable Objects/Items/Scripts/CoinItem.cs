using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food Item", menuName = "Inventory System/Items/Coin")]
public class CoinItem : ItemObject
{
    public int valueBronze;
    public int valueSilver;
    public int valueGold;
    private void Awake()
    {
        type = ItemType.Money;
    }
}

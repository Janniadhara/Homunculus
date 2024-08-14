using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Money
{
    private int requiredAmount = 150;
    public int curBronzeCoins;
    public int curSilverCoins;
    public int curGoldCoins;

    public void GainedCoins(int bronze, int silver, int gold)
    {
        curBronzeCoins += bronze;
        curSilverCoins += silver;
        curGoldCoins += gold;
        while (curBronzeCoins >= requiredAmount)
        {
            curBronzeCoins -= requiredAmount;
            curSilverCoins++;
        } 
        while (curSilverCoins >= requiredAmount)
        {
            curSilverCoins -= requiredAmount;
            curGoldCoins++;
        }
    }
}

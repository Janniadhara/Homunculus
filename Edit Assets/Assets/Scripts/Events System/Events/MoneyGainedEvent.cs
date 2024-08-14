using System;

public class MoneyGainedEvent
{
    public event Action<int, int, int> onGainMoney;
    public void GainMoney(int bronze, int silver, int gold)
    {
        if (onGainMoney != null)
        {
            onGainMoney(bronze, silver, gold);
        }
    }
}

using System;

public class LevelUpEvent
{
    public event Action<int> onGainXp;
    public void GainXp(int xp)
    {
        if (onGainXp != null)
        {
            onGainXp(xp);
        }
    }
    public event Action<int> onLevelUp;
    public void LevelUp(int level)
    {
        if (onLevelUp != null)
        {
            onLevelUp(level);
        }
    }
    public event Action<int, int> onChangeExp;
    public void ChangeExp(int curXp, int reqXp)
    {
        if (onChangeExp != null)
        {
            onChangeExp(curXp, reqXp);
        }
    }
}

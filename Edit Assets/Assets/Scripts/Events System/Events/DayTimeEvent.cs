using System;

public class DayTimeEvent
{
    public event Action onDayTime;
    public void Sunrise()
    {
        if (onDayTime != null)
        {
            onDayTime();
        }
    }
    public event Action onNightTime;
    public void Sunset()
    {
        if (onNightTime != null)
        {
            onNightTime();
        }
    }
}

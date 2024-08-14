using System;

public class KillEvent
{
    public event Action<string> onKillAnimal;
    public void KillAnimal(string animalName)
    {
        if (onKillAnimal != null)
        {
            onKillAnimal(animalName);
        }
    }
}

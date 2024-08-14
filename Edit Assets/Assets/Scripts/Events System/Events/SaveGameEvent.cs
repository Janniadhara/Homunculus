using System;

public class SaveGameEvent
{
    public event Action onSaveGame;
    public void SaveGame()
    {
        if (onSaveGame != null)
        {
            onSaveGame();
        }
    }
}

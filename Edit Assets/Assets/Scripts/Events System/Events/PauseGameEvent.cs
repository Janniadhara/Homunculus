using System;

public class PauseGameEvent
{
    public event Action<bool> onPauseGame;
    public void PauseGame(bool isPaused)
    {
        if (onPauseGame != null)
        {
            onPauseGame(isPaused);
        }
    }
}

using System;
using UnityEngine;

public class DetectPlayerEvent
{
    public event Action<GameObject, GameObject> onPlayerInRange;
    public void PlayerInRange(GameObject wildlife, GameObject player)
    {
        if (onPlayerInRange != null)
        {
            onPlayerInRange(wildlife, player);
        }
    }
    public event Action<GameObject> onPlayerOutRange;
    public void PlayerOutRange(GameObject wildlife)
    {
        if (onPlayerOutRange != null)
        {
            onPlayerOutRange(wildlife);
        }
    }
}

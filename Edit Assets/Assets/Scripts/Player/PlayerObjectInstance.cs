using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectInstance : MonoBehaviour
{
    public static PlayerObjectInstance Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Player.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }
}

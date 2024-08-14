using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagersParent : MonoBehaviour
{
    public static GameManagersParent Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Game Manager's parent.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(transform.gameObject);
    }
}

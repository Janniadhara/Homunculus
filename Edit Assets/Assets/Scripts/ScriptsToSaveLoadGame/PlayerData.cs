using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerData : MonoBehaviour, IDataPersistence
{
    public int count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //for testing
        if (Input.GetKeyDown(KeyCode.D))
        {
            count++;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            count--;
        }
    }

    public void LoadData(GameData data)
    {
        count = data.counted;
    }
    public void SaveData(ref GameData data)
    {
        data.counted = count;
    }
}

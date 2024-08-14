using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerModel : MonoBehaviour, IDataPersistence
{
    public bool isFemale;
    public int classCount;

    private GameObject PlayerModel;
    public void LoadData(GameData data)
    {
        Debug.Log("Loading Player Model.");
        isFemale = data.isFemale;
        classCount = data.classCount;

        PlayerModel = GameObject.FindGameObjectWithTag("PlayerModel");
        SetModel();
        Debug.Log("Loading Player Model completed.");
    }
    public void SaveData(ref GameData data)
    {
        data.isFemale = isFemale;
        data.classCount = classCount;
    }

    void SetModel()
    {
        int playerModelNumber;
        if (isFemale)
        {
            playerModelNumber = classCount;
        }
        else
        {
            playerModelNumber = classCount + 4;
        }
        for (int i = 0; i < 8; i++)
        {
            if (i != playerModelNumber)
            {
                Destroy(PlayerModel.transform.GetChild(i).gameObject);
            }
        }
    }
}

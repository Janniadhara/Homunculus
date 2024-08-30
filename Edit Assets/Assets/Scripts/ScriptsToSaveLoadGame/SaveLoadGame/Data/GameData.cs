using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int counted;
    public Vector3 spawnPositionSampleScene;
    public Vector3 spawnPositionDungeonSlime;
    public Vector3 spawnPositionDungeonGoblin;
    public SerializableDictionary<string, Vector3> PlayerPosition;

    //player character
    public bool isFemale;
    public int classCount;
    public bool isSaved;

    //player stats
    public int level;
    public int exp;
    public int bronzeCoins;
    public int silverCoins;
    public int goldCoins;
    //public int bloodCoins; //nice idea AI!

    //random NPC
    public SerializableDictionary<string, int> VillagerModels;

    //Inventories
    public List<InventorySlot> PlayerInventory;
    public SerializableDictionary<string, ChestData> ChestInventory;

    //Sound
    public float masterVolume;
    public float musicVolume;
    public float effectVolume;

    //Dungeons

    //Quests
    public SerializableDictionary<string, QuestData> QuestData;

    //dialogue variables
    public string GlobalDialogueVariables;

    //game statistics
    public SerializableDictionary<string, int> AnimalKills;

    //default Data when no SaveGame file
    public GameData()
    {
        counted = 0;
        spawnPositionSampleScene = new Vector3(-273.97f, 14.3f, -242.5f);
        spawnPositionDungeonSlime = new Vector3(-51.5229988f, 12.7360001f, 130.796997f);
        spawnPositionDungeonGoblin = new Vector3(2.43799996f, 0.40200001f, -25.5149994f);
        PlayerPosition = new SerializableDictionary<string, Vector3>();

        isFemale = true;
        classCount = 3;

        level = 1;
        exp = 0;
        bronzeCoins = 0;
        silverCoins = 0;
        goldCoins = 0;

        VillagerModels = new SerializableDictionary<string, int>();

        PlayerInventory = new List<InventorySlot>();
        ChestInventory = new SerializableDictionary<string, ChestData>();

        masterVolume = 0.2f;
        musicVolume = 0.2f;
        effectVolume = 0.2f;

        QuestData = new SerializableDictionary<string, QuestData>();

        GlobalDialogueVariables = "";

        AnimalKills = new SerializableDictionary<string, int>();
    }
}

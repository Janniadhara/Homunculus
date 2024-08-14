using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Config")]
    public bool loadSavedGameData;
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    public static DataPersistenceManager Instance { get; private set; }
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private GameData gameData;

    public ItemDatabaseObject Itemdatabase;
    public Object[] LoadedItems;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Data Persistence Manager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dataHandler = new FileDataHandler(Application.persistentDataPath + "/Saves", fileName);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        //EventsManager.Instance.saveGameEvent.onSaveGame += SaveGame;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        //EventsManager.Instance.saveGameEvent.onSaveGame -= SaveGame;
    }
    
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadItemDatabase();
        LoadGame();
        Debug.Log(SceneManager.GetActiveScene().name);
    }
    public void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("unloading current scene");
        SaveGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
        Debug.Log("creating a new game file");
    }

    public void LoadGame()
    {
        Debug.Log("Loading GameData.");
        if (loadSavedGameData)
        {
            //Load any saved data from a file unsing the data handler
            gameData = dataHandler.LoadGame();
        }
        else
        {
            gameData = null;
        }
        //if no data is found, just return and dont create a new game, else data wipe when starting
        if (gameData == null)
        {
            Debug.Log("Couldn't find a Savegame. Create a new Game first before loading.");
            //NewGame(); //for development
            return;
        }

        //push loaded data into scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
        Debug.Log("Loading GameData completed.");
    }

    public void SaveGame()
    {
        if (gameData == null)
        {
            Debug.Log("Couldn't find a Savegame. Create a new Game first before saving.");
            return;
        }
        //pass gameData to the scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        //save the data to a file using the data handler
        dataHandler.SaveGame(gameData);
        Debug.Log("Saving game...");
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private void LoadItemDatabase()
    {
        Debug.Log("Loading ItemDatabase.");
        LoadedItems = Resources.LoadAll("", typeof(ItemObject));
        Itemdatabase.ItemList.Clear();
        Itemdatabase.GetItemId.Clear();
        Itemdatabase.GetItem.Clear();
        for (int i = 0; i < LoadedItems.Length; i++)
        {
            Itemdatabase.ItemList.Add((ItemObject)LoadedItems[i]);
            Itemdatabase.GetItemId.Add(Itemdatabase.ItemList[i], Itemdatabase.ItemList[i].itemId);
            Itemdatabase.GetItem.Add(Itemdatabase.ItemList[i].itemId, Itemdatabase.ItemList[i]);
        }
        Debug.Log("Loading ItemDatabase completed.");
    }
    public bool HasGameData()
    {
        return gameData != null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsManager : MonoBehaviour, IDataPersistence
{
    public SerializableDictionary<string, int> AnimalsKilledStats;
    public void LoadData(GameData data)
    {
        AnimalsKilledStats = data.AnimalKills;
    }

    public void SaveData(ref GameData data)
    {
        data.AnimalKills = AnimalsKilledStats;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventsManager.Instance.killEvent.onKillAnimal += AnimalKilled;
    }

    private void AnimalKilled(string animal)
    {
        AnimalsKilledStats.TryGetValue(animal, out int killCount);
        killCount++;
        if (AnimalsKilledStats.ContainsKey(animal))
        {
            AnimalsKilledStats.Remove(animal);
        }
        AnimalsKilledStats.Add(animal, killCount);
    }
}

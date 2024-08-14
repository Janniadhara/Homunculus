using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFemaleNpc : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;
    [ContextMenu("Generate custom ID")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] GameObject[] FemaleVillagerPrefabs;
    private GameObject FemaleVillagerPrefab;
    public int modelNumber;

    public void LoadData(GameData data)
    {
        data.VillagerModels.TryGetValue(id, out modelNumber);
        RemoveChild();
        UpdateModel();
    }
    public void SaveData(ref GameData data)
    {
        if (data.VillagerModels.ContainsKey(id))
        {
            data.VillagerModels.Remove(id);
        }
        data.VillagerModels.Add(id, modelNumber);
    }

    void UpdateModel()
    {
        if (modelNumber == 0)
        {
            modelNumber = Random.Range(1, FemaleVillagerPrefabs.Length);
        }
        FemaleVillagerPrefab = Instantiate(FemaleVillagerPrefabs[modelNumber - 1]);
        FemaleVillagerPrefab.transform.SetParent(transform, false);
    }

    void RemoveChild()
    {
        GameObject.Destroy(transform.GetChild(0).gameObject);
    }
}

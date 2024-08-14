using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaleNpc : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;
    [ContextMenu("Generate custom ID")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] GameObject[] MaleVillagerPrefabs;
    private GameObject MaleVillagerPrefab;
    public Animator MaleAnimator;
    [SerializeField] private int animNumber;
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
            modelNumber = Random.Range(1, MaleVillagerPrefabs.Length);
        }
        MaleVillagerPrefab = Instantiate(MaleVillagerPrefabs[modelNumber - 1]);
        MaleVillagerPrefab.transform.SetParent(transform, false);
        MaleAnimator = MaleVillagerPrefab.GetComponent<Animator>();
        MaleAnimator.SetInteger("anim", animNumber);
    }

    void RemoveChild()
    {
        GameObject.Destroy(transform.GetChild(0).gameObject);
    }
}

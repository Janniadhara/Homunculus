using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassicveMobSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] WildlifePrefab;
    [SerializeField] private bool spawnWildlife;
    [SerializeField] private bool preSpawn;
    [SerializeField] private float spawnDelay = 10;
    private float timeSinceLastSpawn;
    [SerializeField] private int MinLevel;
    [SerializeField] private int MaxLevel;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private float spawnRange;
    [SerializeField] private float maxTravelDistance;
    void Start()
    {
        timeSinceLastSpawn = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        if (preSpawn)
        {
            for (int i = 0; i < maxSpawnCount; i++)
            {
                Spawn();
            }
        }
    }

    void Update()
    {
        if (spawnWildlife)
        {
            if (transform.childCount < maxSpawnCount)
            {
                timeSinceLastSpawn += Time.deltaTime;
                if (timeSinceLastSpawn > spawnDelay)
                {
                    timeSinceLastSpawn = 0;
                    Spawn();
                }
            }
        }
    }
    private void Spawn()
    {
        float x = Random.Range(-spawnRange, spawnRange);
        float z = Random.Range(-spawnRange, spawnRange);
        int prefabNumber = Random.Range(0, WildlifePrefab.Length);
        Vector3 randomSpawnLocation = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        GameObject Animal = Instantiate(WildlifePrefab[prefabNumber], randomSpawnLocation, transform.rotation, transform);
        Animal.GetComponent<PassiveMobStats>().WildliveLevel = Random.Range(MinLevel, MaxLevel + 1);
        Animal.GetComponent<PassiveMobStats>().SetValues();
        Animal.GetComponent<PassiveMobMovement>().MaxDistance = maxTravelDistance;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnRange * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTravelDistance);
    }
}

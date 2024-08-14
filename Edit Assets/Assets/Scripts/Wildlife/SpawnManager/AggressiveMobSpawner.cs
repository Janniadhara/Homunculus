using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveMobSpawner : MonoBehaviour
{
    [SerializeField] private bool animalUseTerrain;
    [Header("Animal Stats")]
    [SerializeField] private GameObject[] WildlifePrefab;
    [SerializeField] private bool spawnWildlife;
    [SerializeField] private bool spawnAll;
    [SerializeField] private float spawnDelay;
    private float timeSinceLastSpawn;
    [SerializeField] private int MinLevel;
    [SerializeField] private int MaxLevel;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private float spawnRange;
    [Header("Roaming Animals")]
    [SerializeField] private float maxTravelDistance;
    [Header("Patrolling Animals")]
    [SerializeField] private GameObject WaypointPrefab;
    [SerializeField] private bool arePatrolling;
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        if (spawnAll)
        {
            for (int i = 0; i < maxSpawnCount; i++)
            {
                Spawn();
            }
        }
        else
        {
            Spawn();
        }
    }

    void Update()
    {
        if (spawnWildlife)
        {
            if (transform.childCount < maxSpawnCount)
            {
                timeSinceLastSpawn += Time.deltaTime;
                if (timeSinceLastSpawn >= spawnDelay)
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
        SetAnimalStats(Animal);
    }
    private void SetAnimalStats(GameObject Animal)
    {
        Animal.GetComponent<AggressiveMobStats>().WildliveLevel = Random.Range(MinLevel, MaxLevel + 1);
        Animal.GetComponent<AggressiveMobStats>().SetValues();
        Animal.GetComponent<AggressiveMobMovement>().MaxDistance = maxTravelDistance;
        Animal.GetComponent<AggressiveMobMovement>().useTerrain = animalUseTerrain;
        Animal.GetComponent<AggressiveMobMovement>().followWaypoints = false;
        if (WaypointPrefab != null)
        {
            Animal.GetComponent<AggressiveMobMovement>().waypoints = WaypointPrefab.GetComponent<WayPoints>();
            Animal.GetComponent<AggressiveMobMovement>().followWaypoints = true;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnRange * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTravelDistance);
    }
}

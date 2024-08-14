using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ShrineSlimeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] SlimePrefab;
    [SerializeField] private bool spawnSlimes;
    [SerializeField] private float spawnDelay = 10;
    [SerializeField] private int MinLevel;
    [SerializeField] private int MaxLevel;
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private float spawnRange;
    [SerializeField] private float maxTravelDistance;
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        for (int i = 0;i < maxSpawnCount; i++)
        {
            SpawnSlime();
        }
    }

    void Update()
    {
        if (spawnSlimes)
        {
            if (transform.childCount < maxSpawnCount)
            {
                spawnDelay -= Time.deltaTime;
                if (spawnDelay < 0)
                {
                    spawnDelay = 10;
                    SpawnSlime();
                }
            }
        }
    }
    private void SpawnSlime()
    {
        float x = Random.Range(-spawnRange, spawnRange + 1);
        float z = Random.Range(-spawnRange, spawnRange + 1);
        int prefabNumber = Random.Range(0, SlimePrefab.Length);
        Vector3 randomSpawnLocation = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        GameObject Slime = Instantiate(SlimePrefab[prefabNumber], randomSpawnLocation, transform.rotation, transform);
        Slime.GetComponent<SlimeHealth>().SlimeLevel = Random.Range(MinLevel, MaxLevel + 1);
        Slime.GetComponent<SlimeHealth>().SetValues();
        Slime.GetComponent<SlimeMovement>().maxDistance = maxTravelDistance;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, Vector3.one * spawnRange * 2);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxTravelDistance);
    }
}

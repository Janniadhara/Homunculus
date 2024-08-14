using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawningVase : MonoBehaviour
{
    [SerializeField] private GameObject BronzeCoinPrefab;
    [SerializeField] private Transform BronzeCoinParent;
    [SerializeField] private int maxBronzeCount;
    private int curBronzeCount;
    [SerializeField] private GameObject SilverCoinPrefab;
    [SerializeField] private Transform SilverCoinParent;
    [SerializeField] private int maxSilverCount;
    private int curSilverCount;
    [SerializeField] private GameObject GoldCoinPrefab;
    [SerializeField] private Transform GoldCoinParent;
    [SerializeField] private int maxGoldCount;
    private int curGoldCount;

    [SerializeField] private float spawnInterval;
    private float spawnTime = 0.2f;
    private void Update()
    {
        spawnTime -= Time.deltaTime;
        if (spawnTime < 0)
        {
            spawnTime = spawnInterval;
            SpawnBronzeCoin();
            SpawnSilverCoin();
            SpawnGoldCoin();
        }
    }
    private void SpawnBronzeCoin()
    {
        curBronzeCount = BronzeCoinParent.transform.childCount;
        if (curBronzeCount < maxBronzeCount)
        {
            Instantiate(BronzeCoinPrefab, transform.position + Vector3.right * 2, Quaternion.Euler(0, 0, 0), BronzeCoinParent);
        }
    }
    private void SpawnSilverCoin()
    {
        curSilverCount = SilverCoinParent.transform.childCount;
        if (curSilverCount < maxSilverCount)
        {
            Instantiate(SilverCoinPrefab, transform.position + Vector3.right * 2, Quaternion.Euler(0, 0, 0), SilverCoinParent);
        }
    }
    private void SpawnGoldCoin()
    {
        curGoldCount = GoldCoinParent.transform.childCount;
        if (curGoldCount < maxGoldCount)
        {
            Instantiate(GoldCoinPrefab, transform.position + Vector3.right * 2, Quaternion.Euler(0, 0, 0), GoldCoinParent);
        }
    }

}

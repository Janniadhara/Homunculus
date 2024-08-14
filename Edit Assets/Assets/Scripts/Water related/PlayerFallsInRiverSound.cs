using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallsInRiverSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] waterSplash;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.PlayEffect(waterSplash[Random.Range(0, waterSplash.Length)], 0.8f);
        }
    }
}

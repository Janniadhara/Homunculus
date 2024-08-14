using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private AudioClip scream;
    [SerializeField] private AudioClip attackSound;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        audioSource.volume = SoundManager.Instance.effectVolume;
    }
    void StepSound()
    {
        audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length)]);
    }
    void Scream()
    {
        audioSource.PlayOneShot(scream);
    }
    void Attack()
    {
        audioSource.PlayOneShot(attackSound);
    }
}

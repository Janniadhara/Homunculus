using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CementaryMusic : MonoBehaviour
{
    [SerializeField] private ParticleSystem FogParticles;
    [SerializeField] private ParticleSystemStopBehavior FogStopBehavior;
    private float minDensity = 0.005f;
    private float maxDensity = 0.07f;
    private bool entered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.EnterCementary();
            TimeManager.Instance.EnterExtiCementary();
            FogParticles.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.EnterCementary();
            TimeManager.Instance.EnterExtiCementary();
            //FogParticles.isEmitting = false;
            FogParticles.Stop();

        }
    }
}

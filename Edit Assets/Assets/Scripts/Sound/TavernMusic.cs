using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernMusic : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.EnterTavern();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.EnterTavern();
        }
    }
}

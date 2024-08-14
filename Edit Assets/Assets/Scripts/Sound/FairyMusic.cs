using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyMusic : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.EnterFairy();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance.EnterFairy();
        }
    }
}

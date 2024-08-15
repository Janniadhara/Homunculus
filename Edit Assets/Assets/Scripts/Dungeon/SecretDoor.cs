using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDoor : MonoBehaviour
{
    [SerializeField] private AudioClip doorClip;
    public void PlayDoorSound()
    {
        SoundManager.Instance.PlayEffect(doorClip, 1f);
    }
}

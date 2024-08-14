using System;
using UnityEngine;

public class PlaySoundEvent
{
    public event Action<AudioClip, float> onPlayOneShot;
    public void PlayOneShot(AudioClip clip, float volume)
    {
        if (onPlayOneShot != null)
        {
            onPlayOneShot(clip, volume);
        }
    }
}

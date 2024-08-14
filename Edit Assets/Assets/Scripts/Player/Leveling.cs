using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Leveling
{
    public int curLevel;
    public int curExp;
    public int neededExp;
    [HideInInspector]
    public int maxLevel = 100;

    [SerializeField, Tooltip("Needed Exp for the next level: curLevel / x ^ y. The lower this number the higher the required Exp for the next level.")] 
    private float x = 0.05f;
    [SerializeField, Tooltip("Needed Exp for the next level: curLevel / x ^ y. The higher this number the higher the required Exp for the next level.")] 
    private float y = 1.0f;

    [SerializeField] private ParticleSystem LvUpParticles;
    private ParticleSystem LvUpParticlesPrefab;
    [SerializeField] private AudioClip clip;

    public void GainedExp (int gainedExp)
    {
        curExp += gainedExp;

        while (curExp >= neededExp)
        {
            if (curLevel >= maxLevel)
            {
                break;
            }
            curExp -= neededExp;
            curLevel += 1;
            CalculateExp();
            if (LvUpParticlesPrefab == null)
            {
                LvUpParticlesPrefab = GameObject.Instantiate(LvUpParticles, GameObject.FindGameObjectWithTag("Player").transform, false);
            }
            LvUpParticlesPrefab.Play();
            EventsManager.Instance.levelUpEvent.LevelUp(curLevel);
            //EventsManager.Instance.playSoundEvent.PlayOneShot(clip, 1);
        }
    }
    public void CalculateExp()
    {
        neededExp = (int)Mathf.Pow(curLevel / x, y);
        if (neededExp <= 0)
        {
            neededExp = 1;
        }
    }
}

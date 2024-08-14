using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Sound Data Object", menuName = "")]
public class PlayerSoundDataObject : ScriptableObject
{
    [Header("Walking")]
    public AudioClip[] WalkingGrass;
    public AudioClip[] WalkingGravel;
    public AudioClip[] WalkingWood;
    public AudioClip[] WalkingStone;
    public AudioClip[] WalkingDirt;
    public AudioClip[] WalkingLeaves;
    public AudioClip[] WalkingWet;

    [Header("Running")]
    public AudioClip[] RunningGrass;
    public AudioClip[] RunningGravel;
    public AudioClip[] RunningWood;
    public AudioClip[] RunningStone;
    public AudioClip[] RunningDirt;
    public AudioClip[] RunningLeaves;
    public AudioClip[] RunningWet;

    [Header("Jumping")]
    public AudioClip[] JumpingGrass;
    public AudioClip[] JumpingGravel;
    public AudioClip[] JumpingWood;
    public AudioClip[] JumpingStone;
    public AudioClip[] JumpingDirt;
    public AudioClip[] JumpingLeaves;
    public AudioClip[] JumpingWet;

    [Header("Landing")]
    public AudioClip[] LandingGrass;
    public AudioClip[] LandingGravel;
    public AudioClip[] LandingWood;
    public AudioClip[] LandingStone;
    public AudioClip[] LandingDirt;
    public AudioClip[] LandingLeaves;
    public AudioClip[] LandingWet;

    [Header("Swimming")]
    public AudioClip[] Swimming;
}

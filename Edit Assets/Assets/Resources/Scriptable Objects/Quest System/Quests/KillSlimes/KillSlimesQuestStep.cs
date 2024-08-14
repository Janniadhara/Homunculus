using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KillSlimesQuestStep : QuestStep
{
    [SerializeField] private int killsNeeded;
    public int amountKilled;
    private void OnEnable()
    {
        EventsManager.Instance.killEvent.onKillAnimal += AnimalKilled;
    }
    private void OnDisable()
    {
        EventsManager.Instance.killEvent.onKillAnimal -= AnimalKilled;
    }
    private void AnimalKilled(string animalName)
    {
        if (animalName == "Slime")
        {
            amountKilled++;
            UpdateState();
        }
        if (amountKilled >= killsNeeded)
        {
            FinishQuestStep();
        }
    }
    private void UpdateState()
    {
        string state = amountKilled.ToString();
        ChangeState(state, killsNeeded.ToString(), "");
    }
    protected override void SetQuestStepState(string newState)
    {
        amountKilled = System.Int32.Parse(newState);
        UpdateState();
    } 
}

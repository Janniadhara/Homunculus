using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringApplesQuestStep : QuestStep
{
    protected override void SetQuestStepState(string newState)
    {
        //no state is needed for this quest step
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FinishQuestStep();
        }
    }
}

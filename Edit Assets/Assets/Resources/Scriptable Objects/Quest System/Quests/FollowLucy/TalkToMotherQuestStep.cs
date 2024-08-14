using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToMotherQuestStep : QuestStep
{
    private void OnEnable()
    {
        EventsManager.Instance.questEvents.onFinishStep += FinishStep;
    }
    private void OnDisable()
    {
        EventsManager.Instance.questEvents.onFinishStep -= FinishStep;
    }
    void FinishStep(string id)
    {
        if (id == "TalkToMother")
        {
            FinishQuestStep();
        }
    }
    protected override void SetQuestStepState(string newState)
    {
        //no state is needed for this quest step
    }
}

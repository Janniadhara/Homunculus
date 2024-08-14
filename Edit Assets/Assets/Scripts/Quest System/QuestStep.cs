using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    public string questId;
    private int questStepIndex;
    public bool checkInventory;
    public string questStepName;
    public void InitializeQuestStep(string id, int stepIndex, string questStepState)
    {
        questId = id;
        questStepIndex = stepIndex;
        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            EventsManager.Instance.questEvents.AdvanceQuest(questId);
            Destroy(gameObject);
        }    
    }
    protected void ChangeState(string newState, string requiredState, string jText)
    {
        EventsManager.Instance.questEvents.QuestStepStateChange(questId, questStepIndex, new QuestStepState(newState, requiredState, jText));
    }
    protected abstract void SetQuestStepState(string newState);
}

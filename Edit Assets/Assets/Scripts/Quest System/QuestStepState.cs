using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStepState
{
    public string questState;
    public string requiredState;
    public string journalText;
    public QuestStepState(string state, string required, string jText)
    {
        questState = state;
        requiredState = required;
        journalText = jText;
    }
    public QuestStepState()
    {
        questState = "";
    }
}

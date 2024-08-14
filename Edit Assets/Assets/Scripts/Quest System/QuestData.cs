using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public QuestState questState;
    public int questStepIndex;
    public QuestStepState[] questStepStates;
    public QuestData(QuestState state, int questStepIndex, QuestStepState[] questStepStates)
    {
        questState = state;
        this.questStepIndex = questStepIndex;
        this.questStepStates = questStepStates;
    }
}

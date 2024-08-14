using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfo info;

    public QuestState state;
    private int curQuestStepIndex;
    private QuestStepState[] questStepStates;

    public Quest(QuestInfo questInfo)
    {
        info = questInfo;
        state = QuestState.REQUIREMENTS_NOT_MET;
        curQuestStepIndex = 0;
        questStepStates = new QuestStepState[info.questStepPrefabs.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new QuestStepState();
        }
    }
    public Quest(QuestInfo questInfo, QuestState questState, int curQuestStepIndex, QuestStepState[] questStepStates)
    {
        info = questInfo;
        state = questState;
        this.curQuestStepIndex = curQuestStepIndex;
        this.questStepStates = questStepStates;

        if (this.questStepStates.Length != this.info.questStepPrefabs.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are of different lenghts: " + info.id
                + ". You might need to reset your Data if you have changed something in development");
        }
    }
    public void MoveToNextStep()
    {
        curQuestStepIndex++;
    }
    public bool CurStepExists()
    {
        return (curQuestStepIndex < info.questStepPrefabs.Length);
    }
    public void InstantiateCurrentQuestStep(Transform parentTransform, bool checkPlayerInv)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform).GetComponent<QuestStep>();
            questStep.checkInventory = checkPlayerInv;
            questStep.InitializeQuestStep(info.id, curQuestStepIndex, questStepStates[curQuestStepIndex].questState);
        }
    }
    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurStepExists())
        {
            questStepPrefab = info.questStepPrefabs[curQuestStepIndex];
        }
        else 
        {
            Debug.LogWarning("Tried to get quest step prefab but setIndex was out of range");
        }
        return questStepPrefab;
    }
    public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].questState = questStepState.questState;
        }
        else
        {
            Debug.LogWarning("stepIndex was out of range: " + info.id + " / " + stepIndex);
        }
    }
    public QuestData GetQuestData()
    {
        return new QuestData(state, curQuestStepIndex, questStepStates);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateJournal : MonoBehaviour
{
    public QuestInfo[] Quests;
    private List<JournalEntry> JournalEntries = new List<JournalEntry>();
    public JournalEntry test;
    private void OnEnable()
    {
        EventsManager.Instance.questEvents.onQuestStateChange += UpdateJournalEntry;
        EventsManager.Instance.questEvents.onQuestStepStateChange += UpdateQuestStep;

    }
    void Start()
    {
        Quests = Resources.LoadAll<QuestInfo>("Scriptable Objects/Quest System/Quests");
    }  
    private void UpdateJournalEntry(Quest quest)
    {
        if (quest.state == QuestState.IN_PROGRESS)
        {
            QuestInfo questInfo = GetQuestInfo(quest);
            AddQuestToJournal(questInfo);
        }
    }
    private QuestInfo GetQuestInfo(Quest quest)
    {
        for (int i = 0; i < Quests.Length; i++)
        {
            if (quest.info == Quests[i])
            {
                return Quests[i];
            }
        }
        Debug.LogWarning("Couldn't get QuestInfo: " + quest.info);
        return null;
    }

    private void AddQuestToJournal(QuestInfo questInfo)
    {
        for (int i = 0; i < JournalEntries.Count; i++)
        {
            if (JournalEntries[i].questId == questInfo.id)
            {
                return;
            }
        }
        JournalEntry journalEntry = new JournalEntry();
        journalEntry.questId = questInfo.id;
        journalEntry.questName = questInfo.displayName;
        journalEntry.questDescription = questInfo.description;
        JournalEntries.Add(journalEntry);
        Debug.Log(journalEntry.questId + ", " + journalEntry.questName + ", " + journalEntry.questDescription);
        //string questStepState = questInfo.questStepPrefabs[0].transform.GetComponent<QuestStep>().;

    }
    private void UpdateQuestStep(string id, int stepIndex, QuestStepState stepState)
    {
        for (int i = 0; i < JournalEntries.Count; i++)
        {
            if (id == JournalEntries[i].questId)
            {
                JournalEntries[i].questStepIndex = stepIndex;
                JournalEntries[i].questStepState = System.Int32.Parse(stepState.questState);
                JournalEntries[i].requiredStepState = System.Int32.Parse(stepState.requiredState);
                JournalEntries[i].questStepStateText = stepState.journalText;
                Debug.Log(JournalEntries[i].questId + " step updated");
                test = JournalEntries[i];
            }
        }
    }
}

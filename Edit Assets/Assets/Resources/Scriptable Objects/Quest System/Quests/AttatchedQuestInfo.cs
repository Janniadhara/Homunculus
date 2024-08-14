using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttatchedQuestInfo : MonoBehaviour
{
    public QuestInfo QuestInfo;
    public void DisplayQuest()
    {
        QuestManager.Instance.ShowQuestInJournal(QuestInfo);
    }
}

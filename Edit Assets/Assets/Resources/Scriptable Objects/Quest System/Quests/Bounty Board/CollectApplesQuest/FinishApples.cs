using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishApples : MonoBehaviour
{
    private void OnEnable()
    {
        EventsManager.Instance.questEvents.onQuestStateChange += CheckIfFinish;
    }
    private void CheckIfFinish(Quest quest)
    {
        if (quest.info.id == "CollectApples" && quest.state == QuestState.CAN_FINISH)
        {
            EventsManager.Instance.questEvents.FinishQuest("CollectApples");
            //Destroy(gameObject);
        }
        if (quest.info.id == "CollectBread" && quest.state == QuestState.CAN_FINISH)
        {
            EventsManager.Instance.questEvents.FinishQuest("CollectBread");
            //Destroy(gameObject);
        }
        if (quest.info.id == "CollectPotions" && quest.state == QuestState.CAN_FINISH)
        {
            EventsManager.Instance.questEvents.FinishQuest("CollectPotions");
            //Destroy(gameObject);
        }
        if (quest.info.id == "VisitRocksOutsideTown" && quest.state == QuestState.CAN_FINISH)
        {
            EventsManager.Instance.questEvents.FinishQuest("VisitRocksOutsideTown");
            //Destroy(gameObject);
        }
        if (quest.info.id == "CollectApples" && quest.state == QuestState.FINISHED)
        {
            Debug.Log("finished apples dfawefwas and destroy");
            //Destroy(gameObject);
        }
    }
}

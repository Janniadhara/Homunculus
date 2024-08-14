using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBoard : MonoBehaviour
{
    [Header("3d Object")]
    [SerializeField] private GameObject NewQuestIndicator;
    [SerializeField] private QuestInfo[] RepeatedQuests;
    [SerializeField] private GameObject QuestDisplayPrefab;
    private GameObject QuestBoardDisplay;
    [SerializeField] private List<QuestInfo> Quests = new List<QuestInfo>();
    [SerializeField] private Quest QuestOnBoard;
    [SerializeField] private int questIndex;
    [SerializeField] private int newQuestCount = 0;
    private void Awake()
    {
        //RemoveChildren();
    }
    private void OnEnable()
    {
        EventsManager.Instance.questEvents.onQuestStateChange += CheckQuestState;
    }
    private void OnDisable()
    {
        EventsManager.Instance.questEvents.onQuestStateChange -= CheckQuestState;
    }
    private void CheckQuestState(Quest quest)
    {
        QuestBoardDisplay = QuestManager.Instance.GetQuestBoardScreen();
        for (int i = 0; i < RepeatedQuests.Length; i++)
        {
            if (quest.info.id == RepeatedQuests[i].id && quest.state == QuestState.CAN_START)
            {
                Quests.Add(quest.info);
                if (CheckOpenSlots())
                {
                    AddPrefab(Quests[QuestBoardDisplay.transform.childCount], quest.state);
                }
            }
            else if (quest.info.id == RepeatedQuests[i].id && quest.state == QuestState.IN_PROGRESS)
            {
                questIndex = Quests.IndexOf(quest.info);
                if (questIndex < 0)
                {
                    //todo check if quest is already in Quests list. if not add, else continue
                    Quests.Add(quest.info);
                    AddPrefab(Quests[0], quest.state);
                }
            }
            else if (quest.info.id == RepeatedQuests[i].id && quest.state == QuestState.FINISHED)
            {
                if (Quests.Contains(quest.info))
                {
                    questIndex = Quests.IndexOf(quest.info);
                    Debug.Log(questIndex + " dmkeofwsefn");
                    Destroy(QuestBoardDisplay.transform.GetChild(questIndex).gameObject);
                    Quests.Remove(quest.info);
                }
            }
        }
        if (newQuestCount > 0)
        {
            NewQuestIndicator.SetActive(true);
        }
        else
        {
            NewQuestIndicator.SetActive(false);
        }
    }
    private bool CheckOpenSlots()
    {
        if (Quests.Count >= QuestBoardDisplay.transform.childCount)
        {
            return true;
        }
        return false;
    }
    private void AddPrefab(QuestInfo questInfo, QuestState questState)
    {
        GameObject newDisplay = Instantiate(QuestDisplayPrefab, QuestBoardDisplay.transform);
        newDisplay.transform.GetComponent<QuestBoardQuest>().info = questInfo;
        newDisplay.transform.GetChild(0).GetComponent<Text>().text = questInfo.displayName;
        newDisplay.transform.GetChild(1).GetComponent<Text>().text = questInfo.description;
        newDisplay.transform.GetChild(2).GetComponent<Text>().text = "~ " + questInfo.questGiver;
        newDisplay.transform.GetChild(3).gameObject.SetActive(false);
        if (questState != QuestState.IN_PROGRESS)
        {
            newDisplay.transform.GetChild(3).gameObject.SetActive(true);
            newDisplay.transform.GetChild(3).GetComponent<Button>().onClick.AddListener((UnityEngine.Events.UnityAction)delegate
            {
                newQuestCount--;
                EventsManager.Instance.questEvents.StartQuest(questInfo.id);
                newDisplay.transform.GetChild(3).gameObject.SetActive(false);
            });
        }
        newQuestCount++;
    }
    private void RemoveChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

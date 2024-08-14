using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfo QuestInfoForQuestpoint;
    private string questId;
    private QuestState curQuestState;
    [SerializeField] private bool playerIsNear = false;
    [Header("Config")]
    [SerializeField] private bool startPoint;
    [SerializeField] private bool finishPoint;
    [SerializeField] private bool finishQuestRemote;
    [SerializeField] private Image StartQuestMarker;
    [SerializeField] private Image InProgressMarker;

    private void Awake()
    {
        questId = QuestInfoForQuestpoint.id;
        StartQuestMarker.enabled = false;
        InProgressMarker.enabled = false;
    }
    private void OnEnable()
    {
        EventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
    }
    private void OnDisable()
    {
        EventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }
    private void Update()
    {
        if (finishPoint && finishQuestRemote)
        {
            if (curQuestState.Equals(QuestState.CAN_FINISH))
            {
                EventsManager.Instance.questEvents.FinishQuest(questId);
            }
        }
    }
    private void QuestStateChange(Quest quest)
    {
        //only update the quest if this point has the corresponding quest
        if (quest.info.id == questId)
        {
            curQuestState = quest.state;
            if (quest.state == QuestState.CAN_START && startPoint)
            {
                StartQuestMarker.enabled = true;
            }
            else if (quest.state == QuestState.IN_PROGRESS)
            {
                StartQuestMarker.enabled = false;
                InProgressMarker.enabled = true;
            }
            else if (quest.state == QuestState.CAN_FINISH && finishPoint)
            {
                StartQuestMarker.enabled = false;
                InProgressMarker.enabled = true;
            }
            else if (quest.state == QuestState.FINISHED)
            {
                StartQuestMarker.enabled = false;
                InProgressMarker.enabled = false;
            }
        }
    }
    public void StartQuest()
    {
        EventsManager.Instance.questEvents.StartQuest(questId);
    }
    public void FinishQuest()
    {
        EventsManager.Instance.questEvents.FinishQuest(questId);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (curQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
            {
                EventsManager.Instance.questEvents.FinishQuest(questId);
            }
        }
        //EventsManager.Instance.questEvents.AdvanceQuest(questId);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}

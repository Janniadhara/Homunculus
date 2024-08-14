using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour, IDataPersistence
{
    public static QuestManager Instance;

    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;
    public QuestInfo[] Quests;
    private Dictionary<string, Quest> QuestMap;
    public List<QuestInfo> QuestsInProgress;
    public List<QuestInfo> QuestsCompleted;
    private int curLevel;
    [Header("Quest Board")]
    [SerializeField] private GameObject QuestBoardDisplay;
    [Header("QuestJournal")]
    [SerializeField] private GameObject JournalScreen;
    [SerializeField] private GameObject QuestsInProgressView;
    [SerializeField] private GameObject QuestsCompletedView;
    [SerializeField] private GameObject QuestStepsView; 
    [SerializeField] private GameObject QuestButtonPrefab;
    [SerializeField] private GameObject QuestStepsPrefab;
    [SerializeField] private Text QuestNameText;
    [SerializeField] private Text QuestDescriptionText;
    [SerializeField] private Text QuestRewardText;


    private void OnEnable()
    {
        EventsManager.Instance.questEvents.onStartQuest += StartQuest;
        EventsManager.Instance.questEvents.onAdvanceQuest += AdvanceQuest;
        EventsManager.Instance.questEvents.onFinishQuest += FinishQuest;

        EventsManager.Instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        EventsManager.Instance.levelUpEvent.onLevelUp += ChangePlayerLevel;

        EventsManager.Instance.pauseGameEvent.onPauseGame += PauseGame;
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Quest Manager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        QuestBoardDisplay.SetActive(false);
        JournalScreen.SetActive(false);
    }
    private void OnDisable()
    {
        EventsManager.Instance.questEvents.onStartQuest -= StartQuest;
        EventsManager.Instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        EventsManager.Instance.questEvents.onFinishQuest -= FinishQuest;

        EventsManager.Instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        EventsManager.Instance.levelUpEvent.onLevelUp -= ChangePlayerLevel;
    }
    public GameObject GetQuestBoardScreen()
    {
        return QuestBoardDisplay;
    }
    public void OpenQuestBoardScreen()
    {
        QuestBoardDisplay.SetActive(true);
    }
    public void CloseQuestBoardScreen()
    {
        QuestBoardDisplay.SetActive(false);
    }
    private void PauseGame(bool isPaused)
    {
        if (QuestBoardDisplay.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        EventsManager.Instance.questEvents.QuestStateChange(quest);
        Debug.Log(quest.info.id + " : " + quest.state);
    }
    private void ChangePlayerLevel(int level)
    {
        curLevel = level;
        //check if with the new level player can start new quests
        foreach (Quest quest in QuestMap.Values)
        {
            //if meting the required level start the quest
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }
    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;
        if (curLevel < quest.info.levelRequired)
        {
            meetsRequirements = false;
        }
        foreach (QuestInfo prerequisitedQuests in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisitedQuests.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }
        return meetsRequirements;
    }
    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform, true);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);


        QuestsInProgress.Add(quest.info);
    }
    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        //move to next step
        quest.MoveToNextStep();
        //if there are more steps, instantiate the next one
        if (quest.CurStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform, true);
        }
        //if there are no more steps, we finished all
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }
    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED); 
        Debug.Log("Finished Quest: " + id);
        QuestsCompleted.Add(quest.info);
        QuestsInProgress.Remove(quest.info);
        //check if with the new finished quest can start new quests
        foreach (Quest newQuest in QuestMap.Values)
        {
            //if meting the requirements start the quest
            if (newQuest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(newQuest))
            {
                ChangeQuestState(newQuest.info.id, QuestState.CAN_START);
                Debug.Log("Can Start new Quest: " + newQuest.info.id);
            }
        }
    }
    private void ClaimRewards(Quest quest)
    {
        EventsManager.Instance.levelUpEvent.GainXp(quest.info.xpReward);
        EventsManager.Instance.moneyGainedEvent.GainMoney(quest.info.bronzeReward, quest.info.silverReward, quest.info.goldReward);
        Debug.Log("Money gained: " 
            + quest.info.bronzeReward + " Bronze, " 
            + quest.info.silverReward + " Silver, " 
            + quest.info.goldReward + " Gold.");
    }
    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }
    private void CreateQuestMap()
    {
        QuestMap.Clear();
        QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Scriptable Objects/Quest System/Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfo questInfo in allQuests)
        {
            if (QuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate QuestID found:" + questInfo.id);
                QuestMap.Remove(questInfo.id);
            }
            QuestMap.Add(questInfo.id, new Quest(questInfo));
        }
    }
    private Quest GetQuestById(string id)
    {
        Quest quest = QuestMap[id];
        if (quest == null)
        {
            Debug.LogError("QuestId not found:" + id);
        }
        return quest;
    }

    public void OpenJournalScreen()
    {
        RemoveJournalChildren();
        QuestNameText.text = "";
        QuestDescriptionText.text = "";
        QuestRewardText.text = "";
        for (int i = 0; i < QuestsInProgress.Count; i++)
        {
            GameObject button = Instantiate(QuestButtonPrefab, QuestsInProgressView.transform, false);
            QuestInfo questInfo = QuestsInProgress[i];
            button.GetComponent<AttatchedQuestInfo>().QuestInfo = questInfo;
            button.GetComponentInChildren<Text>().text = questInfo.displayName;
        }
        for (int i = 0; i < QuestsCompleted.Count; i++)
        {
            GameObject button = Instantiate(QuestButtonPrefab, QuestsCompletedView.transform, false);
            QuestInfo questInfo = QuestsCompleted[i];
            button.GetComponent<AttatchedQuestInfo>().QuestInfo = questInfo;
            button.GetComponentInChildren<Text>().text = questInfo.displayName;
        }
        JournalScreen.SetActive(true);
    }
    public void CloseJournalScreen()
    {
        JournalScreen.SetActive(false);
    }
    private void RemoveJournalChildren()
    {
        for (int i = 0; i < QuestsInProgressView.transform.childCount; i++)
        {
            Destroy(QuestsInProgressView.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < QuestsCompletedView.transform.childCount; i++)
        {
            Destroy(QuestsCompletedView.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < QuestStepsView.transform.childCount; i++)
        {
            Destroy(QuestStepsView.transform.GetChild(i).gameObject);
        }
    }
    public void ShowQuestInJournal(QuestInfo questInfo)
    {
        Quest quest = GetQuestById(questInfo.id);
        QuestData questData = quest.GetQuestData();

        QuestNameText.text = questInfo.displayName;
        QuestDescriptionText.text = questInfo.description;
        QuestRewardText.text = "Reward: " 
            + questInfo.goldReward + " Gold, " 
            + questInfo.silverReward + " Silver, "
            + questInfo.bronzeReward + " Bronze";
        for (int i = 0; i < QuestStepsView.transform.childCount; i++)
        {
            Destroy(QuestStepsView.transform.GetChild(i).gameObject);
        }
        if (questData.questStepIndex < questInfo.questStepPrefabs.Length)
        {
            for (int i = 0; i <= questData.questStepIndex; i++)
            {
                GameObject questStep = Instantiate(QuestStepsPrefab, QuestStepsView.transform, false);
                if (i < questData.questStepIndex)
                {
                    questStep.GetComponent<TextMeshProUGUI>().text = "<s> - " + questInfo.questStepNames[i] + "</s>";
                }
                else
                {
                    questStep.GetComponent<TextMeshProUGUI>().text = "- " +  questInfo.questStepNames[i];
                }

            }
        }
    }

    public void LoadData(GameData data)
    {
        QuestsInProgress = new List<QuestInfo>();
        QuestsCompleted = new List<QuestInfo>();

        QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Scriptable Objects/Quest System/Quests");
        Quests = allQuests;
        QuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfo questInfo in allQuests)
        {
            if (data.QuestData.ContainsKey(questInfo.id) && loadQuestState)
            {
                QuestData questData = data.QuestData[questInfo.id];
                Quest newQuest = new Quest(questInfo, questData.questState, questData.questStepIndex, questData.questStepStates);
                Debug.Log("Loading Quest: " + questInfo.id + " : " + newQuest.state);
                QuestMap.Add(questInfo.id, newQuest);
                if (newQuest.state == QuestState.IN_PROGRESS || newQuest.state == QuestState.CAN_FINISH)
                {
                    newQuest.InstantiateCurrentQuestStep(this.transform, false);
                    //ChangeQuestState(newQuest.info.id, newQuest.state);
                    QuestsInProgress.Add(newQuest.info);
                }
                else if (newQuest.state == QuestState.FINISHED)
                {
                    QuestsCompleted.Add(newQuest.info);
                }
                EventsManager.Instance.questEvents.QuestStateChange(newQuest);
            }
            else
            {
                Quest newQuest = new Quest(questInfo);
                QuestMap.Add(questInfo.id, newQuest);
                EventsManager.Instance.questEvents.QuestStateChange(newQuest);
            }
        }
        ChangePlayerLevel(data.level);
    }

    public void SaveData(ref GameData data)
    {
        foreach (Quest quest in QuestMap.Values)
        {
            QuestData questData = quest.GetQuestData();
            //Debug.Log(quest.info.id);
            //Debug.Log("state: " + questData.questState);
            //Debug.Log("index: " + questData.questStepIndex);
            foreach (QuestStepState stepState in questData.questStepStates)
            {
                //Debug.Log("step state = " + stepState.questState);
            }
            if (data.QuestData.ContainsKey(quest.info.id))
            {
                data.QuestData.Remove(quest.info.id);
            }
            data.QuestData.Add(quest.info.id, questData);
        }
    }
}

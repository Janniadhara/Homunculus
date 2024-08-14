using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour, IDataPersistence
{
    public static DialogueManager Instance;
    [Header("Displays")]
    [SerializeField] private GameObject DialogueScreen;
    [SerializeField] private GameObject NpcTextArea;
    [SerializeField] private GameObject AnswerButtonArea;
    [Header("Prefabs")]
    [SerializeField] private TextMeshProUGUI NpcText; //static cause only one npc
    [SerializeField] private float writingSpeed;
    [SerializeField] private Button AnswerButton;

    private Story currentDialogue;
    private string nameOfNpc;
    [SerializeField] private TextAsset globalVariablesJson;
    private DialogueVariables dialogueVariables;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        EventsManager.Instance.pauseGameEvent.onPauseGame += GamePaused;
        DialogueScreen.SetActive(false);
    }
    private void OnDisable()
    {
        EventsManager.Instance.pauseGameEvent.onPauseGame -= GamePaused;
    }

    public void StartDialogue(TextAsset inkJson, string npcName)
    {
        currentDialogue = new Story(inkJson.text);
        EventsManager.Instance.dialogueEvent.StartDialogue(inkJson, npcName);
        DialogueScreen.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        nameOfNpc = npcName;

        dialogueVariables.StartListening(currentDialogue);
        currentDialogue.BindExternalFunction("startQuest", (string questId) =>
        {
            Debug.Log(questId);
            EventsManager.Instance.questEvents.StartQuest(questId);
        });
        currentDialogue.BindExternalFunction("finishStep", (string questId) =>
        {
            Debug.Log(questId);
            EventsManager.Instance.questEvents.FinishStep(questId);
        });
        currentDialogue.BindExternalFunction("finishQuest", (string questId) =>
        {
            Debug.Log(questId);
            EventsManager.Instance.questEvents.FinishQuest(questId);
        });
        currentDialogue.BindExternalFunction("takeItemsFromPlayer", (string questId) =>
        {
            Debug.Log("Take Items" + questId);
            EventsManager.Instance.pickUpItemEvent.TakeItemFromPlayer(questId);
        });
        DisplayDialogue();
    }
    public void FinishDialogue(string npcName)
    {
        EventsManager.Instance.dialogueEvent.FinishDialogue(npcName);
        DialogueScreen.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        dialogueVariables.StopListening(currentDialogue);
        currentDialogue.UnbindExternalFunction("startQuest");
        currentDialogue.UnbindExternalFunction("finishStep");
        currentDialogue.UnbindExternalFunction("finishQuest");
        currentDialogue.UnbindExternalFunction("takeItemsFromPlayer");
    }
    private void DisplayDialogue()
    {
        ClearPrevDialogue();

        if (currentDialogue.canContinue)
        {
            //string npcText = currentDialogue.Continue();
            //npcText = npcText.Trim();
            StopAllCoroutines();
            StartCoroutine(WriteNpcText(currentDialogue.Continue().Trim()));
        }
        //if there are answers, show them
        if (currentDialogue.currentChoices.Count > 0)
        {
            for (int i = 0; i < currentDialogue.currentChoices.Count; i++)
            {
                Choice choice = currentDialogue.currentChoices[i];
                Button button = CreatePlayerAnswer(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate
                {
                    OnClickChoiceButton(choice);
                });
            }
        }
        //else close the dialogue
        else
        {
            FinishDialogue(nameOfNpc);
        }
    }
    private IEnumerator WriteNpcText(string npcText)
    {
        NpcText.text = npcText;
        NpcText.maxVisibleCharacters = 0;
        foreach (char letter in npcText.ToCharArray())
        {
            NpcText.maxVisibleCharacters++;
            yield return new WaitForSeconds(writingSpeed);
        }
    }
    private Button CreatePlayerAnswer(string answer)
    {
        Button choice = Instantiate(AnswerButton) as Button;
        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.text = answer;
        choice.transform.SetParent(AnswerButtonArea.transform, false);
        return choice;
    }
    void OnClickChoiceButton(Choice choice)
    {
        // When we click the choice button, tell the story to choose that choice!
        currentDialogue.ChooseChoiceIndex(choice.index);
        DisplayDialogue();
    }
    private void ClearPrevDialogue()
    {
        if (AnswerButtonArea.transform.childCount > 0)
        {
            for (int i = AnswerButtonArea.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(AnswerButtonArea.transform.GetChild(i).gameObject);
            }
        }
    }
    private void GamePaused(bool isPaused)
    {
        if (DialogueScreen.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    //call this in pther scripts to get a value to change an icon, or color, etc.
    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object value = null;
        dialogueVariables.GlobalVariables.TryGetValue(variableName, out value);
        if (value == null)
        {
            Debug.LogWarning("Ink variable not found: " + variableName);
        }    
        return value;
    }
    //call this if you want to change the value of a variable from another script
    //e.g. when you can Start or Finish a quest,
    //the npc will have the corresponding answer option in their dialogue enabled
    public void SetVariableState(string name, object value)
    {
        dialogueVariables.ChangeValueOfVariable(name, value);
    }

    public void LoadData(GameData data)
    {
        //load global variables from json (default states)
        dialogueVariables = new DialogueVariables(globalVariablesJson, data.GlobalDialogueVariables);
    }

    public void SaveData(ref GameData data)
    {
        data.GlobalDialogueVariables = dialogueVariables.SaveVariables();
    }
}

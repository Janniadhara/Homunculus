using System;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicDialog : MonoBehaviour
{
    //public static event Action<Story> OnCreateStory;

    [SerializeField]
    private TextAsset inkJSONAsset;
    public Story Dialog;

    [SerializeField] private GameObject NpcTextArea;
    [SerializeField] private GameObject AnswerButtonArea;
    //prefabs
    [SerializeField] private TextMeshProUGUI NpcText;
    [SerializeField] private Button AnswerButton;
    private bool isSpeaking;

    private void Update()
    {
        if (isSpeaking)
        {
            Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            Vector3 npcPos = transform.position;
            Vector3 nTop = new Vector3(playerPos.x - npcPos.x, 0.0f, playerPos.z - npcPos.z);
            Quaternion rotation = Quaternion.LookRotation(nTop);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4f);
        }
    }
    public void StartDialog(TextAsset npcDialogue)
    {
        Dialog = new Story(npcDialogue.text);
        //if (OnCreateStory != null)
        //{
        //    OnCreateStory(Dialog);
        //}
        RefreshView();
    }
    void RefreshView()
    {
        // Remove all the UI on screen
        ClearDialogUi();

        // Read all the content until we can't continue any more
        while (Dialog.canContinue)
        {
            // Continue gets the next line of the story
            string text = Dialog.Continue();
            // This removes any white space from the text.
            text = text.Trim();
            // Display the text on screen!
            CreateContentView(text);
        }

        // Display all the choices, if there are any!
        if (Dialog.currentChoices.Count > 0)
        {
            for (int i = 0; i < Dialog.currentChoices.Count; i++)
            {
                Choice choice = Dialog.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                // Tell the button what to do when we press it
                button.onClick.AddListener(delegate
                {
                    OnClickChoiceButton(choice);
                });
            }
        }
        // If we've read all the content and there's no choices, the story is finished!
        else
        {
            var LookScript = GameObject.FindGameObjectWithTag("Player").GetComponent<LookAtTarget>();
            string npcName = transform.name;
            Button choice = CreateChoiceView("Go away.");
            choice.onClick.AddListener((UnityEngine.Events.UnityAction)delegate
            {
                DialogueManager.Instance.FinishDialogue(npcName);
                //this.CloseDialog();
            });
            choice.onClick.AddListener((UnityEngine.Events.UnityAction)delegate
            {
                //LookScript.DisplayDialog();
            });
        }
    }
    // When we click the choice button, tell the story to choose that choice!
    void OnClickChoiceButton(Choice choice)
    {
        Dialog.ChooseChoiceIndex(choice.index);
        RefreshView();
    }

    // Creates a textbox showing the the line of text
    void CreateContentView(string text)
    {
        TextMeshProUGUI storyText = Instantiate(NpcText) as TextMeshProUGUI;
        storyText.text = text;
        storyText.transform.SetParent(NpcTextArea.transform, false);
    }

    // Creates a button showing the choice text
    Button CreateChoiceView(string text)
    {
        // Creates the button from a prefab
        Button choice = Instantiate(AnswerButton) as Button;
        choice.transform.SetParent(AnswerButtonArea.transform, false);

        // Gets the text from the button prefab
        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.text = text;

        // Make the button expand to fit the text
        //HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
        //layoutGroup.childForceExpandHeight = false;

        return choice;
    }
    private void ClearDialogUi()
    {
        if (NpcTextArea.transform.childCount > 0)
        {
            for (int i = NpcTextArea.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(NpcTextArea.transform.GetChild(i).gameObject);
            }
        }
        if (AnswerButtonArea.transform.childCount > 0)
        {
            for (int i = AnswerButtonArea.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(AnswerButtonArea.transform.GetChild(i).gameObject);
            }
        }
    }
    
}

using System;
using UnityEngine;

public class DialogueEvent
{
    public event Action<TextAsset, string> onStartDialogue;
    public void StartDialogue(TextAsset dialogueJson, string npcName)
    {
        if (onStartDialogue != null)
        {
            onStartDialogue(dialogueJson, npcName);
        }
    }
    public event Action<string> onFinishDialogue;
    public void FinishDialogue(string npcName)
    {
        if (onFinishDialogue != null)
        {
            onFinishDialogue(npcName);
        }
    }
}

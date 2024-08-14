using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JournalEntry
{
    public string questId;
    public string questName;
    public string questDescription;
    public string questStepStateText;
    public int questStepIndex;
    public int questStepState;
    public int requiredStepState;
}

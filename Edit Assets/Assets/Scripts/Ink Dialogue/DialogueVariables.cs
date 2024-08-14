using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> GlobalVariables;// { get; private set; }
    private Story globalDialogueVariables;
    public DialogueVariables(TextAsset globalVariablesJson, string savedVariables)
    {
        //compile global variables as a story
        globalDialogueVariables = new Story(globalVariablesJson.text);
        if (savedVariables != "")
        {
            globalDialogueVariables.state.LoadJson(savedVariables);
        }

        //initialize the dictionary
        GlobalVariables = new Dictionary<string, Ink.Runtime.Object>();
        foreach (string name in globalDialogueVariables.variablesState)
        {
            Ink.Runtime.Object value = globalDialogueVariables.variablesState.GetVariableWithName(name);
            GlobalVariables.Add(name, value);
            Debug.Log("Initialize global variable: " + name + " = " + value);
        }
    }
    public void StartListening(Story dialogue)
    {
        VariablesToDialogue(dialogue);
        dialogue.variablesState.variableChangedEvent += VariableChanged;
    }
    public void StopListening(Story dialogue)
    {
        dialogue.variablesState.variableChangedEvent -= VariableChanged;
    }
    public void ChangeValueOfVariable(string name, object value)
    {
        StartListening(globalDialogueVariables);
        if (globalDialogueVariables != null)
        {
            globalDialogueVariables.variablesState[name] = value;
            Debug.Log(name + " = " + value);
        }
        StopListening(globalDialogueVariables);
    }
    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        if (GlobalVariables.ContainsKey(name))
        {
            GlobalVariables.Remove(name);
            GlobalVariables.Add(name, value);
        }
    }
    private void VariablesToDialogue(Story dialogue)
    {
        foreach (KeyValuePair<string, Ink.Runtime.Object> variable in GlobalVariables)
        {
            dialogue.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }
    public string SaveVariables()
    {
        if (globalDialogueVariables != null)
        {
            //load vurrent variables values into the global story
            VariablesToDialogue(globalDialogueVariables);
            Debug.Log(globalDialogueVariables.variablesState.GetVariableWithName("test"));
            return globalDialogueVariables.state.ToJson();
        }
        return "";
    }
    public Story GetCurrentValues(Story dialogue)
    {
        VariablesToDialogue(dialogue);
        return dialogue;
    }
}

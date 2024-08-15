using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetDebugLog : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugLogTextField;
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            debugLogTextField.text = logString;
        }
    }
}

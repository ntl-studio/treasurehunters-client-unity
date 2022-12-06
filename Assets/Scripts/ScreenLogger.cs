using System.Collections; using TMPro;
using UnityEngine;

public class ScreenLogger : MonoBehaviour
{
    public TextMeshProUGUI LogText;

    void Start()
    {
        Debug.Assert(LogText);
    }

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    string myLog;
    Queue myLogQueue = new Queue();

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }

        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }

        LogText.text = myLog;
    }
}

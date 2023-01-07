using System.Text;
using TMPro;
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

    private readonly StringBuilder _myLogBuilder = new StringBuilder();

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        _myLogBuilder.Append($"[{type}] : {logString}\n");

        if (type == LogType.Exception)
            _myLogBuilder.Append($"{stackTrace}\n");

        LogText.text = _myLogBuilder.ToString();
    }
}

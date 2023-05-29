using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsEventManager : MonoBehaviour
{
    private static readonly Dictionary<string, object> _eventParameters = new Dictionary<string, object>();

    public static void OnEvent(string Event)
    {
        AppMetrica.Instance.ReportEvent(Event);
        Debug.Log(Event);
    }

    public static void OnEvent(string Event, string parameterName, string value)
    {
        Debug.Log($"{Event}, {parameterName}, {value}");

        _eventParameters[parameterName] = value;
        AppMetrica.Instance.ReportEvent(Event, _eventParameters);
        _eventParameters.Clear();
    }
}

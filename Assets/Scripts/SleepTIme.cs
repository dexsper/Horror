using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepTIme : MonoBehaviour
{
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("ad");
    }
}

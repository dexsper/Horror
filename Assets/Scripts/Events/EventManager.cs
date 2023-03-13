using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static Action OnLeaveRoom;

    public static void OnLeaveRoomInvoke()
    {
        OnLeaveRoom?.Invoke();
    }
}

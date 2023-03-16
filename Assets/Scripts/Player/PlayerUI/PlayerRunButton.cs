using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerRunButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        PlayerComponents.Instance.PlayerMovement.Run();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PlayerComponents.Instance.PlayerMovement.CancelRun();
    }
}

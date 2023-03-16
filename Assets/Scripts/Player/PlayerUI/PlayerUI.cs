using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [field: SerializeField] public Button UseButton { get; private set; }
    [SerializeField] private Button takeButton;
    [SerializeField] private Button runButton;
    

    public void ShowTakeButton()
    {
        takeButton.gameObject.SetActive(true);
    }

    public void HideTakeButton()
    {
        takeButton.gameObject.SetActive(false);
    }


}

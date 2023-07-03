using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialAnimator : MonoBehaviour
{
    [SerializeField] private Button playButton, shopButton, lobbyButton;

    public void OnStart()
    {
        playButton.interactable = false;
        shopButton.interactable = false;
        lobbyButton.interactable = false;
    }

    public void OnFinish()
    {
        playButton.interactable = true;
        shopButton.interactable = true;
        lobbyButton.interactable = true;
    }
    
    public void CancelTutorial()
    {
        Destroy(gameObject);
    }
}

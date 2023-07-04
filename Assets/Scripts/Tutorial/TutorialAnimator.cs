using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialAnimator : MonoBehaviour
{
    [SerializeField] private GameObject firstTutorial,secondTutorial,thirdTutorial;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            PlayerPrefs.SetInt("Tutorial",1);

            secondTutorial.SetActive(false);
            thirdTutorial.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnMenuOpened()
    {
        firstTutorial.SetActive(false);
        secondTutorial.SetActive(true);
    }

    private void OnCharacterSelected()
    {
        secondTutorial.SetActive(false);
        thirdTutorial.SetActive(true);
    }

    private void OnShopClosed()
    {
        thirdTutorial.SetActive(false);
    }
    
    public void CancelTutorial()
    {
        Destroy(gameObject);
    }
}

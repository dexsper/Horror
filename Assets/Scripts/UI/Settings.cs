using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Min(0.01f)]
    [SerializeField] private Slider cameraSensitivity;

    [SerializeField] private Text sensitivitySliderValue;

    private void Awake()
    {
        /*openButton.onClick.AddListener(OpenMenu);
        closeButton.onClick.AddListener(CloseMenu);
        exitButton.onClick.AddListener(Exit);*/
    }

    private void Start()
    {
        cameraSensitivity.value = PlayerPrefs.HasKey("Sensitivity") ? PlayerPrefs.GetFloat("Sensitivity") : 0.14f;
        sensitivitySliderValue.text = $"{Math.Round(cameraSensitivity.value,2) * 100}%";
    }

    public static event Action<float> OnSensetivityChange;
    
    public void SetSensitivity()
    {
        PlayerPrefs.SetFloat("Sensitivity",cameraSensitivity.value * 100);
        sensitivitySliderValue.text = $"{Math.Round(cameraSensitivity.value,2) * 100}%";
        if (cameraSensitivity.value <= 0)
        {
            cameraSensitivity.value = 0.01f;
            sensitivitySliderValue.text = $"{Math.Round(cameraSensitivity.value,2) * 100}%";
            PlayerPrefs.SetFloat("Sensitivity",cameraSensitivity.value);
        }
        OnSensetivityChange?.Invoke(cameraSensitivity.value * 100);
    }

    [ContextMenu("Clear Prefs")]
    private void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

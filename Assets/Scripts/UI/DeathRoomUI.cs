using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathRoomUI : MonoBehaviour
{
    [SerializeField] private Text _progressText;

    public void UpdateProgress(float v)
    {
        int value = (int)v;
        
        _progressText.text = $"{value}...";
    }
}

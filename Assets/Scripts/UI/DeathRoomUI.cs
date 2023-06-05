using UnityEngine;
using UnityEngine.UI;

public class DeathRoomUI : MonoBehaviour
{
    [SerializeField] private Button _progressButton;
    [SerializeField] private DeathRoom _deathRoom;
    [SerializeField] private Slider _progressSlider;


    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject tutorialInfo;

    private void Awake()
    {
        _progressButton.onClick.AddListener(OnProgressClick);
        confirmButton.onClick.AddListener(HideTutorial);
    }

    public void OnProgressClick()
    {
        _deathRoom.AddLeaveProgress(PlayerBehavior.LocalPlayer);
    }

    public void UpdateProgress(float v)
    {
        _progressSlider.value = v;
    }

    public void ShowTutorial()
    {
        tutorialInfo.gameObject.SetActive(true);
    }

    private void HideTutorial()
    {
        tutorialInfo.gameObject.SetActive(false);
        Ads.Instance.ShowAd();
    }
}

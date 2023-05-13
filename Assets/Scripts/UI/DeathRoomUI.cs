using UnityEngine;
using UnityEngine.UI;

public class DeathRoomUI : MonoBehaviour
{
    [SerializeField] private Button _progressButton;
    [SerializeField] private DeathRoom _deathRoom;
    [SerializeField] private Slider _progressSlider;

    private void Awake()
    {
        _progressButton.onClick.AddListener(OnProgressClick);
    }

    public void OnProgressClick()
    {
        _deathRoom.AddLeaveProgress(PlayerBehavior.LocalPlayer);
    }

    public void UpdateProgress(float v)
    {
        _progressSlider.value = v;
    }
}

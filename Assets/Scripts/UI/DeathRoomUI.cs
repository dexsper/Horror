using UnityEngine;
using UnityEngine.UI;

public class DeathRoomUI : MonoBehaviour
{
    [SerializeField] private Button _progressButton;
    [SerializeField] private DeathRoom _deathRoom;

    private void Awake()
    {
        _progressButton.onClick.AddListener(OnProgressClick);
    }

    public void OnProgressClick()
    {
        _deathRoom.AddLeaveProgress(PlayerBehavior.LocalPlayer);
    }
}

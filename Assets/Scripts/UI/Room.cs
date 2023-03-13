using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [SerializeField] private float animationDuration;
    [SerializeField] private TextMeshProUGUI roomNameText;

    [SerializeField] private Button leaveRoomButton;

    private void Start()
    {
        leaveRoomButton.onClick.AddListener(CloseRoom);
    }

    public void OpenRoom(string text)
    {
        roomNameText.text = text;
        transform.DOScale(1, animationDuration).From(0).SetEase(Ease.Linear);
    }

    public void CloseRoom()
    {
        EventManager.OnLeaveRoomInvoke();
        transform.DOScale(0, animationDuration).From(1).SetEase(Ease.Linear);
    }
    
    
}

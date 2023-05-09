using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    [SerializeField] private Image _image;

    private PlayerBehavior _behavior;

    public void SetPlayer(PlayerBehavior player)
    {
        _behavior = player;

        _behavior.Health.OnDead += OnPlayerDead;
        _behavior.Health.OnRestored += OnPlayerHealthRestored;
    }

    private void OnDestroy()
    {
        _behavior.Health.OnDead -= OnPlayerDead;
        _behavior.Health.OnRestored -= OnPlayerHealthRestored;
    }

    private void OnPlayerDead()
    {
        _image.color = Color.red;
    }

    private void OnPlayerHealthRestored()
    {
        _image.color = Color.white;
    }
}
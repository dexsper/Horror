using System.Collections.Generic;
using UnityEngine;

public class PlayersStateUI : MonoBehaviour
{
    [SerializeField] private PlayerStateUI _playerUIPrefab;

    private Dictionary<PlayerBehavior, PlayerStateUI> _playersState = new();

    private void Awake()
    {
        PlayerBehavior.OnPlayerSpawned += OnPlayerSpawned;
        PlayerBehavior.OnPlayerDestroy += OnPlayerDestroy;
    }

    private void OnPlayerDestroy(PlayerBehavior player)
    {
        if(_playersState.ContainsKey(player))
        {
            Destroy(_playersState[player].gameObject);

            _playersState.Remove(player);
        }
    }

    private void OnPlayerSpawned(PlayerBehavior player)
    {
        if(!_playersState.ContainsKey(player))
        {
            var playerStateUI = Instantiate(_playerUIPrefab, transform);
            playerStateUI.SetPlayer(player);

            _playersState.Add(player, playerStateUI);
        }
    }
}

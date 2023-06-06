using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayersStateUI : MonoBehaviour
{
    [SerializeField] private PlayerStateUI _playerUIPrefab;
    [SerializeField] private Vector3 startPosition;

    private Dictionary<PlayerBehavior, PlayerStateUI> _playersState = new Dictionary<PlayerBehavior, PlayerStateUI>();

    private void Awake()
    {
        PlayerBehavior.OnRespawned += OnPlayerSpawned;
        PlayerBehavior.OnDead += OnPlayerDestroy;
    }

    private void OnDestroy()
    {
        PlayerBehavior.OnRespawned -= OnPlayerSpawned;
        PlayerBehavior.OnDead -= OnPlayerDestroy;
    }

    private void Start()
    {
        SetPosition();
    }

    [ContextMenu("Test")]
    private void SetPosition()
    {
        transform.localPosition = startPosition;
    }
    
    private void OnPlayerDestroy(PlayerBehavior player)
    {
        if (player == null)
            return;
            
        if (_playersState.ContainsKey(player))
        {
            Destroy(_playersState[player].gameObject);

            _playersState.Remove(player);
        }
    }

    private void OnPlayerSpawned(PlayerBehavior player)
    {
        if (!_playersState.ContainsKey(player))
        {
            var playerStateUI = Instantiate(_playerUIPrefab, transform);
            playerStateUI.SetPlayer(player);

            _playersState.Add(player, playerStateUI);
        }
    }
}

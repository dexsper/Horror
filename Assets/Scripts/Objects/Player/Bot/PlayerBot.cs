using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerBot : Bot
{
    [SerializeField] private CharactersData _characterData;
    [SerializeField] private List<string> _names;
    [SerializeField] private TextMeshProUGUI _nameText;

    private PlayerBehavior _behavior;
    private Generator _generator;

    protected override void Awake()
    {
        base.Awake();

        _behavior = GetComponent<PlayerBehavior>();

        if (_nameText != null)
        {
            string name = _names[Random.Range(0, _names.Count)];
            _nameText.text = name;

            ConnectionIdentity.Players[this.Owner.ClientId] = new Player($"Bot")
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {LobbyManager.KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, name)},
                }
            };
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        _behavior.UpdateModel(_characterData.Characters.ElementAt(Random.Range(0, _characterData.Characters.Count)).Value);
    }

    protected override void Update()
    {
        base.Update();

        if (!IsMove && _generator != null && !_generator.IsRepairing)
        {
            transform.LookAt(_generator.transform);

            if (_behavior.Interaction.CanInteract)
            {
                _behavior.Interaction.LookInteractable.Interact(_behavior);
            }
        }

        if (_behavior.Animator != null)
            _behavior.Animator.SetBool(nameof(IsMove), IsMove);
    }

    protected override Vector3 GetTargetPosition()
    {
        if (_generator != null)
        {
            if (_generator.IsRepaired || (_generator.IsRepairing && _generator.RepairInitiator != _behavior))
            {
                _generator = null;
            }
        }

        if (_generator == null)
        {
            var generator = _fieldOfView.GetObject<Generator>();

            if (generator != null && !generator.IsRepaired)
            {
                _generator = generator;
            }
        }

        if (_generator != null)
        {
            return _generator.transform.position;
        }

        return base.GetTargetPosition();
    }
}

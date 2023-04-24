using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _interactionText;
    [SerializeField] private Button _interactButton;

    [SerializeField] private TextMeshProUGUI repairedGeneratorsCount;
    
    private void Awake()
    {
        _interactButton.onClick.AddListener(Interact);
    }

    private void Start()
    {
        UpdateRepairedGeneratorsText(Generator.Generators.Count);
    }

    private void OnEnable()
    {
        Generator.OnRepaired += OnGeneratorRepaired;

    }

    private void OnDestroy()
    {
        Generator.OnRepaired -= OnGeneratorRepaired;
    }

    private void Interact()
    {
        if (PlayerBehavior.LocalPlayer != null)
        {
            PlayerBehavior.LocalPlayer.Interaction.Interact();
        }
    }

    private void Update()
    {
        PlayerBehavior localPlayer = PlayerBehavior.LocalPlayer;
        if (localPlayer == null) return;
        
        UpdateInteraction(localPlayer);
    }

    private void UpdateInteraction(PlayerBehavior localPlayer)
    {
        bool canInteract = localPlayer.Interaction.CanInteract;
        _interactButton.gameObject.SetActive(canInteract);

        if (localPlayer.Interaction.CanInteract && localPlayer.Interaction.LookInteractable != null)
        {
            _interactionText.text = localPlayer.Interaction.LookInteractable.InteractionPrompt;
            return;
        }

        _interactionText.text = "";
    }

    private void UpdateRepairedGeneratorsText(int count)
    {
        repairedGeneratorsCount.text = $"Generators Left {count}";
    }
    
    private void OnGeneratorRepaired(Generator generator)
    {
        UpdateRepairedGeneratorsText(Generator.Generators.Where((generator1 => !generator1.IsRepaired)).Count());
    }
}

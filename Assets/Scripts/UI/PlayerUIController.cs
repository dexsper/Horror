using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _interactionText;
    [SerializeField] private Button _interactButton;
    [SerializeField] private Button _leaveButton;

    [SerializeField] private TextMeshProUGUI repairedGeneratorsCount;

    [SerializeField] private Slider interactionSlider;
    
    private void Awake()
    {
        _interactButton.onClick.AddListener(Interact);
        _leaveButton.onClick.AddListener(Leave);
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
        interactionSlider.gameObject.SetActive(canInteract);

        if (localPlayer.Interaction.CanInteract && localPlayer.Interaction.LookInteractable != null)
        {
            _interactionText.text = localPlayer.Interaction.LookInteractable.InteractionPrompt;
            UpdateSlider(localPlayer.Interaction.LookInteractable.GetRepairProgress());
            return;
        }

        _interactionText.text = "";
    }
    private void Interact()
    {
        if (PlayerBehavior.LocalPlayer != null)
        {
            PlayerBehavior.LocalPlayer.Interaction.Interact();
        }
    }

    private void Leave()
    {
        LobbyManager.Instance.LeaveLobby();
    }

    private void UpdateRepairedGeneratorsText(int count)
    {
        repairedGeneratorsCount.text = $"Generators Left {count}";
    }  
    private void OnGeneratorRepaired(Generator generator)
    {
        UpdateRepairedGeneratorsText(Generator.Generators.Where((generator1 => !generator1.IsRepaired)).Count());
        if (Generator.Generators.Where((generator1 => !generator1.IsRepaired)).Count() == 0)
        {
            repairedGeneratorsCount.text = "All Generators Repaired, Run Away!";
        }
    }

    private void UpdateSlider(float value)
    {
        interactionSlider.value = value;
    }
}

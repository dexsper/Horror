using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using FishNet;
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

    [SerializeField] private Button openSettingsButton, closeSettingsButton;
    [SerializeField] private Image settingsPanel;

    private void Awake()
    {
        _interactButton.onClick.AddListener(Interact);
        _leaveButton.onClick.AddListener(Leave);

        openSettingsButton.onClick.AddListener(OnSettingsButtonClick);
        closeSettingsButton.onClick.AddListener(InSettingsButtonClick);
    }

    private void Start()
    {
        StartCoroutine(PlayerTextAtStart());
    }

    private IEnumerator PlayerTextAtStart()
    {
        repairedGeneratorsCount.text = "Find Generators";
        yield return new WaitForSeconds(5f);
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

    private void OnSettingsButtonClick()
    {
        settingsPanel.transform.DOScale(1f, 0.25f).From(0f).SetEase(Ease.Linear);
    }

    private void InSettingsButtonClick()
    {
        settingsPanel.transform.DOScale(0f, 0.25f).From(1f).SetEase(Ease.Linear);
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
        if (LobbyManager.Instance.JoinedLobby != null)
        {
            LobbyManager.Instance.LeaveLobby();
        }
        else
        {
            InstanceFinder.ServerManager.StopConnection(true);
        }
    }

    private void UpdateRepairedGeneratorsText(int count)
    {
        repairedGeneratorsCount.text = $"Generators left {count}";
    }
    private void OnGeneratorRepaired(Generator generator)
    {
        UpdateRepairedGeneratorsText(Generator.Generators.Where((generator1 => !generator1.IsRepaired)).Count());

        if (Generator.Generators.Where((generator1 => !generator1.IsRepaired)).Count() == 0)
        {
            repairedGeneratorsCount.text = "Run!";
        }
    }

    private void UpdateSlider(float value)
    {
        interactionSlider.value = value;
    }
}

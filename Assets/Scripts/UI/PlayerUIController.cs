using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FishNet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private Button _interactButton;
    [SerializeField] private Button _leaveButton;

    [SerializeField] private Text repairedGeneratorsCount;

    [SerializeField] private Slider interactionSlider;

    [SerializeField] private Button openSettingsButton, closeSettingsButton;
    [SerializeField] private Image settingsPanel;

    [SerializeField] private Image endGamePanel;

    [SerializeField] private string findGeneratorsRu, findGeneratorsEn;
    [SerializeField] private string generatorsLeftRu, generatorsLeftEn;
    [SerializeField] private string endRu, endEn;

    [SerializeField] private List<Vector3> endPositions;
    [SerializeField] private List<Button> emojiButtons;

    [SerializeField] private Button _openButton;

    private AudioSource _source;
    
    private bool _isOpen;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        
        _openButton.onClick.AddListener(OnButtonClick);

        _interactButton.onClick.AddListener(Interact);
        _leaveButton.onClick.AddListener(Leave);

        openSettingsButton.onClick.AddListener(OnSettingsButtonClick);
        closeSettingsButton.onClick.AddListener(InSettingsButtonClick);
    }

    private void OnButtonClick()
    {
        if (!_isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void Open()
    {
        _isOpen = true;
        for (int i = 0; i < emojiButtons.Count; i++)
        {
            emojiButtons[i].transform.DOLocalMove(endPositions[i], 0.2f).SetEase(Ease.Linear);
            emojiButtons[i].transform.DOScale(1f, 0.2f).SetEase(Ease.Linear);
        }
    }

    private void Close()
    {
        _isOpen = false;
        for (int i = 0; i < emojiButtons.Count; i++)
        {
            emojiButtons[i].transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.Linear);
            emojiButtons[i].transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear);
        }
    }
    
    public void CreateEmoji(int index)
    {
        PlayerBehavior.LocalPlayer.PlayerUI.CreateEmoji(index);
        Close();
    }

    private void Start()
    {
        StartCoroutine(PlayerTextAtStart());
    }

    private void OnGameEnded()
    {
        endGamePanel.gameObject.SetActive(true);
    }

    private IEnumerator PlayerTextAtStart()
    {
        repairedGeneratorsCount.text =
            LocalizationUI.Instance.GetLocaleName() == "ru" ? findGeneratorsRu : findGeneratorsEn;
        yield return new WaitForSeconds(5f);
        UpdateRepairedGeneratorsText(Generator.Generators.Where((generator1 => generator1.IsRepaired)).Count());
    }
    private void OnEnable()
    {
        Generator.OnRepaired += OnGeneratorRepaired;
        GameController.OnGameEnded += OnGameEnded;

    }
    private void OnDestroy()
    {
        Generator.OnRepaired -= OnGeneratorRepaired;
        GameController.OnGameEnded -= OnGameEnded;
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
        bool isInteract = localPlayer.Interaction.IsInteract;

        _interactButton.gameObject.SetActive(canInteract && !isInteract);
        interactionSlider.gameObject.SetActive(canInteract);

        if (localPlayer.Interaction.CanInteract && localPlayer.Interaction.LookInteractable != null)
        {
            UpdateSlider(localPlayer.Interaction.LookInteractable.GetRepairProgress());
            return;
        }

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
            Ads.Instance.ShowAd();
        }
        else
        {
            Ads.Instance.ShowAd();
            InstanceFinder.ServerManager.StopConnection(true);
        }
    }

    private void UpdateRepairedGeneratorsText(int count)
    {
        repairedGeneratorsCount.text = LocalizationUI.Instance.GetLocaleName() == "ru"
            ? $"{generatorsLeftRu} {count}/{Generator.Generators.Count}"
            : $"{generatorsLeftEn} {count}/{Generator.Generators.Count}";
    }
    private void OnGeneratorRepaired(Generator generator)
    {
        int count = Generator.Generators.Where((generator1 => generator1.IsRepaired)).Count();

        UpdateRepairedGeneratorsText(count);

        AnalyticsEventManager.OnEvent("Repaired generator", "Repaired", count.ToString());
        
        for (int i = 0; i < Generator.Generators.Count; i++)
        {
            if (!Generator.Generators[i].IsRepaired)
                return;
        }
        repairedGeneratorsCount.text = LocalizationUI.Instance.GetLocaleName() == "ru" ? endRu : endEn;
        _source.Play();
    }

    private void UpdateSlider(float value)
    {
        interactionSlider.value = value;
    }
}

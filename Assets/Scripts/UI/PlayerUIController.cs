using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _interactionText;
    [SerializeField] private Button _interactButton;

    [SerializeField] private TextMeshProUGUI repairedObjectsCount;


    [Inject] private ObjectsController _objectsController;
    private void Awake()
    {
        _interactButton.onClick.AddListener(Interact);
        repairedObjectsCount.text = $"Generators left : {_objectsController.ObjectsCount}";
    }

    private void OnEnable()
    {
        _objectsController.OnObjectRepaired += UpdateRepairedObjects;
    }

    private void OnDestroy()
    {
        _objectsController.OnObjectRepaired -= UpdateRepairedObjects;
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

    private void UpdateRepairedObjects(int count)
    {
        repairedObjectsCount.text = $"Generators left : {count}";
    }
}

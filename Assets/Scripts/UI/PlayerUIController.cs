using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _interactionText;
    [SerializeField] private Button _interactButton;

    private void Awake()
    {
        _interactButton.onClick.AddListener(Interact);
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
}

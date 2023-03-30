using UnityEngine;

public interface IInteractable
{
    Transform GetTransform();


    string InteractionPrompt { get; }

    bool CanInteract(Player player);
    void Interact(Player player);
}
using UnityEngine;

public interface IInteractable
{
    Transform GetTransform();


    string InteractionPrompt { get; }

    bool CanInteract(PlayerBehavior player);
    void Interact(PlayerBehavior player);
}
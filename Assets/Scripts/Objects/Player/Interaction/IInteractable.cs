using UnityEngine;

public interface IInteractable
{
    Transform GetTransform();

    string InteractionPrompt { get; }

    bool IsInteract(PlayerBehavior player);
    bool CanInteract(PlayerBehavior player);
    
    void Interact(PlayerBehavior  player);
    void Interact_RPC(PlayerBehavior player);

    float GetRepairProgress();
}
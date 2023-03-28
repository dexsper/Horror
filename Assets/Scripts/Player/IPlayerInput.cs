using UnityEngine;

public interface IPlayerInput
{
    Vector2 Movement { get; }
    Vector2 LookDirection { get; }

    bool IsSprint { get; }
}

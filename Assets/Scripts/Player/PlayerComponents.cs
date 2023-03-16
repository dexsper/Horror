using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerComponents : NetworkBehaviour
{
    public static PlayerComponents Instance;
    public PlayerUI PlayerUI { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerAnimator PlayerAnimator { get; private set; }

    [field: SerializeField] public PlayerLock PlayerLook { get; private set; }

    private void Awake()
    {
        Instance = this;
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerUI = GetComponent<PlayerUI>();
        PlayerAnimator = GetComponent<PlayerAnimator>();
    }
}

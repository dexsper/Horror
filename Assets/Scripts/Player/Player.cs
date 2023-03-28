using FishNet.Object;
using UnityEngine;
using Zenject;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private float cameraYOffset = 0.4f;

    public Transform PlayerCameraTransform { get; private set; }
    public Camera PlayerCamera { get; private set; }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(base.IsOwner)
        {
            FindObjectOfType<SceneContext>().Container.InjectGameObject(gameObject);
            PlayerCamera = Camera.main;
            PlayerCameraTransform = PlayerCamera.GetComponent<Transform>();
            PlayerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            PlayerCamera.transform.SetParent(transform);
        }
    }
}

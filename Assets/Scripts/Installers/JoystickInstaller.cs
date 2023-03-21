using UnityEngine;
using Zenject;

public class JoystickInstaller : MonoInstaller
{
    [SerializeField] private Joystick joystick;
    public override void InstallBindings()
    {
        Container.Bind<Joystick>().FromInstance(joystick).AsSingle();
    }
}
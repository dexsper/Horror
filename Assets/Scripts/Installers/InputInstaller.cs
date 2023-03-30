using UnityEngine;
using Zenject;

public class InputInstaller : MonoInstaller
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private TouchField touchField;


    public override void InstallBindings()
    {
        Container.Bind<TouchField>().FromInstance(touchField).AsSingle();
        Container.Bind<Joystick>().FromInstance(joystick).AsSingle();
    }
}
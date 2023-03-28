using UnityEngine;
using Zenject;

public class TouchFieldInstaller : MonoInstaller
{
   [SerializeField] private PlayerTouchField touchField;

   public override void InstallBindings()
   {
      Container.Bind<PlayerTouchField>().FromInstance(touchField).AsSingle();
   }
}

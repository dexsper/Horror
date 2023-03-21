using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class DiContainerInstaller : MonoInstaller
{
    [Inject] private DiContainer diContainer;

    public override void InstallBindings()
    {
        DiContainerRef.Container = diContainer;
    }
}

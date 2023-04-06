using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LobbyManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<LobbyManager>().FromNewComponentOnNewGameObject().WithGameObjectName("LobbyManager").AsSingle().NonLazy();
    }
}

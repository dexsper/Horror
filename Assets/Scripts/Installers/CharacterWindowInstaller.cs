using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CharacterWindowInstaller : MonoInstaller
{
    [SerializeField] private CharacterWindowUI characterWindowUI;

    public override void InstallBindings()
    {
        Container.Bind<CharacterWindowUI>().FromInstance(characterWindowUI).AsSingle();
    }
}

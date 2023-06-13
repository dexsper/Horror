using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RewardedInstaller : MonoInstaller
{
    [SerializeField] private RewardedMenuImage rewardedMenuImage;
    public override void InstallBindings()
    {
        Container.Bind<RewardedMenuImage>().FromInstance(rewardedMenuImage).AsSingle();
    }
}

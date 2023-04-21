using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectControllerInstaller : MonoInstaller
{
    [SerializeField] private ObjectsController objectsController;

    public override void InstallBindings()
    {
        Container.Bind<ObjectsController>().FromInstance(objectsController).AsSingle();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public static class DiContainerRef
{
    public static DiContainer Container
    {
        get => container;

        set => container ??= value;
    }

    private static DiContainer container;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{ 
    [SerializeField] private List<Generator> interactables = new List<Generator>();

    public int ObjectsCount => interactables.Count;
    public Action<int> OnObjectRepaired;
    
    public void OnGeneratorRepaired(Generator repairedObject)
    {
        interactables.Remove(repairedObject);
        OnObjectRepaired?.Invoke(ObjectsCount);
    }
    
}

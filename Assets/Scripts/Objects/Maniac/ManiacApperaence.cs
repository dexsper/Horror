using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManiacApperaence : MonoBehaviour
{
   [SerializeField] private List<GameObject> models = new List<GameObject>();

   public Animator Animator { get; private set; }

   public GameObject GetManiacApperaence()
   {
      int index = Random.Range(0, models.Count);

      Animator = models[index].GetComponent<Animator>();
      return models[index];
   }
}

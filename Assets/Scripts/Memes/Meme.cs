using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Meme : MonoBehaviour
{
    [SerializeField] private List<Texture2D> memeTexture;

    private static List<Texture2D> MemeTexture = new List<Texture2D>();
    
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        for (int i = 0; i < memeTexture.Count; i++)
        {
            MemeTexture.Add(memeTexture[i]);
        }
        
        var index = Random.Range(0, MemeTexture.Count);
        _meshRenderer.materials[0].mainTexture = MemeTexture[index];
        MemeTexture.Remove(MemeTexture[index]);
    }
}

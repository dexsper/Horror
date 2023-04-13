using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName = "SO",menuName = "CreateNewCharacter")]
public class CharactersData : SerializedScriptableObject
{
    [DictionaryDrawerSettings(KeyLabel = "Character Name", ValueLabel = "Character Prefab")]
    public Dictionary<string, GameObject> Characters = default;

    public GameObject this[string characterName]
    {
        get => Characters[characterName];
    }
}

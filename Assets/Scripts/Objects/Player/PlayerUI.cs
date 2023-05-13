using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNickName;

    public void SetPlayerNickNameOnUI(string name)
    {
        playerNickName.text = $"{name}";
    }
}

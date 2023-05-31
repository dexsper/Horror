using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBoardConfirm : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _inputText;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Confirm);
    }

    private void Confirm()
    {
        if (_inputText.text.Length >= 3)
        {
            EditPlayerName.Instance.ConfirmName(_inputText.text);
        }
    }
}

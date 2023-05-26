using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _actionText;

    [SerializeField] private Button _actionButton;
    [SerializeField] private Transform _modelHolder;
    
    [SerializeField] private List<string> russianButtonText = new List<string>();
    [SerializeField] private List<string> englishButtonText = new List<string>();
    
    private int _price;
    private string _name;

    private void Awake()
    {
        _actionButton.onClick.AddListener(Action);
    }

    private void Action()
    {
        if(!string.IsNullOrEmpty(_name))
        {
            CharacterWindowUI.Instance.CharacterAction(_name);
        }
    }

    public void Initialize(string name, int price, GameObject model)
    {
        _name = name;
        _price = price;

        Instantiate(model, _modelHolder);
        UpdateUI();
    }

    public void UpdateUI()
    {
        _nameText.text = _name;

        if (CharacterWindowUI.Instance.CharacterIsPurchased(_name))
        {
            if (CharacterWindowUI.Instance.SelectedCharacterName == _name)
            {
                _actionButton.interactable = true;
                _actionText.text = LocalizationUI.Instance.GetLocaleName() == "ru"
                    ? russianButtonText[0]
                    : englishButtonText[0];
            }
            else
            {
                _actionButton.interactable = true;
                _actionText.text = LocalizationUI.Instance.GetLocaleName() == "ru"
                    ? russianButtonText[1]
                    : englishButtonText[1];
            }
        }
        else
        {
            _actionButton.interactable = true;
            _actionText.text = LocalizationUI.Instance.GetLocaleName() == "ru"
                ? $"{russianButtonText[2]} {_price}"
                : $"{englishButtonText[2]} {_price}";
        }
    }
}
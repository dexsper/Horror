using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CharacterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Text _actionText;

    [SerializeField] private Button _actionButton;
    [SerializeField] private Image _moneyImage;
    [SerializeField] private Transform _modelHolder;
    [SerializeField] private Vector3 _rotationVector;
    
    [SerializeField] private List<string> russianButtonText = new List<string>();
    [SerializeField] private List<string> englishButtonText = new List<string>();
    
    public Button ActionButton { get; private set; }
    
    private int _price;
    private string _name;

    private void Awake()
    {
        _actionButton.onClick.AddListener(Action);

        PlayerEconomy.OnDataRefreshed += UpdateUI;
    }

    private void OnDestroy()
    {
        PlayerEconomy.OnDataRefreshed -= UpdateUI;
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

    private void FixedUpdate()
    {
        _modelHolder.Rotate(_rotationVector);
    }

    public void UpdateUI()
    {
        _nameText.text = _name;

        if (CharacterWindowUI.Instance.CharacterIsPurchased(_name))
        {
            if (CharacterWindowUI.Instance.SelectedCharacterName == _name)
            {
                _actionButton.interactable = true;
                _moneyImage.gameObject.SetActive(false);
                _actionText.text = LocalizationUI.Instance.GetLocaleName() == "ru"
                    ? russianButtonText[0]
                    : englishButtonText[0];
                _actionButton.GetComponent<Image>().color = Color.red;
            }
            else
            {
                _actionButton.interactable = true;
                _moneyImage.gameObject.SetActive(false);
                _actionText.text = LocalizationUI.Instance.GetLocaleName() == "ru"
                    ? russianButtonText[1]
                    : englishButtonText[1];
                _actionButton.GetComponent<Image>().color = Color.red;
            }
        }
        else
        {
            _actionButton.interactable = true;
            _moneyImage.gameObject.SetActive(true);
            _actionText.text = LocalizationUI.Instance.GetLocaleName() == "ru"
                ? $"{russianButtonText[2]} {_price}"
                : $"{englishButtonText[2]} {_price}";
            _actionButton.GetComponent<Image>().color = Color.green;
        }
    }
}
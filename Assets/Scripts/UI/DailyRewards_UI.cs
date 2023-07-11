using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class DailyReward
{
    public RewardType Type;
    public int Amount;

    public enum RewardType
    {
        Currency,
        Skin
    }
}

public class DailyRewards_UI : MonoBehaviour
{
    [SerializeField] private CharactersData _charactersData;
    [SerializeField] private List<DailyReward> _rewards;
    [SerializeField] private List<Image> _rewardsImages;
    [SerializeField] private List<TextMeshProUGUI> _rewardTexts;

    private bool _canClaimReward;

    private int _currentStreak
    {
        get => PlayerPrefs.GetInt("CurrentStreak", 0);
        set => PlayerPrefs.SetInt("CurrentStreak", value);
    }

    private DateTime? _lastClaimTime
    {
        get
        {
            string data = PlayerPrefs.GetString("LastClaimTime", null);

            if (!string.IsNullOrEmpty(data))
                return DateTime.Parse(data);

            return null;
        }
        set
        {
            if (value != null)
                PlayerPrefs.SetString("LastClaimTime", value.ToString());
            else
                PlayerPrefs.DeleteKey("LastClaimTime");
        }
    }

    private void Awake()
    {
        LobbyManager.OnServicesInitialized += Activate;
        gameObject.transform.DOScale(0f,0.2f).SetEase(Ease.Linear);
    }

    private void OnDestroy()
    {
        LobbyManager.OnServicesInitialized -= Activate;
    }

    private void Activate()
    {
        gameObject.transform.DOScale(1f,0.2f).SetEase(Ease.Linear);
        UpdateRewardsState();
    }

    private void UpdateRewardsState()
    {
        _canClaimReward = true;

        if (_lastClaimTime.HasValue)
        {
            var timeSpan = DateTime.Now - _lastClaimTime.Value;

            switch (timeSpan.TotalDays)
            {
                case >= 2:
                    _lastClaimTime = null;
                    _currentStreak = 0;
                    gameObject.transform.DOScale(1f,0.2f).SetEase(Ease.Linear);
                    break;
                case < 1:
                    _canClaimReward = false;
                    gameObject.transform.DOScale(0f,0.2f).SetEase(Ease.Linear);
                    break;
            }
        }

        UpdateRewardsUI();
    }

    public void ClaimReward()
    {
        if (!_canClaimReward)
            return;

        var reward = _rewards[_currentStreak];

        switch (reward.Type)
        {
            case DailyReward.RewardType.Currency:
                PlayerEconomy.Instance.IncrementBalance(reward.Amount);
                break;
            case DailyReward.RewardType.Skin:
                int skinIndex = Random.Range(0, _charactersData.Characters.Count);
                
                PlayerEconomy.Instance.AddItem(_charactersData.Characters.ElementAt(skinIndex).Key);
                break;
        }

        Debug.Log($"Claim {reward.Type} - {reward.Amount}");

        _lastClaimTime = DateTime.Now;
        _currentStreak = (_currentStreak + 1) % _rewards.Count;

        UpdateRewardsState();
    }

    private void UpdateRewardsUI()
    {
        for (int i = 0; i < _rewardsImages.Count; i++)
        {
            float opacity = _currentStreak > i ? 0.3f : 1f;

            if (i == _currentStreak)
            {
                _rewardsImages[i].GetComponent<Outline>().enabled = _canClaimReward;
            }

            _rewardsImages[i].color = new Color(1, 1, 1, opacity);
        }

        for (int i = 0; i < _rewards.Count; i++)
        {
            if (_rewards[i].Type == DailyReward.RewardType.Currency)
            {
                _rewardTexts[i].text = $"{_rewards[i].Amount}";
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class AdWarningText : MonoBehaviour
{
    [SerializeField] private Button confirmButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(ConfirmAndShowAd);
    }

    private void ConfirmAndShowAd()
    {
        gameObject.SetActive(false);
        
        if (Ads.Instance != null)
        {
            Ads.Instance.ShowAd();
        }
    }

}

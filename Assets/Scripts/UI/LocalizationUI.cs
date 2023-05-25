using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class LocalizationUI : MonoBehaviour
{
    #region Singleton

    public static LocalizationUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public static event Action<string> OnLocaleChange;

    public void LoadLocale(string languageIdentifier)
    {
        LocalizationSettings settings = LocalizationSettings.Instance;
        LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);//can be "en" "de" "ja" etc.
        for(int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
            LocaleIdentifier anIdentifier = aLocale.Identifier;
            if(anIdentifier == localeCode)
            {
                LocalizationSettings.SelectedLocale = aLocale;
            }
        }
        Debug.Log(GetCurrentLocaleName());
        OnLocaleChange?.Invoke(GetCurrentLocaleName());
    }

    private Locale _currentLocale;

    public string GetCurrentLocaleName()
    {
        return LocalizationSettings.SelectedLocale.LocaleName;
    }
}

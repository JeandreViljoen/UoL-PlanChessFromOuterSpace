using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class HUDManager : MonoService
{
    public TextMeshProUGUI CurrencyDisplay;

    private EasyService<CurrencyManager> _currencyManager;
    void Start()
    {
        SubscribeCurrencyEvents();
        RefreshCurrencyDisplay(0);
    }

    private void RefreshCurrencyDisplay(int amount)
    {
        CurrencyDisplay.text = $"CURRENCY: {_currencyManager.Value.Currency}";
        CurrencyDisplay.color = GlobalGameAssets.Instance.HighlightColor;
    }

    private void SubscribeCurrencyEvents()
    {
        _currencyManager.Value.OnCurrencyRemoved += RefreshCurrencyDisplay;
        _currencyManager.Value.OnCurrencyAdded += RefreshCurrencyDisplay;
    }
    
    private void UnSubscribeCurrencyEvents()
    {
        _currencyManager.Value.OnCurrencyRemoved -= RefreshCurrencyDisplay;
        _currencyManager.Value.OnCurrencyAdded -= RefreshCurrencyDisplay;
    }

    void Update()
    {
        
    }

    private void OnDestroy()
    {
        UnSubscribeCurrencyEvents();
    }
}

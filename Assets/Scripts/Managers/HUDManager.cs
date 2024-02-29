using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class HUDManager : MonoService
{
    public TextMeshProUGUI CurrencyDisplay;
    public GameObject StatsPanel;
    public GameObject LosePrompt;
    public KingController KingController;
    

    private EasyService<CurrencyManager> _currencyManager;
    void Start()
    {
        SubscribeCurrencyEvents();
        RefreshCurrencyDisplay(0);
        LosePrompt.SetActive(false);
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            StatsPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        UnSubscribeCurrencyEvents();
    }
}

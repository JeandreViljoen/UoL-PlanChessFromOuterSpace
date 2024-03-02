using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoService
{
    public TextMeshProUGUI CurrencyDisplay;
    public TextMeshProUGUI TurnCounter;
    public GameObject StatsPanel;
    public GameObject LosePrompt;
    public GameObject WinPrompt;
    public GameObject DeployText;
    public KingController KingController;
    public ShopManagerScript ShopMenu;
    public UnitOrderTimelineController TimelineController;
    public Image DimPanel;
    

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
            if (StatsPanel.activeSelf)
            {
                StatsPanel.SetActive(false);
            }
            else
            {
                StatsPanel.SetActive(true);
            }
            
        }
    }

    private void OnDestroy()
    {
        UnSubscribeCurrencyEvents();
    }

    public void SetTurnCounter(int turn)
    {
        TurnCounter.text = $"ROUND {turn}";
    }

}

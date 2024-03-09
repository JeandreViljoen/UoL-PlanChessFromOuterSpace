using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManagerScript : MonoService
{
    public TextMeshProUGUI CurrencyText;

    private EasyService<CurrencyManager> _currencyManager;

    private Tween _tweenMove;
    private Vector3 _showPos;

    private void Awake()
    {
        _showPos = transform.localPosition;
    }

    void Start()
    {
        //When currency added/removed, fire RefreshCurrencyField function
        _currencyManager.Value.OnCurrencyAdded += RefreshCurrencyField; 
        _currencyManager.Value.OnCurrencyRemoved += RefreshCurrencyField;
        HideInstant();
    }

    void RefreshCurrencyField(int _)
    {
        CurrencyText.text = "Currency: " + _currencyManager.Value.Currency;
    }

    private void OnDestroy()
    {
        //Always remove event subscriptions
        _currencyManager.Value.OnCurrencyAdded -= RefreshCurrencyField;
        _currencyManager.Value.OnCurrencyRemoved -= RefreshCurrencyField;
    }

    public void Show()
    {
        _tweenMove?.Kill();
        _tweenMove = transform.DOLocalMove(_showPos, 0.2f).SetEase(Ease.InOutSine);
    }

    public void Hide()
    {
        _tweenMove.Kill();
        _tweenMove = transform.DOLocalMove(_showPos + Vector3.right * 500f, 0.2f).SetEase(Ease.InOutSine);
    }
    
    public void HideInstant()
    {
        _tweenMove.Kill();
        _tweenMove = transform.DOLocalMove(_showPos + Vector3.right * 500f, 0.00001f).SetEase(Ease.InOutSine);
    }
}

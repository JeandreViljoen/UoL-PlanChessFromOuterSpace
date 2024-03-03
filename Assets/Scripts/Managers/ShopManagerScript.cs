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
    //public int[,] shopItems = new int [6,6];
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
    }

    void RefreshCurrencyField(int _)
    {
        CurrencyText.text = "Currency: " + _currencyManager.Value.Currency;
    }
    
    
    //Not needed anymore, as buttons themselves keep track of values and interactions
    
                //Enables purchase of new units and updating of currency balance
                //public void Buy()
                //{
                    //GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

                    //_currencyManager.Value.TryRemoveCurrency(shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID]);
                    
                    // if(currencyBalance >= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID])
                    // {
                    //     currencyBalance -= shopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemID];
                    //     currencyTxt.text = "Currency: $"+_currencyManager.Value.Currency.ToString();
                    //     
                    //     //TO BE DONE
                    //     //add insertion of selected game piece on the board
                    //     //add updating of CurrencyManager Currency
                    //     //add exiting from Shop Menu (with or without changes)
                    // }
                //}

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
}

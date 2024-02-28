//Used by ShopManagerScript.cs

using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour
{
    public ChessPieceType Type; //Defines the accosiated piece type for this button
    public TextMeshProUGUI PriceText; // Text field to display _price
    private int _price;  //price value assigned to this button
    public TextMeshProUGUI BuyText; // Text field to display piece name
    public Image Sprite; // Sprite field to display unique sprite
    public MouseEventHandler EventHandler; //Event handler to hook events in to MouseEvents like click and hover etc.

    private EasyService<CurrencyManager> _currencyManager;
    public event Action<ChessPieceType> OnSuccessfulPurchase;
    public event Action<ChessPieceType> OnAttemptedPurchase;

    private EasyService<BoardManager> _boardManager;
    private void Start()
    {
        EventHandler = GetComponent<MouseEventHandler>();//Retrieve and store the event handler object
        EventHandler.OnMouseDown += TryBuy; //Fire TryBuy() function on Click event
        
        InitValues(); // Initialise starting values for button object
    }

    private void TryBuy(PointerEventData _)
    {
     
        if (_currencyManager.Value.HasCurrency(_price))
        {
            _boardManager.Value.EnableBuyingState(Type);
            OnAttemptedPurchase?.Invoke(Type);
        }
        else
        {
            //Code for handling not enough currency
        }
        
    }
    
    void Update()
    {
       
    }
    
    private void InitValues()
    {
        //Uses the TYPE of the piece assigned in inspector to assign values.
        switch (Type)
        {
            case ChessPieceType.Pawn:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.PawnCost;
                Sprite.sprite = GlobalGameAssets.Instance.PawnData.Sprite;
                break;
            case ChessPieceType.Knight:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.KnightCost;
                Sprite.sprite = GlobalGameAssets.Instance.KnightData.Sprite;
                break;
            case ChessPieceType.Bishop:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.BishopCost;
                Sprite.sprite = GlobalGameAssets.Instance.BishopData.Sprite;
                break;
            case ChessPieceType.Rook:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.RookCost;
                Sprite.sprite = GlobalGameAssets.Instance.RookData.Sprite;
                break;
            case ChessPieceType.Queen:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.QueenCost;
                Sprite.sprite = GlobalGameAssets.Instance.QueenData.Sprite;
                break;
            case ChessPieceType.King:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.KingCost;
                Sprite.sprite = GlobalGameAssets.Instance.KingData.Sprite;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //Update text fields.
        BuyText.text = $"Buy {Type.ToString()}";
        PriceText.text = "Price: " + _price;
    }

    private void OnDestroy()
    {
        EventHandler.OnMouseDown -= TryBuy; // Always unsubscribe events
    }
}

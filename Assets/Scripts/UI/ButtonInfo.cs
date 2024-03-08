//Used by ShopManagerScript.cs

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public Image Background; // Sprite field to display unique sprite
    public MouseEventHandler EventHandler; //Event handler to hook events in to MouseEvents like click and hover etc.

    private EasyService<CurrencyManager> _currencyManager;
    private EasyService<BoardManager> _boardManager;
    private EasyService<GameStateManager> _stateManager;
    private EasyService<AudioManager> _audioManager;

    private Tween _tweenMove;
    private Vector3 _startPos;
    
    private ButtonState _state;

    protected ButtonState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            EvaluateState(_state);
        }
    }

    private void Start()
    {
        EventHandler = GetComponent<MouseEventHandler>();//Retrieve and store the event handler object
        EventHandler.OnMouseDown += TryBuy; //Fire TryBuy() function on Click event
        EventHandler.OnMouseEnter += OnMouseEnterBehaviour;
        EventHandler.OnMouseExit += OnMouseExitBehaviour;
        
        _boardManager.Value.OnSuccessfulPurchase += ForceReset;
        _boardManager.Value.OnCancelledPurchase += ForceReset;

        InitValues(); // Initialise starting values for button object
    }

    private void TryBuy(PointerEventData _)
    {

        if (_currencyManager.Value.HasCurrency(_price))
        {
            _boardManager.Value.EnableBuyingState(Type);
            StartCoroutine(DelayedSelectState());
            _audioManager.Value.PlaySound(Sound.UI_Click, gameObject);
        }
        else
        {
            //Code for handling not enough currency
        }
        
    }

    IEnumerator DelayedSelectState()
    {
        yield return new WaitForSeconds(0.1f);
        State = ButtonState.Selected;
    }
    
    private void OnMouseEnterBehaviour (PointerEventData _)
    {
        if (State == ButtonState.Selected)
        {
            return;
        }
        State = ButtonState.Highlighted;
        _audioManager.Value.PlaySound(Sound.UI_Subtle, gameObject);
    }
    private void OnMouseExitBehaviour (PointerEventData _)
    {
        if (State == ButtonState.Selected)
        {
            return;
        }
        State = ButtonState.Base;
    }

    private void EvaluateState(ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Highlighted:
                HighlightButton();
                break;
            case ButtonState.Selected:
                HighlightButton();
                break;
            case ButtonState.Base:
                UnHighlightButton();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    void Update()
    {
    }

    public void ForceReset()
    {
        State = ButtonState.Base;
    }

    private void InitValues()
    {
        State = ButtonState.Base;
        
        //Uses the TYPE of the piece assigned in inspector to assign values.
        switch (Type)
        {
            case ChessPieceType.Pawn:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.PawnCost;
                Sprite.sprite = GlobalGameAssets.Instance.PawnData.Portrait;
                break;
            case ChessPieceType.Knight:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.KnightCost;
                Sprite.sprite = GlobalGameAssets.Instance.KnightData.Portrait;
                break;
            case ChessPieceType.Bishop:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.BishopCost;
                Sprite.sprite = GlobalGameAssets.Instance.BishopData.Portrait;
                break;
            case ChessPieceType.Rook:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.RookCost;
                Sprite.sprite = GlobalGameAssets.Instance.RookData.Portrait;
                break;
            case ChessPieceType.Queen:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.QueenCost;
                Sprite.sprite = GlobalGameAssets.Instance.QueenData.Portrait;
                break;
            case ChessPieceType.King:
                _price = GlobalGameAssets.Instance.CurrencyBalanceData.KingCost;
                Sprite.sprite = GlobalGameAssets.Instance.KingData.Portrait;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //Update text fields.
        BuyText.text = $"{Type.ToString()}";
        PriceText.text =  _price.ToString();
    }

    private void OnDestroy()
    {
        EventHandler.OnMouseDown -= TryBuy;
        EventHandler.OnMouseEnter -= OnMouseEnterBehaviour;
        EventHandler.OnMouseExit -= OnMouseExitBehaviour;
        
        _boardManager.Value.OnSuccessfulPurchase -= ForceReset;
        _boardManager.Value.OnCancelledPurchase -= ForceReset;
    }

    private void HighlightButton()
    {
        if (_startPos == Vector3.zero)
        {
            _startPos = Background.transform.localPosition;
        }
        Background.color = GlobalGameAssets.Instance.HighlightColor;
        _tweenMove?.Kill();
        _tweenMove = Background.transform.DOLocalMove( Vector3.left*30f, 0.15f).SetEase(Ease.InOutSine);
    }

    private void UnHighlightButton()
    {
        Background.color = Color.black;
        _tweenMove?.Kill();
        _tweenMove = Background.transform.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.InOutSine);
    }

}

public enum ButtonState
{
    Highlighted,
    Selected,
    Base
}
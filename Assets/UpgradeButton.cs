using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour
{
    public MouseEventHandler EventHandler;
    public SpriteRenderer BackGround;
    public SpriteRenderer HighlightBorder;
    public SpriteRenderer Icon;
    public TextMeshPro CostField;
    public GameObject CostPanel;
    public float AnimationSpeed = 0.15f;
    
    private int _cost;

    public int Cost
    {
        get
        {
            return _cost;
        }
        set
        {
            _cost = value;
            SetCostField(_cost);
        }
    }

    private Tween _tweenHighlightFade;
    private Tween _tweenHighlightScale;
    private Tween _tweenHighlightRotate;
    private Tween _tweenPriceMove;
    private Vector3 _priceStartPos;

    private Vector3 _startScale;

    private void OnEnable()
    {
        
        
    }

    private void Awake()
    {
        _priceStartPos = CostPanel.transform.localPosition;
    }

    void Start()
    {
        EventHandler.OnMouseEnter += OnHighlight;
        EventHandler.OnMouseDown += OnPress;
        EventHandler.OnMouseExit += OnUnHighlight;
        
        _tweenHighlightFade = HighlightBorder.DOFade(0f, AnimationSpeed).SetEase(Ease.InOutSine).SetUpdate(true);
        _startScale = transform.localScale;
        
        BackGround.color = GlobalGameAssets.Instance.HighlightColor;
    }
    
    void Update()
    {
        
    }

    private void OnHighlight(PointerEventData _)
    {
        _tweenHighlightFade?.Kill();
        _tweenHighlightScale?.Kill();
        _tweenHighlightRotate?.Kill();
        _tweenPriceMove?.Kill();
        
        _tweenHighlightScale = transform.DOScale(_startScale *1.05f, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightFade = HighlightBorder.DOFade(1f, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightRotate = HighlightBorder.transform.DOLocalRotate(new Vector3(0f,0f,60f), AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenPriceMove = CostPanel.transform.DOLocalMove(_priceStartPos + Vector3.right, AnimationSpeed).SetEase(Ease.InOutSine);

    }
    
    private void OnPress(PointerEventData _)
    {
        _tweenHighlightFade?.Kill();
        _tweenHighlightScale?.Kill();
        _tweenHighlightRotate?.Kill();
        _tweenPriceMove?.Kill();
        
        _tweenHighlightRotate = HighlightBorder.transform.DOLocalRotate(new Vector3(0f,0f,120f), AnimationSpeed).SetEase(Ease.InOutSine);

        Sequence s = DOTween.Sequence();
        s.Append(CostPanel.transform.DOLocalMove(_priceStartPos + Vector3.right*2, AnimationSpeed/2).SetEase(Ease.InOutSine));
        s.Append(CostPanel.transform.DOLocalMove(_priceStartPos + Vector3.right, AnimationSpeed/2).SetEase(Ease.InOutSine));
        _tweenPriceMove = s;

    }
    
    private void OnUnHighlight(PointerEventData _)
    {
        _tweenHighlightFade?.Kill();
        _tweenHighlightScale?.Kill();
        _tweenHighlightRotate?.Kill();
        _tweenPriceMove?.Kill();
        _tweenHighlightScale = transform.DOScale(_startScale, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightFade = HighlightBorder.DOFade(0f, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightRotate = HighlightBorder.transform.DOLocalRotate(new Vector3(0f,0f,0f), AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenPriceMove = CostPanel.transform.DOLocalMove(_priceStartPos, AnimationSpeed).SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        EventHandler.OnMouseEnter -= OnHighlight;
        EventHandler.OnMouseExit -= OnUnHighlight;
    }

    private void SetCostField(int cost)
    {
        CostField.text = cost.ToString();
        if (!ServiceLocator.GetService<CurrencyManager>()
                .HasCurrency(cost))
        {
            CostField.color = new Color(1f, 1f, 1f, 0.1f);
        }
        else
        {
            CostField.color = new Color(1f, 1f, 1f, 1f);
        }
    }

}

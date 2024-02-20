using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour
{
    public MouseEventHandler EventHandler;
    public SpriteRenderer HighlightBorder;
    public SpriteRenderer Icon;
    public float AnimationSpeed = 0.15f;

    private Tween _tweenHighlightFade;
    private Tween _tweenHighlightScale;
    private Tween _tweenHighlightRotate;

    private Vector3 _startScale;

    private void OnEnable()
    {
        
        
    }

    void Start()
    {
        EventHandler.OnMouseEnter += OnHighlight;
        EventHandler.OnMouseExit += OnUnHighlight;
        
        _tweenHighlightFade = HighlightBorder.DOFade(0f, AnimationSpeed).SetEase(Ease.InOutSine).SetUpdate(true);
        _startScale = transform.localScale;
        
        Icon.color = GlobalGameAssets.Instance.HighlightColor;
        HighlightBorder.color = GlobalGameAssets.Instance.HighlightColor;
    }
    
    void Update()
    {
        
    }

    private void OnHighlight(PointerEventData _)
    {
        _tweenHighlightFade?.Kill();
        _tweenHighlightScale?.Kill();
        _tweenHighlightRotate?.Kill();
        _tweenHighlightScale = transform.DOScale(_startScale *1.05f, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightFade = HighlightBorder.DOFade(1f, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightRotate = HighlightBorder.transform.DOLocalRotate(new Vector3(0f,0f,60f), AnimationSpeed).SetEase(Ease.InOutSine);

    }
    
    private void OnUnHighlight(PointerEventData _)
    {
        _tweenHighlightFade?.Kill();
        _tweenHighlightScale?.Kill();
        _tweenHighlightRotate?.Kill();
        _tweenHighlightScale = transform.DOScale(_startScale, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightFade = HighlightBorder.DOFade(0f, AnimationSpeed).SetEase(Ease.InOutSine);
        _tweenHighlightRotate = HighlightBorder.transform.DOLocalRotate(new Vector3(0f,0f,0f), AnimationSpeed).SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        EventHandler.OnMouseEnter -= OnHighlight;
        EventHandler.OnMouseExit -= OnUnHighlight;
    }
}

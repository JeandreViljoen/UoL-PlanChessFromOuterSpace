using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HoverAnimate : MonoBehaviour
{
    private Tween _hoverTween;
    
    public float HoverHeight = 1f;
    public float HoverSpeed = 1f;

    private Vector3 _startPos;
    private Vector3 _hoverPos;

    private void Awake()
    {
        _startPos = gameObject.transform.localPosition;
        _hoverPos = new Vector3(_startPos.x, _startPos.y + HoverHeight, _startPos.z);
    }

    void Start()
    {
        HoverUp();
    }

    private void HoverUp()
    {
        _hoverTween?.Kill();
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(_hoverPos, HoverSpeed).SetEase(Ease.InOutSine));
        s.AppendCallback(() => { HoverDown(); });
        _hoverTween = s;
    }

    private void HoverDown()
    {
        _hoverTween?.Kill();
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(_startPos, HoverSpeed).SetEase(Ease.InOutSine));
        s.AppendCallback(() => { HoverUp(); });
        _hoverTween = s;
    }


}

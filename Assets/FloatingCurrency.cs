using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class FloatingCurrency : MonoBehaviour
{
    public TextMeshPro TextField;

    private Tween _tweenMove;
    //private Tween _tweenFade;
   
    void Start()
    {
        TextField = GetComponentInChildren<TextMeshPro>();
    }

    public void Init(int value)
    {
        TextField.text = value > 0 ? $"{value}" : $"{value}";

        Sequence s = DOTween.Sequence();
        s.Append(TextField.transform.DOLocalMove(TextField.transform.localPosition + Vector3.up * 1f, 1f).SetEase(Ease.OutSine));
        s.Append(TextField.DOFade(0f, 0.5f));
        s.AppendCallback(() => {
            Destroy(gameObject);
        });
        _tweenMove = s;
    }

    private void OnDestroy()
    {
        _tweenMove?.Kill();
       
    }
}

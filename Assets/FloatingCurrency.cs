using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class FloatingCurrency : MonoBehaviour
{
    [CanBeNull] public TextMeshPro TextField;
    [CanBeNull] public TextMeshProUGUI TextFieldUI;

    private Tween _tweenMove;

    public void Init(int value)
    {
        if (TextField == null)
        {
            return;
        }
        
        TextField.text = value > 0 ? $"+{value}" : $"{value}";

        Sequence s = DOTween.Sequence();
        s.Append(TextField.transform.DOLocalMove(TextField.transform.localPosition + Vector3.up * 1f, 1f).SetEase(Ease.OutSine));
        s.Append(TextField.DOFade(0f, 0.5f));
        s.AppendCallback(() => {
            Destroy(gameObject);
        });
        _tweenMove = s;
    }
    
    public void InitUI(int value)
    {
        if (TextFieldUI == null)
        {
            return;
        }
        
        TextFieldUI.text = value > 0 ? $"+{value}" : $"{value}";

        Sequence s = DOTween.Sequence();
        s.Append(TextFieldUI.transform.DOLocalMove(TextFieldUI.transform.localPosition + Vector3.down * 100f, 1f).SetEase(Ease.OutSine));
        s.Append(TextFieldUI.DOFade(0f, 0.5f));
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

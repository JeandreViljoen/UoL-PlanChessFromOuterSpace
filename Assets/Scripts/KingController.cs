﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;

public class KingController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _deployKingPrompt;

    private Tween _tweenKingPrompt;
    private Vector3 _startPos;

    private void Awake()
    {
        _startPos = transform.localPosition;
    }

    void Start()
    {
        _tweenKingPrompt = _deployKingPrompt.DOFade(0f, 00001f).SetUpdate(true);
        gameObject.SetActive(false);
    }

    public void ShowDeployKingPrompt()
    {
        ServiceLocator.GetService<AudioManager>().PlaySound(Sound.UI_Deny);
        _tweenKingPrompt?.Kill();
        Sequence s = DOTween.Sequence();
        s.Append(_deployKingPrompt.DOFade(1f, 0.1f).SetEase(Ease.InOutSine));
        s.AppendInterval(2f);
        s.Append(_deployKingPrompt.DOFade(0f, 2f).SetEase(Ease.InOutSine));
        _tweenKingPrompt = s;
    }

    public void Disable()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(_startPos + Vector3.right * 1000f, 0.3f).SetEase(Ease.InOutSine));
        s.AppendCallback(() => { gameObject.SetActive(false); });
    }
}

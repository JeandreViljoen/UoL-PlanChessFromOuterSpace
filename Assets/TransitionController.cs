using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;

public class TransitionController : MonoService
{
    public TextMeshProUGUI TransitionText;
    private EasyService<GameStateManager> _stateManager;
    
    private Vector3 _startPos = new Vector3(-1920f,0f,0f);
    private Vector3 _endPos = new Vector3(1920f,0f,0f);
    private Vector3 _showPos = new Vector3(0f,0f,0f);

    private Tween _tweenShow;

    public event Action<GameState> OnTransitionStart; 

    void Start()
    {
        TransitionText.transform.localPosition = _startPos;
    }

   
    void Update()
    {
        
    }

    public void Transition(GameState stateTransitioningTo, float preDelay = 0f, float holdDelay = 1f, float postDelay = 0.5f)
    {
        _tweenShow?.Kill();
        TransitionText.transform.localPosition = _startPos;
        SetText(stateTransitioningTo);
        
        Sequence t = DOTween.Sequence();
        t.AppendInterval(preDelay);
        t.AppendCallback(() => { OnTransitionStart?.Invoke(stateTransitioningTo); });
        t.Append(TransitionText.transform.DOLocalMove(_showPos, 0.15f).SetEase(Ease.InOutSine));
        t.AppendInterval(holdDelay);
        t.Append(TransitionText.transform.DOLocalMove( _endPos, 0.15f).SetEase(Ease.InOutSine));
        t.AppendInterval(postDelay);
        t.AppendCallback(() => { _stateManager.Value.GameState = stateTransitioningTo; });
        _tweenShow = t;

    }

    private void SetText(GameState stateTransitioningTo)
    {
        TransitionText.text = stateTransitioningTo.ToString();
    }
}

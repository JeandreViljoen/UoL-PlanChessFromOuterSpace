using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;

public class CheckUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _sub;

    private Tween _fadeHeader;
    private Tween _fadeSub;
    void Start()
    {
        _fadeHeader = _header.DOFade(0f, 0.0001f);
        _fadeSub = _sub.DOFade(0f, 0.0001f);

        ServiceLocator.GetService<BoardManager>().OnKingChecked +=  Show;
        ServiceLocator.GetService<BoardManager>().OnNoKingChecked += Hide;
        ServiceLocator.GetService<GameStateManager>().OnStateChanged += HideOnLose;
    }

    void Update()
    {
        
    }

    public void Show(ChessPiece _)
    {
        _fadeHeader?.Kill();
        _fadeHeader = _header.DOFade(1f, 0.2f);
        
        _fadeSub?.Kill();
        _fadeSub = _sub.DOFade(1f, 0.5f);
    }

    public void Hide()
    {
        _fadeHeader?.Kill();
        _fadeHeader = _header.DOFade(0f, 0.2f);
        
        _fadeSub?.Kill();
        _fadeSub = _sub.DOFade(0f, 0.2f);
    }

    private void HideOnLose(GameState state)
    {
        if (state == GameState.LOSE)
        {
            Hide();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class HudButtonsManager : MonoBehaviour
{
    public Button PauseButton;
    public Button FightButton;
    public Button ReturnButton;

    private EasyService<MenuManager> _menuManager;
    private EasyService<GameStateManager> _stateManager;
    private EasyService<TransitionController> _transitionController;
    private EasyService<CameraManager> _cameraManager;
    private EasyService<AudioManager> _audioManager;

    private Tween _tweenFightButton;
    private Vector3 _fightButtonStartPos;
    private Vector3 _fightButtonHidePos;
    private Tween _tweenBackButton;
    private Vector3 _backButtonStartPos;
    private Vector3 _backButtonHidePos;

    public float ButtonHideSpeed = 0.15f;

    public Sprite PauseIcon;
    public Sprite PlayIcon;

    private void Awake()
    {
        _fightButtonStartPos = FightButton.transform.localPosition;
        _backButtonStartPos = ReturnButton.transform.localPosition;

        _fightButtonHidePos = _fightButtonStartPos + Vector3.right * 500f;
        _backButtonHidePos = _backButtonStartPos + Vector3.down * 500f;
    }

    void Start()
    {
        PauseButton.onClick.AddListener(OnPauseButtonPressed);
        FightButton.onClick.AddListener(OnFightButtonPressed);
        ReturnButton.onClick.AddListener(OnReturnButtonPressed);

        //_transitionController.Value.OnTransitionStart += TryShowFightButton;
        _stateManager.Value.OnStateChanged += TryShowFightButton;
        _cameraManager.Value.OnCameraFocus += ShowBackButton;
        _cameraManager.Value.OnCameraTopDown += HideBackButton;

        _tweenFightButton = FightButton.transform.DOLocalMove(_fightButtonHidePos, 0.00001f);
        _tweenBackButton = ReturnButton.transform.DOLocalMove(_backButtonHidePos, 0.000001f);
    }

    void OnPauseButtonPressed()
    {
        if (_menuManager.Value.IsPaused)
        {
            PauseButton.image.sprite = PauseIcon;
            _menuManager.Value.PauseMenu.Resume();
        }
        else
        {
            PauseButton.image.sprite = PlayIcon;
            _menuManager.Value.PauseMenu.Pause();
        }
        _audioManager.Value.PlaySound(Sound.PauseButton, PauseButton.gameObject);
    }
    
    void OnFightButtonPressed()
    {
        if (_stateManager.Value.GameState == GameState.PREP)
        {
            if (!_stateManager.Value.HasPlacedKing)
            {
                ServiceLocator.GetService<HUDManager>().KingController.ShowDeployKingPrompt();
                return;
            }
            _audioManager.Value.PlaySound(Sound.UI_Sub, FightButton.gameObject);
            _stateManager.Value.GameState = GameState.COMBAT;
            HideFightButton();
        }
        
    }
    
    void OnReturnButtonPressed()
    {
        _cameraManager.Value.ResetCameraPosition();
        _audioManager.Value.PlaySound(Sound.UI_Click, ReturnButton.gameObject);
    }
    
    void Update()
    {
        
    }

    private void TryShowFightButton(GameState state)
    {
        if (state == GameState.PREP)
        {
            _tweenFightButton?.Kill();
            _tweenFightButton = FightButton.transform.DOLocalMove(_fightButtonStartPos, ButtonHideSpeed).SetEase(Ease.InOutSine);
        }
    }

    private void HideFightButton()
    {
        _tweenFightButton?.Kill();
        _tweenFightButton = FightButton.transform.DOLocalMove(_fightButtonHidePos, ButtonHideSpeed).SetEase(Ease.InOutSine);
    }
    
    private void ShowBackButton(BoardSquare _)
    {
        _tweenBackButton?.Kill();
        _tweenBackButton = ReturnButton.transform.DOLocalMove(_backButtonStartPos, ButtonHideSpeed).SetEase(Ease.InOutSine);
    }

    private void HideBackButton()
    {
        _tweenBackButton?.Kill();
        _tweenBackButton = ReturnButton.transform.DOLocalMove(_backButtonHidePos, ButtonHideSpeed).SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        if (_transitionController.HasService())
        {
            _transitionController.Value.OnTransitionStart -= TryShowFightButton;
        }
        
        _cameraManager.Value.OnCameraFocus -= ShowBackButton;
        _cameraManager.Value.OnCameraTopDown -= HideBackButton;
    }
}

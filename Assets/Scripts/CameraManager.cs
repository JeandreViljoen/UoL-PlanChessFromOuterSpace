﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;

public class CameraManager : MonoService
{
    private Camera _mainCam;
    [SerializeField] private Light _mainCamLight1;
    [SerializeField] private Light _mainCamLight2;
    [SerializeField] private float _lightBrightness;
    private Tween _tweenLight1;
    private Tween _tweenLight2;

    private Tween _tweenPosition;
    private Tween _tweenRotation;
    private Tween _tweenFOV;

    public float TransitionSpeed;

    public Vector3 ResetPosition;
    public Vector3 ResetRotation;
    public float ResetFOV;
    
    public float FocusHeight;
    public float FocusXRotation;
    public float FocusFOV;

    private EasyService<AudioManager> _audioManager;

    public event Action<BoardSquare> OnCameraFocus;
    public event Action OnCameraTopDown;
    
    
    private void Awake()
    {
        _mainCam = Camera.main;
    }

    void Start()
    {
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetCameraPosition();
        }
    }

    public void ResetCameraPosition()
    {
        LightsOn();
        _tweenPosition?.Kill();
        _tweenRotation?.Kill();
        _tweenFOV?.Kill();

        _tweenPosition = _mainCam.transform.DOMove(ResetPosition, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenRotation = _mainCam.transform.DORotate(ResetRotation, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenFOV = _mainCam.DOFieldOfView(ResetFOV, TransitionSpeed).SetEase(Ease.InOutSine);

        ServiceLocator.GetService<BoardManager>().SelectedUnit = null;
        //ServiceLocator.GetService<BoardManager>().EnableAllPieceLights();
        _audioManager.Value.PlaySound(Sound.UI_CameraMove, _mainCam.gameObject);
        OnCameraTopDown?.Invoke();
        

    }

    public void FocusTile(ChessPiece piece)
    {
        LightsOff();
        BoardSquare tile = piece.AssignedSquare;

        Vector3 focusPosition = new Vector3(tile.IndexZ, FocusHeight, tile.IndexX - 0.6f);
        Vector3 focusRotation = new Vector3(FocusXRotation, 0f, 0f);
        
        //ServiceLocator.GetService<BoardManager>().DisableAllPieceLights();
        
        _tweenPosition?.Kill();
        _tweenRotation?.Kill();
        _tweenFOV?.Kill();

        _tweenPosition = _mainCam.transform.DOMove(focusPosition, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenRotation = _mainCam.transform.DORotate(focusRotation, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenFOV = _mainCam.DOFieldOfView(FocusFOV, TransitionSpeed).SetEase(Ease.InOutSine);

        _audioManager.Value.PlaySound(Sound.FocusTile, _mainCam.gameObject);
        OnCameraFocus?.Invoke(tile);
    }

    public void LightsOn()
    {
        _tweenLight1?.Kill();
        _tweenLight1 = _mainCamLight1.DOIntensity(_lightBrightness, TransitionSpeed);
        
        _tweenLight2?.Kill();
        _tweenLight2 = _mainCamLight2.DOIntensity(_lightBrightness, TransitionSpeed);
    }
    
    public void LightsOff()
    {
        _tweenLight1?.Kill();
        _tweenLight1 = _mainCamLight1.DOIntensity(0.5f, TransitionSpeed);
        
        _tweenLight2?.Kill();
        _tweenLight2 = _mainCamLight2.DOIntensity(0.5f, TransitionSpeed);
    }
}

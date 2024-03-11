using System;
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

    public GameObject Spaceship;

    private EasyService<AudioManager> _audioManager;

    public event Action<BoardSquare> OnCameraFocus;
    public event Action OnCameraTopDown;
    
    private bool _isFocused = false;
    
    private void Awake()
    {
        _mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isFocused)
            {
                ResetCameraPosition();
            }
            else
            {
                ServiceLocator.GetService<MenuManager>().PauseMenu.Pause();
            }
            
        }
    }

    /// <summary>
    /// Resets The camera position to top-down view
    /// </summary>
    public void ResetCameraPosition()
    {
        _isFocused = false;
        LightsOn();
        _tweenPosition?.Kill();
        _tweenRotation?.Kill();
        _tweenFOV?.Kill();

        _tweenPosition = _mainCam.transform.DOMove(ResetPosition, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenRotation = _mainCam.transform.DORotate(ResetRotation, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenFOV = _mainCam.DOFieldOfView(ResetFOV, TransitionSpeed).SetEase(Ease.InOutSine);

        ServiceLocator.GetService<BoardManager>().SelectedUnit = null;
        _audioManager.Value.PlaySound(Sound.UI_CameraMove, _mainCam.gameObject);
        OnCameraTopDown?.Invoke();
        
    }

    /// <summary>
    /// Focus a the camera on the given BoardSquare in parameters
    /// </summary>
    /// <param name="piece"></param>
    public void FocusTile(ChessPiece piece)
    {
        _isFocused = true;
        LightsOff();
        BoardSquare tile = piece.AssignedSquare;

        Vector3 focusPosition = new Vector3(tile.IndexZ, FocusHeight, tile.IndexX - 0.6f);
        Vector3 focusRotation = new Vector3(FocusXRotation, 0f, 0f);

        _tweenPosition?.Kill();
        _tweenRotation?.Kill();
        _tweenFOV?.Kill();

        _tweenPosition = _mainCam.transform.DOMove(focusPosition, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenRotation = _mainCam.transform.DORotate(focusRotation, TransitionSpeed).SetEase(Ease.InOutSine);
        _tweenFOV = _mainCam.DOFieldOfView(FocusFOV, TransitionSpeed).SetEase(Ease.InOutSine);

        _audioManager.Value.PlaySound(Sound.FocusTile, _mainCam.gameObject);
        _audioManager.Value.PlaySound(Sound.GAME_Spaceship, Spaceship);
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

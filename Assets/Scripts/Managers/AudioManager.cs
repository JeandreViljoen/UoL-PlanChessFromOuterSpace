using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using UnityEngine.UI;

public class AudioManager : MonoService
{
    
    // --- Sounds for the game ---
    // UI
    public AudioClip PauseButtonSound;
    public AudioClip UiButtonPressed;
    
    // Gameplay 
    public AudioClip BackgroundMusic;
    public AudioClip FocusTileSound;
    public AudioClip ReturnCameraTopDownSound;
    public AudioClip SuccessfulSound;
    public AudioClip FailSound;
    
    // --- AudioSource for playing audios
    public GameObject AudioObject;
    private AudioSource _soundEffectAudioSource;
    private AudioSource _backgroundMusicAudioSource;
    
    // -- Service Initialization
    private bool _isInitialized = false;
    
    // --- Buttons
    public Button PauseButton;
    public List<Button> UIButtons;
    
    // --- Member methods & functions ---
    private void Start()
    {
        if (!AudioObject)
            return;

        // Find AudioSources for playing clips
        _soundEffectAudioSource = AudioObject.transform.Find("SoundEffectAudio").GetComponent<AudioSource>();
        _backgroundMusicAudioSource = AudioObject.transform.Find("BackgroundAudio").GetComponent<AudioSource>(); 
        
        // Initialize button listeners
        foreach (var button in UIButtons)
        {
            button.onClick.AddListener(PlayUIButtonSFX);
            Debug.Log($"AudioManager: Button from game object #{button.gameObject.name} added as UI button.");
        }

        // Set Pause Button sound
        if (PauseButton)
        {
            PauseButton.onClick.AddListener(PlayPauseButtonSFX);
        }
        else
        {
            Debug.LogWarning("There is no pause button assigned to the AudioManager.");
        }
        
        // Camera management sounds
        if (FocusTileSound)
            ServiceLocator.GetService<CameraManager>().OnCameraFocus += PlayFocusTileSFX;
        if (ReturnCameraTopDownSound)
            ServiceLocator.GetService<CameraManager>().OnCameraTopDown += TopDownCameraSFX;
        
        // Init background music
        _backgroundMusicAudioSource.clip = BackgroundMusic;
        _backgroundMusicAudioSource.Play();
        
        _isInitialized = true;
    }

    public void PlaySuccessSFX()
    {
        PlaySFX(SuccessfulSound);
    }

    public void PlayFailSFX()
    {
        PlaySFX(FailSound);
    }

    private void PlayUIButtonSFX()
    {
        PlaySFX(UiButtonPressed);
    }

    private void PlayPauseButtonSFX()
    {
        PlaySFX(PauseButtonSound);
    }

    private void PlayFocusTileSFX(BoardSquare tile)
    {
        PlaySFX(FocusTileSound);
    }

    private void TopDownCameraSFX()
    {
        PlaySFX(ReturnCameraTopDownSound);
    }
    
    // Generic use Play SFX
    private void PlaySFX(AudioClip clip)
    {
        _soundEffectAudioSource.clip = clip;
        _soundEffectAudioSource.Play();
    }
    
}

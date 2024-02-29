using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;
using UnityEngine.UI;

public class AudioManager : MonoService
{
    
    // --- AudioSource for playing audios
    public GameObject AudioObject;
    private AudioSource _soundEffectAudioSource;
    private AudioSource _backgroundMusicAudioSource;
    
    // -- Service Initialization
    private bool _isInitialized = false;

    // --- Member methods & functions ---
    private void Start()
    {
        if (!AudioObject)
            return;

        // Find AudioSources for playing clips
        _soundEffectAudioSource = AudioObject.transform.Find("SoundEffectAudio").GetComponent<AudioSource>();
        _backgroundMusicAudioSource = AudioObject.transform.Find("BackgroundAudio").GetComponent<AudioSource>();

        // Init background music
        PlayMusic(Music.Track1);
        
        _isInitialized = true;
    }

    /// <summary>
    /// Play sound of given sound ID in parameters
    /// </summary>
    /// <param name="id">Sound enum of audio clip to play</param>
    public void PlaySound(Sound id)
    {
        AudioClip clip = GetClipFromID(id);
        
        _soundEffectAudioSource.clip = clip;
        _soundEffectAudioSource.Play();
    }
    
    /// <summary>
    /// Play music of given music ID in parameters
    /// </summary>
    /// <param name="id">Music enum of audio clip to play</param>
    public void PlayMusic(Music id)
    {
        AudioClip clip = GetMusicFromID(id);
        
        _backgroundMusicAudioSource.clip = clip;
        _backgroundMusicAudioSource.Play();
    }

    //Fetch audio clip from data object using given Sound ID enum
    private AudioClip GetClipFromID(Sound id)
    {
        AudioData data = GlobalGameAssets.Instance.AudioData;
        
        switch (id)
        {
            case Sound.PauseButton: return data.PauseButtonSound;
            case Sound.GenericUIButton: return data.UiButtonPressed;
            case Sound.HoverButton: return data.HoverButton;
            case Sound.FocusTile: return data.FocusTileSound;
            case Sound.ReturnCamera: return data.ReturnCameraTopDownSound;
            case Sound.Success: return data.SuccessfulSound;
            case Sound.Fail: return data.FailSound;
            default:
                throw new ArgumentOutOfRangeException(nameof(id), id, null);
        }
    }
    
    //Fetch audio clip from data object using given Music ID enum
    private AudioClip GetMusicFromID(Music id)
    {
        AudioData data = GlobalGameAssets.Instance.AudioData;
        
        switch (id)
        {
            case Music.Track1: return data.BackgroundMusic;
            default:
                throw new ArgumentOutOfRangeException(nameof(id), id, null);
        }
    }
    
}

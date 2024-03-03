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
    
    public void PlaySound(Sound id, GameObject sourceObj)
    {
        AudioSource source = sourceObj.GetComponent<AudioSource>();
        if (source == null)
        {
            Debug.LogError($"[AudioManager.cs] : PlaySound(_,_) - Could not find AudioSource on object requesting audio." +
                           $"\nMake sure the calling script ({sourceObj}) has an AudioSource component attached.");
            return;
        }
        
        AudioClip clip = GetClipFromID(id);
        
        source.clip = clip;
        source.Play();
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
            case Sound.PauseButton: return RandomClip(data.PauseButtonSound);
            case Sound.GenericUIButton: return RandomClip(data.UiButtonPressed);
            case Sound.FocusTile: return RandomClip(data.FocusTileSound);
            case Sound.ReturnCamera: return RandomClip(data.ReturnCameraTopDownSound);
            case Sound.Success: return RandomClip(data.SuccessfulSound);
            case Sound.Fail: return RandomClip(data.FailSound);
            case Sound.UI_Hover: return RandomClip(data.UI_Hover);
            case Sound.UI_Subtle: return RandomClip(data.UI_Subtle);
            case Sound.UI_Click: return RandomClip(data.UI_Click);
            case Sound.UI_Sub: return RandomClip(data.UI_Sub);
            case Sound.UI_UpgradeSuccess: return RandomClip(data.UI_UpgradeSuccess);
            case Sound.UI_CameraMove: return RandomClip(data.UI_CameraMove);
            case Sound.UI_Deny: return RandomClip(data.UI_Deny);
            case Sound.Lightning: return RandomClip(data.Lightning);

            case Sound.ENEMY_Move: return RandomClip(data.EnemyMove);
            case Sound.ENEMY_Activate: return RandomClip(data.EnemyActivate);
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

    private AudioClip RandomClip(List<AudioClip> clips)
    {
        if (clips.Count == 0)
        {
            Debug.LogError($"[AudioManager.cs] : RandomClip - Requested a random choice from an empty list. Make sure no empty lists are present in the AudioData scriptable");
            return null;
        }
        int randomIndex = UnityEngine.Random.Range(0, clips.Count);

        return clips[randomIndex];

    }
}

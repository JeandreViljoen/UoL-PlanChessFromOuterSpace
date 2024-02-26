using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Custom Assets/New Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("UI")]
    public AudioClip PauseButtonSound;
    public AudioClip UiButtonPressed;
    
    [Header("Gameplay")]
    public AudioClip FocusTileSound;
    public AudioClip ReturnCameraTopDownSound;
    public AudioClip SuccessfulSound;
    public AudioClip FailSound;
    
    [Header("Music")]
    public AudioClip BackgroundMusic;
}

public enum Sound
{
    PauseButton,
    GenericUIButton,
    FocusTile,
    ReturnCamera,
    Success,
    Fail
}

public enum Music
{
    Track1
}
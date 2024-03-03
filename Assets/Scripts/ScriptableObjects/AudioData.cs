using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Custom Assets/New Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("UI")]
    public List<AudioClip> PauseButtonSound;
    public List<AudioClip> UiButtonPressed;

    public List<AudioClip> UI_Hover;
    public List<AudioClip> UI_Subtle;
    public List<AudioClip> UI_Click;
    public List<AudioClip> UI_Sub;
    public List<AudioClip> UI_UpgradeSuccess;
    public List<AudioClip> UI_CameraMove;
    public List<AudioClip> UI_Deny;


    [Header("Gameplay")]
    public List<AudioClip> FocusTileSound;
    public List<AudioClip> ReturnCameraTopDownSound;
    public List<AudioClip> SuccessfulSound;
    public List<AudioClip> FailSound;
    public List<AudioClip> Lightning;
    
    [Header("Enemies")]
    public List<AudioClip> EnemyActivate;
    public List<AudioClip> EnemyMove;
    
    [Header("Music")]
    public AudioClip BackgroundMusic;
}

public enum Sound
{
    PauseButton,
    GenericUIButton,
    HoverButton,
    FocusTile,
    ReturnCamera,
    Success,
    Fail,
    UI_Hover,
    UI_Subtle,
    UI_Click,
    UI_Sub,
    UI_UpgradeSuccess,
    UI_CameraMove,
    UI_Deny,
    Lightning,
    
    ENEMY_Move,
    ENEMY_Activate
}

public enum Music
{
    Track1
}
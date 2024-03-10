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
    
    
    public List<AudioClip> FriendlySpawn;
    public List<AudioClip> EnemySpawn;
    public List<AudioClip> FriendlyDeath;
    public List<AudioClip> EnemyDeath;
    public List<AudioClip> Spaceship;
    public List<AudioClip> Win;
    public List<AudioClip> Lose;
    
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
    
    GAME_FriendlySpawn,
    GAME_FriendlyDeath,
    GAME_EnemySpawn,
    GAME_EnemyDeath,
    GAME_Spaceship,
    GAME_WIN,
    GAME_LOSE,
    
    
    ENEMY_Move,
    ENEMY_Activate
}

public enum Music
{
    Track1
}
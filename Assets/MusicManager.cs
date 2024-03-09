using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;

public class MusicManager : MonoService
{
    public List<AudioSource> Layers;
    public AudioSource MenuMusic;
    public float TransitionTimes;
    public float MusicVolume = 1f;
    [HideInInspector] public bool IsMusicEnabled = false;
    void Start()
    {
        if (ServiceLocator.GetService<AudioManager>().IsMainMenu)
        {
            PlayMenuMusic();
        }
        else
        {
            PlayGameMusic();
            ServiceLocator.GetService<BoardManager>().OnKingChecked += (c) => { SetDangerLayer(0.5f); };
            ServiceLocator.GetService<BoardManager>().OnNoKingChecked += () => { SetDangerLayer(0f); };
        }
       
    }
    
    void Update()
    {
        
    }
    
    private Tween _menuMusicTween;
    private Tween _layer1Tween;
    private Tween _layer2Tween;
    private Tween _layer3Tween;
    private Tween _layer4Tween;
    private Tween _layer5Tween;
    private Tween _layer6Tween_combat;
    private Tween _layer7Tween_combat_perc;
    private Tween _layer8Tween_danger;

    public void PlayGameMusic()
    {
    
        foreach (var layer in Layers)
        {
            layer.Play();
        }

        EnableBaseLayers();
        IsMusicEnabled = true;
    }

    public void PlayMenuMusic()
    {
        MenuMusic.Play();
        _menuMusicTween?.Kill();
        _menuMusicTween = MenuMusic.DOFade(MusicVolume, 1f);
    }

    public void EnableBaseLayers()
    {
        _layer1Tween?.Kill();
        _layer1Tween = Layers[0].DOFade(MusicVolume, TransitionTimes);
        
        _layer2Tween?.Kill();
        _layer2Tween = Layers[1].DOFade(MusicVolume, TransitionTimes);
        
        _layer3Tween?.Kill();
        _layer3Tween = Layers[2].DOFade(MusicVolume, TransitionTimes);
        
        _layer4Tween?.Kill();
        _layer4Tween = Layers[3].DOFade(MusicVolume, TransitionTimes);
        
        _layer5Tween?.Kill();
        _layer5Tween = Layers[4].DOFade(MusicVolume, TransitionTimes);
    }

    public void SetDangerLayer(float amount)
    {
        _layer8Tween_danger?.Kill();
        _layer8Tween_danger = Layers[7].DOFade(amount, TransitionTimes);
    }

    public void EnableCombatLayers()
    {
        _layer6Tween_combat?.Kill();
        _layer7Tween_combat_perc?.Kill();
        
        _layer6Tween_combat = Layers[5].DOFade(MusicVolume, TransitionTimes);
        _layer7Tween_combat_perc = Layers[6].DOFade(MusicVolume, TransitionTimes);
    }
    
    public void DisableCombatLayers()
    {
        _layer6Tween_combat?.Kill();
        _layer7Tween_combat_perc?.Kill();
        
        _layer6Tween_combat = Layers[5].DOFade(0f, TransitionTimes);
        _layer7Tween_combat_perc = Layers[6].DOFade(0f, TransitionTimes);
    }
}

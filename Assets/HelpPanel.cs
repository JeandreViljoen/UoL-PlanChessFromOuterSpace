using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

public class HelpPanel : MonoBehaviour
{
    private EasyService<AudioManager> _audioManager;
    
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ToggleMenu()
    {
        _audioManager.Value.PlaySound(Sound.GenericUIButton);
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
}

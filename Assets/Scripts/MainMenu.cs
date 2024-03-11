using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private EasyService<AudioManager> _audioManager;

    public void OnOpenGame(){
        _audioManager.Value.PlaySound(Sound.GenericUIButton);
        SceneManager.LoadSceneAsync(1); //Loads scene 2 Test Board Manager
        Debug.Log("Loads Game with scene 1"); 
    }

    public void OnPauseGame(){
        if (Time.timeScale == 0f) // If the game is already paused, resume itv
        {
            Time.timeScale = 1f;
            Debug.Log("Resumes the game");
        }
        else // If the game is not paused, pause it
        {
            Time.timeScale = 0f;
            Debug.Log("Pauses the game");
        }
    }

    public void OnShop(){
        Debug.Log("Opens Shop"); // Opens shop
    }

    public void OnCloseGame(){
        _audioManager.Value.PlaySound(Sound.GenericUIButton);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void HoverButton()
    {
        _audioManager.Value.PlaySound(Sound.UI_Hover);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void OnOpenGame(){
        SceneManager.LoadSceneAsync(2); //Loads scene 2 Test Board Manager
        Debug.Log("Loads Game with scene 2"); 
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
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button StartButton;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnStartClicked()
    {
        //logic here
        return;
    }

    public void PauseGame()
    {
        //run pause logic.
        if (Time.timeScale == 0f) // If the game is already paused, resume it
        {
            Time.timeScale = 1f;
            Debug.Log("Resumes the game");
        }
        else // If the game is not paused, pause it
        {
            
            Time.timeScale = 0f;
            Debug.Log("Pauses the game");
        }
        Debug.Log("Pauses the game"); //Game pauses
    }

}

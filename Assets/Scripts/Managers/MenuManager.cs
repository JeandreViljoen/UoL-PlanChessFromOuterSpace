using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoService
{
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // On Hitting the escape key the MainMenu shows up
        {
            GoToMainMenu();
        }
    }
    private void GoToMainMenu() // Loads the MainMenu
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}

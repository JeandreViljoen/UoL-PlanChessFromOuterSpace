using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoService
{
    public PauseMenu PauseMenu;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // On Hitting the escape key the MainMenu shows up
        {
            GoToMainMenu();
        }
    }
    private void GoToMainMenu() // Loads the MainMenu
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
    }
}

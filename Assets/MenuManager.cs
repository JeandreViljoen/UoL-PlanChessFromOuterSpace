using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoService
{
    public void OnPlayGame()
    {
        SceneManager.LoadSceneAsync(1); 
        Debug.Log("Loads Level 1"); //Loads Level 1
    }
    public void OnShop()
    {
        Debug.Log("Shop Menu Will Open"); // Shop Menu will open
    }


    public void OnCloseGame()
    {
        Debug.Log("Quits The Game"); //Quits the game
    }
}

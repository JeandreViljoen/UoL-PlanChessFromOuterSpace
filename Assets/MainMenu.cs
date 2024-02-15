using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayGame()
    {
        SceneManager.LoadSceneAsync(1); 
        Debug.Log("Loads Level 1"); //Loads Level 1
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void OnPlayGame()
    {
        SceneManager.LoadSceneAsync(1); //Loads Level 1
    }
}

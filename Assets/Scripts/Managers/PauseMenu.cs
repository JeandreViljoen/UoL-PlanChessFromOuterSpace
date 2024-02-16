using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] RectTransform pausemenupanel;
    [SerializeField] float posTopY, posMidY; 
    [SerializeField] float tweentime;
    // Start is called before the first frame update
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        PauseMenuIn();
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void Resume()
    {
        PauseMenuOut();
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void PauseMenuIn()
    {
        pausemenupanel.DOAnchorPosY(posMidY, tweentime);
    }

    public void PauseMenuOut()
    {
        pausemenupanel.DOAnchorPosY(posTopY, tweentime);
    }
}

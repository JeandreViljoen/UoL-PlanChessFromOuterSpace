using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Services;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _pauseMenuPanel;
    [SerializeField] private Vector3 _pauseMenuPanelRelativeHideOffset;
    private Vector3 _showPosition, _hidePosition;
    [SerializeField] private float _tweenTime;

    private Tween _tweenPauseMenuMove;

    private void Awake()
    {
        _showPosition = _pauseMenuPanel.transform.localPosition;
        _hidePosition = _showPosition + _pauseMenuPanelRelativeHideOffset;
    }

    private void Start()
    {
        _pauseMenuPanel.transform.localPosition = _hidePosition;
        gameObject.SetActive(false);
    }

    public void Pause()
    {
        gameObject.SetActive(true);
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
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void PauseMenuIn()
    {
        _tweenPauseMenuMove?.Kill();
        _tweenPauseMenuMove = _pauseMenuPanel.gameObject.transform.DOLocalMove(_showPosition, _tweenTime).SetUpdate(true);
    }

    public void PauseMenuOut()
    {
        _tweenPauseMenuMove?.Kill();
        _tweenPauseMenuMove = _pauseMenuPanel.gameObject.transform.DOLocalMove(_hidePosition, _tweenTime).SetUpdate(true);
    }

    public void PlayUIButtonSound()
    {
        ServiceLocator.GetService<AudioManager>().PlaySound(Sound.GenericUIButton);
    }
    
    public void PlayPauseSound()
    {
        ServiceLocator.GetService<AudioManager>().PlaySound(Sound.PauseButton);
    }
}

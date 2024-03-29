﻿using System;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ServiceLocator.GetService<MenuManager>().IsPaused)
            {
                Resume();
            }
        }
    }

    public void Pause()
    {
        ServiceLocator.GetService<MenuManager>().IsPaused = true;
        gameObject.SetActive(true);
        Time.timeScale = 0;
        AudioListener.pause = true;
        PauseMenuIn();
    }
    public void Home()
    {
        Resume();
        SceneManager.LoadScene(0);
    }

    public void Resume()
    {
        ServiceLocator.GetService<MenuManager>().IsPaused = false;
        AudioListener.pause = false;
        PauseMenuOut();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Resume();
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
        ServiceLocator.GetService<AudioManager>().PlaySound(Sound.UI_Click);
    }
}

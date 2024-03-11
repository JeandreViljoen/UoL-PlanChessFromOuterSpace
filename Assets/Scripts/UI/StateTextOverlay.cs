using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class StateTextOverlay : MonoBehaviour
{
    private TextMeshProUGUI _textField;
    private EasyService<GameStateManager> _gameStateManager;

    void Start()
    {
        _textField = GetComponent<TextMeshProUGUI>();
        gameObject.SetActive(false);
        
        //Check debug flag whether to enable or not.
        if (GlobalDebug.Instance.ShowGameState)
        {
            //Subscribe to state change event and update the text if the state changes
            _gameStateManager.Value.OnStateChanged += UpdateState;
            UpdateState(_gameStateManager.Value.GameState);
        }
    }

    private void UpdateState(GameState state)
    {
        gameObject.SetActive(true);
        _textField.text = "STATE: <color=#AAFFAAAA>" + state.ToString() + "</color>";
    }

    private void OnDestroy()
    {
        if (GlobalDebug.Instance.ShowGameState)
        {
            _gameStateManager.Value.OnStateChanged -= UpdateState;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class GameStateManager : MonoService
{
    public GameState GameState;

    public event Action<GameState> OnStateChanged;

    void Start()
    {
        SetGameState(GameState.START);
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Sets the state of the game given the GameState enum in parameters. Setting the same game state twice will return early without code execution
    /// </summary>
    /// <param name="newState"></param>
    public void SetGameState(GameState newState)
    {
        if (GameState == newState)
        {
            Debug.LogWarning($"[GameStateManager.cs] - SetGameState() : Attempted to set the game state to {newState} but the GameState is already set to {newState}. Returning early without executing state logic.");
            return;
        }
        
        switch (newState)
        {
            case GameState.START:
                break;
            case GameState.PREP:
                break;
            case GameState.COMBAT:
                break;
            case GameState.WIN:
                break;
            case GameState.LOSE:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        
        OnStateChanged?.Invoke(newState);
    }
}

public enum GameState
{
    START,
    PREP,
    COMBAT,
    WIN,
    LOSE
}
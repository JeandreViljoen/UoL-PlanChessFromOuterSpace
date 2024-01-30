using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class GameStateManager : MonoService
{

    private GameState _gameState;



    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            if (_gameState == value)
            {
                Debug.LogWarning($"[GameStateManager.cs] - : Attempted to set the game state to {value} but the GameState is already set to {value}. Returning early without executing state logic.");
                return;
            }
            
            _gameState = value;
            
            switch (_gameState)
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
                    throw new ArgumentOutOfRangeException(nameof(_gameState), _gameState, null);
            }

            OnStateChanged?.Invoke(_gameState);
        }
    }

    public event Action<GameState> OnStateChanged;

    void Start()
    {
       
    }

    void Update()
    {
        
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
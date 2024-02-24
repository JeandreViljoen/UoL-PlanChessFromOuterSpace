﻿using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class GameStateManager : MonoService
{

    private GameState _gameState;
    private float _stateChangeTime;



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
                case GameState.SPAWN:
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
            _stateChangeTime = Time.time;
        }
    }

    public event Action<GameState> OnStateChanged;

    void Start()
    {
        GameState = GameState.PREP;
    }

    void Update()
    {
        
    }

    public float GetTimeSinceStateChange()
    {
        return Time.time - _stateChangeTime;
    }

}



public enum GameState
{
    START,
    SPAWN,
    PREP,
    COMBAT,
    WIN,
    LOSE
}

/*
TODO:
Set up data objects for different chess pieces and figure out movement mechanics + range upgrades
















*/
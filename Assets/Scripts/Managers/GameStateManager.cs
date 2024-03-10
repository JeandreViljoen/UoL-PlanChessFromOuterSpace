using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class GameStateManager : MonoService
{

    private GameState _gameState;
    private float _stateChangeTime;
    private int _turnCount = 0;
    private EasyService<TransitionController> _transitionController;
    private EasyService<ExecutionOrderManager> _executionOrderManager;
    private EasyService<EnemySpawner> _enemySpawner;
    private EasyService<ScoreManager> _scoreManager;
    private EasyService<HUDManager> _hudManager;
    private EasyService<CurrencyManager> _currencyManager;
    private EasyService<BoardManager> _boardManager;

    public bool HasPlacedKing = false;
    public int TurnsToWin = 20;

    public ChessPiece KingReference;

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

            if (_gameState != GameState.TRANSITION)
            {
                _gameState = GameState.TRANSITION;
            }
            else
            {
                _gameState = value;
            }
            
            _hudManager.Value.LosePrompt.SetActive(false);
            _hudManager.Value.StatsPanel.SetActive(false);
            _hudManager.Value.DimPanel.gameObject.SetActive(false);
            _hudManager.Value.WinPrompt.SetActive(false);

            switch (_gameState)
            {
                case GameState.TRANSITION:
                    _transitionController.Value.Transition(value);
                    break;
                case GameState.START:
                    break;
                case GameState.SPAWN:
                    _turnCount++;
                    _hudManager.Value.SetTurnCounter(_turnCount);
                    _enemySpawner.Value.ExecuteSpawning();
                    break;
                case GameState.PREP:
                    
                    
                    _currencyManager.Value.AddCurrency(GlobalGameAssets.Instance.CurrencyBalanceData.CurrencyEarnedPerRound); ;
                    _boardManager.Value.CheckIfKingIsInCheck();
                    break;
                case GameState.COMBAT:
                    _scoreManager.Value.UpdateStats();
                    break;
                case GameState.WIN:
                    _scoreManager.Value.UpdateStats();
                    _hudManager.Value.WinPrompt.SetActive(true);
                    _hudManager.Value.DimPanel.gameObject.SetActive(true);
                    _hudManager.Value.StatsPanel.SetActive(true);
                    break;
                case GameState.LOSE:
                    _scoreManager.Value.UpdateStats();
                    _hudManager.Value.DimPanel.gameObject.SetActive(true);
                    _hudManager.Value.LosePrompt.SetActive(true);
                    _hudManager.Value.StatsPanel.SetActive(true);
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
        GameState = GameState.SPAWN;
        //_executionOrderManager.Value.OnTimeLineInit += IncreaseTurnCount;
    }

    void Update()
    {
        
    }

    public float GetTimeSinceStateChange()
    {
        return Time.time - _stateChangeTime;
    }
    
    public int GetTurnNumber()
    {
        return _turnCount;
    }

    private void IncreaseTurnCount()
    {
        _turnCount++;
    }
}



public enum GameState
{
    START,
    SPAWN,
    PREP,
    COMBAT,
    WIN,
    LOSE,
    TRANSITION
}

/*
TODO:
Set up data objects for different chess pieces and figure out movement mechanics + range upgrades
















*/
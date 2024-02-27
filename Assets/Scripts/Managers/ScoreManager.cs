using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

public class ScoreManager : MonoService
{

    // Services
    private EasyService<CurrencyManager> _currencyManager;
    private EasyService<GameStateManager> _gameStateManager;

    // Score Data
    private Dictionary<ChessPieceType, int> _enemiesDefeated;
    private int _unitLostCount;
    private int _currencyEarned;
    private int _turnsLasted;
    
    // Time register
    private float _gameStartTime;
    private float _gameEndTime;
    private int _secondsPlayed;

    void Start()
    {
        BuildSubscriptions();
        BuildEnemiesDictionary();
        SetInitialValues();
    }

   
    void Update()
    {
        
    }

    
    // Public Methods
    public void AddEnemyDestroyedToScore(ChessPieceType pieceType)
    {
        _enemiesDefeated[pieceType] += 1;
    }

    public void AlliedPieceDestroyed()
    {
        _unitLostCount++;
    }
    
    public string GetTimePlayedString()
    {
        int hours = (int)(_secondsPlayed / 3600);
        int minutes = (int)(_secondsPlayed % 3600 / 60);
        int seconds = (int)(_secondsPlayed % 3600 % 60);

        if (hours > 0)
        {
            return $"You played a total of {hours} hours, {minutes} minutes and {seconds} seconds!";
        }
        else if (minutes > 0)
        {
            return $"You played a total of {minutes} minutes and {seconds} seconds!";
        }
        else
        {
            return $"Your game lasted only {seconds} seconds!";
        }
    }

    public Dictionary<ChessPieceType, int> GetPiecesDestroyed()
    {
        return _enemiesDefeated;
    }

    public int GetCurrencyEarned()
    {
        return _currencyEarned;
    }

    public int GetNumberOfTurns()
    {
        return _turnsLasted;
    }

    public int GetUnitLostCount()
    {
        return _unitLostCount;
    }

    // Must be called on game end in order to calculate the duration of the match
    public void UpdateStats()
    {
        // Save amount of seconds played
        _gameEndTime = Time.time;
        _secondsPlayed = (int)(_gameEndTime - _gameStartTime);
        _turnsLasted = _gameStateManager.Value.GetTurnNumber();
    }
    
    // Subscriptions
    private void BuildSubscriptions()
    {
        _currencyManager.Value.OnCurrencyAdded += AddCurrencyToScore;
    }

    private void RemoveSubscriptions()
    {
        _currencyManager.Value.OnCurrencyAdded -= AddCurrencyToScore;
    }

    private void AddCurrencyToScore(int amount)
    {
        _currencyEarned += amount;
    }
    
    // Creates data for the enemies defeated score variable
    private void BuildEnemiesDictionary()
    {
        _enemiesDefeated = new Dictionary<ChessPieceType, int>();
        _enemiesDefeated.Add(ChessPieceType.Rook, 0);
        _enemiesDefeated.Add(ChessPieceType.Bishop, 0);
        _enemiesDefeated.Add(ChessPieceType.Knight, 0);
        _enemiesDefeated.Add(ChessPieceType.King, 0);
        _enemiesDefeated.Add(ChessPieceType.Queen, 0);
        _enemiesDefeated.Add(ChessPieceType.Pawn, 0);
    }

    private void SetInitialValues()
    {
        _currencyEarned = 0;
        _gameStartTime = Time.time;
        _secondsPlayed = 0;
        _turnsLasted = 1;
    }

    private void DebugCurrentScore()
    {
        Debug.Log("Printing current score...");
        foreach (var enemyData in _enemiesDefeated)
        {
            Debug.Log($"Enemies of type {enemyData.Key} defeated: {enemyData.Value}");
        }
    }

    private void OnDestroy()
    {
        // Remove subscriptions
        RemoveSubscriptions();
    }
}

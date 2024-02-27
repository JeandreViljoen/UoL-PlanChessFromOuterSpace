﻿using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{

    private EasyService<ScoreManager> _scoreManager;

    public TextMeshProUGUI PawnPiecesDestroyed;
    public TextMeshProUGUI RookPiecesDestroyed;
    public TextMeshProUGUI BishopPiecesDestroyed;
    public TextMeshProUGUI KnightPiecesDestroyed;
    public TextMeshProUGUI KingPiecesDestroyed;
    public TextMeshProUGUI QueenPiecesDestroyed;
    public TextMeshProUGUI CurrencyText;
    public TextMeshProUGUI TimePlayedText;
    public TextMeshProUGUI TurnsLastedText;
    public TextMeshProUGUI UnitsLostText;
    
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (!_scoreManager.IsAssigned)
        {
            return;
        }
        UpdateStats();
    }
    
    

    public void UpdateStats()
    {
        _scoreManager.Value.UpdateStats();
        Dictionary<ChessPieceType, int> _piecesDestroyed = _scoreManager.Value.GetPiecesDestroyed();

        PawnPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Pawn]}x";
        RookPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Rook]}x";
        BishopPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Bishop]}x";
        KnightPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Knight]}x";
        KingPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.King]}x";
        QueenPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Queen]}x";
        CurrencyText.text = $"You have earned {_scoreManager.Value.GetCurrencyEarned()} currency";
        TimePlayedText.text = $"{_scoreManager.Value.GetTimePlayedString()}";
        TurnsLastedText.text = $"You have been playing for {_scoreManager.Value.GetNumberOfTurns()} turns";
        UnitsLostText.text = $"{_scoreManager.Value.GetUnitLostCount()} of your units have fallen in combat";
    }
    
    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{

    //private EasyService<ScoreManager> _scoreManager;

    public TextMeshProUGUI PawnPiecesDestroyed;
    public TextMeshProUGUI RookPiecesDestroyed;
    public TextMeshProUGUI BishopPiecesDestroyed;
    public TextMeshProUGUI KnightPiecesDestroyed;
    public TextMeshProUGUI KingPiecesDestroyed;
    public TextMeshProUGUI QueenPiecesDestroyed;
    
    public Image PawnIcon;
    public Image RookIcon;
    public Image BishopIcon;
    public Image KnightIcon;
    public Image KingIcon;
    public Image QueenIcon;
    
    public TextMeshProUGUI CurrencyText;
    public TextMeshProUGUI TimePlayedText;
    public TextMeshProUGUI TurnsLastedText;
    public TextMeshProUGUI UnitsLostText;
    
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        PawnIcon.sprite = GlobalGameAssets.Instance.PawnData.EnemySprite;
        RookIcon.sprite = GlobalGameAssets.Instance.RookData.EnemySprite;
        BishopIcon.sprite = GlobalGameAssets.Instance.BishopData.EnemySprite;
        KnightIcon.sprite = GlobalGameAssets.Instance.KnightData.EnemySprite;
        KingIcon.sprite = GlobalGameAssets.Instance.KingData.EnemySprite;
        QueenIcon.sprite = GlobalGameAssets.Instance.QueenData.EnemySprite;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        UpdateStats();
    }
    
    

    public void UpdateStats()
    {

        ScoreManager _scoreManager = ServiceLocator.GetService<ScoreManager>();
        if (_scoreManager == null)
        {
            return;
        }
        _scoreManager.UpdateStats();
        Dictionary<ChessPieceType, int> _piecesDestroyed = _scoreManager.GetPiecesDestroyed();

        PawnPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Pawn]}x";
        RookPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Rook]}x";
        BishopPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Bishop]}x";
        KnightPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Knight]}x";
        KingPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.King]}x";
        QueenPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Queen]}x";
        CurrencyText.text = $"You have earned {_scoreManager.GetCurrencyEarned()} currency";
        TimePlayedText.text = $"{_scoreManager.GetTimePlayedString()}";
        TurnsLastedText.text = $"You have been playing for {_scoreManager.GetNumberOfTurns()} turns";
        UnitsLostText.text = $"{_scoreManager.GetUnitLostCount()} of your units have fallen in combat";
    }
    
    public void CloseUI()
    {
        gameObject.SetActive(false);
    }
}

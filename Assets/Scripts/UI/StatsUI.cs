using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{

    private EasyService<ScoreManager> _scoreManager;

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
    
    // Statistics
    private Dictionary<ChessPieceType, int> _piecesDestroyed = new Dictionary<ChessPieceType, int>();
    
    // Start is called before the first frame update
    void Start()
    {
        PawnIcon.sprite = GlobalGameAssets.Instance.PawnData.EnemyPortrait;
        RookIcon.sprite = GlobalGameAssets.Instance.RookData.EnemyPortrait;
        BishopIcon.sprite = GlobalGameAssets.Instance.BishopData.EnemyPortrait;
        KnightIcon.sprite = GlobalGameAssets.Instance.KnightData.EnemyPortrait;
        KingIcon.sprite = GlobalGameAssets.Instance.KingData.EnemyPortrait;
        QueenIcon.sprite = GlobalGameAssets.Instance.QueenData.EnemyPortrait;
       
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
        if (_scoreManager)
        {
          _scoreManager.UpdateStats();
                  Dictionary<ChessPieceType, int> _piecesDestroyed = _scoreManager.GetPiecesDestroyed();
          
                  PawnPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Pawn]}x";
                  RookPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Rook]}x";
                  BishopPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Bishop]}x";
                  KnightPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Knight]}x";
                  KingPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.King]}x";
                  QueenPiecesDestroyed.text = $"{_piecesDestroyed[ChessPieceType.Queen]}x";
                  CurrencyText.text = $"You have earned <color=#ff2222ff>{_scoreManager.GetCurrencyEarned()}</color> currency";
                  TimePlayedText.text = $"{_scoreManager.GetTimePlayedString()}";
                  TurnsLastedText.text = $"You have been playing for <color=#ff2222ff>{_scoreManager.GetNumberOfTurns()}</color> turns";
                  UnitsLostText.text = $"<color=#ff2222ff>{_scoreManager.GetUnitLostCount()}</color> of your units have fallen in combat";  
        }
    }
    
    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    public void ToggleUI()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}

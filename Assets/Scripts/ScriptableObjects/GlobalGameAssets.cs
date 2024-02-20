using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Global Game Asset", menuName = "Custom Assets/New Global Game Asset")]
public class GlobalGameAssets : ScriptableObjectSingleton<GlobalGameAssets>
{
   [Header("Highlight Colors")]
   public Color HighlightColor;
   
   
   [Header("Currency Balance Data")] 
   public CurrencyBalanceData CurrencyBalanceData;
   

   [Header("Chess Piece Data")] 
   public ChessPieceData PawnData;
   public GameObject PawnPrefab;
   public ChessPieceData RookData;
   public GameObject RookPrefab;
   public ChessPieceData BishopData;
   public GameObject BishopPrefab;
   public ChessPieceData KnightData;
   public GameObject KnightPrefab;
   public ChessPieceData QueenData;
   public GameObject QueenPrefab;
   public ChessPieceData KingData;
   public GameObject KingPrefab;
   
   

   public float ChessPieceAnimateSpeed;

}

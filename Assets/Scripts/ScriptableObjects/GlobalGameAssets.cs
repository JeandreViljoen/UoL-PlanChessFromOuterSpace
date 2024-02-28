using System;
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

   [Header("AI Balance Data")] 
   public AIBalanceData AIBalanceData;
   
   [Header("Audio Data")] 
   public AudioData AudioData;


   public GameObject FloatingCurrencyPrefab;
   public GameObject FloatingCurrencyUIPrefab;

   public float ChessPieceAnimateSpeed;

   public ChessPieceData GetChessPieceData(ChessPieceType type)
   {
      switch (type)
      {
         case ChessPieceType.Pawn:
            return PawnData;
          
         case ChessPieceType.Knight:
            return KnightData;
           
         case ChessPieceType.Bishop:
            return BishopData;
           
         case ChessPieceType.Rook:
            return RookData;
          
         case ChessPieceType.Queen:
            return QueenData;
           
         case ChessPieceType.King:
            return KingData;
          
         default:
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
   }
   
   public GameObject GetChessPiecePrefab(ChessPieceType type)
   {
      switch (type)
      {
         case ChessPieceType.Pawn:
            return PawnPrefab;
          
         case ChessPieceType.Knight:
            return KnightPrefab;
           
         case ChessPieceType.Bishop:
            return BishopPrefab;
           
         case ChessPieceType.Rook:
            return RookPrefab;
          
         case ChessPieceType.Queen:
            return QueenPrefab;
           
         case ChessPieceType.King:
            return KingPrefab;
          
         default:
            throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
   }

}

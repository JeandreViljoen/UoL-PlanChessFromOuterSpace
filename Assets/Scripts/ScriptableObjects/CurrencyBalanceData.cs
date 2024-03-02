using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CurrencyBalanceData", menuName = "Custom Assets/New Currency Balance Data")]
public class CurrencyBalanceData : ScriptableObject
{
    [Header("SETTINGS")]

    public int StartCurrency;
    public int CurrencyEarnedPerRound;
    public int UpgradeCostIncrease;
    public int AdditionalCurrencyRewardPerLevel;
    
    [Header("REWARDS")] 
    public int QueenReward;
    public int RookReward;
    public int KnightReward;
    public int BishopReward;
    public int PawnReward;
    public int KingReward;
    

    [Header("UNIT COSTS")] 
    public int QueenCost;
    public int RookCost;
    public int KnightCost;
    public int BishopCost;
    public int PawnCost;
    public int KingCost;

    [Header("UPGRADE COSTS")]
    public int UpgradeSpeedCost;
    public int UpgradeRangeCost;
    public int UpgradeSpecialCost;
        
        
    [Header("POWERUP COSTS")]
    public int PowerUp1Cost;
    public int PowerUp2Cost;
    public int PowerUp3Cost;
    
    
    public int GetChessPieceCost(ChessPieceType type)
    {
        switch (type)
        {
            case ChessPieceType.Pawn:
                return PawnCost;
          
            case ChessPieceType.Knight:
                return KnightCost;
           
            case ChessPieceType.Bishop:
                return BishopCost;
           
            case ChessPieceType.Rook:
                return RookCost;
          
            case ChessPieceType.Queen:
                return QueenCost;
           
            case ChessPieceType.King:
                return KingCost;
          
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
   
    
    
    
}

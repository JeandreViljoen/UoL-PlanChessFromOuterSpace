using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class CurrencyManager : MonoService
{
    public int Currency { get; private set; }

    public event Action<int> OnCurrencyAdded; 
    public event Action<int> OnCurrencyRemoved;

    private void Awake()
    {
        Currency = GlobalGameAssets.Instance.CurrencyBalanceData.StartCurrency;
    }

    void Start()
     {
         
     }

    private void Update()
    {
        if (GlobalDebug.Instance.AllowCheats)
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
                AddCurrency(100);
            }
        }
    }

    /// <summary>
    /// Adds currency.
    /// </summary>
    /// <param name="amount">Amount to add (Cannot be negative)</param>
    public void AddCurrency(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"[CurrencyManager.cs] - AddCurrency() : Cannot ADD negative currency, use TryRemoveCurrency() instead.");
            return;
        }
        if (amount == 0)
        {
            Debug.LogWarning($"[CurrencyManager.cs] - AddCurrency() : Tried to add 0 currency");
            return;
        }

        InitFloatingCurrency(amount);
        Currency += amount;
        OnCurrencyAdded?.Invoke(amount);
    }
    
    /// <summary>
    /// Removes currency if there is enough available. 
    /// </summary>
    /// <param name="amount">amount to remove (Cannot be negative)</param>
    /// <returns>true if currency successfully removed</returns>
    public bool TryRemoveCurrency(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"[CurrencyManager.cs] - TryRemoveCurrency() : Cannot REMOVE negative currency, use AddCurrency() instead.");
            return false;
        }
        if (!HasCurrency(amount))
        {
            Debug.LogWarning($"[CurrencyManager.cs] - TryRemoveCurrency() : Not enough available currency to remove. Returning early");
            return false;
        }
        if (amount == 0)
        {
            Debug.LogWarning($"[CurrencyManager.cs] - TryRemoveCurrency() : Tried to remove 0 currency");
            return false;
        }

        Currency -= amount;
        InitFloatingCurrency(amount*-1);
        OnCurrencyRemoved?.Invoke(amount);
        return true;
    }

    private void InitFloatingCurrency(int value)
    {
        float xOffset = 60.0f;
        FloatingCurrency f = Instantiate(GlobalGameAssets.Instance.FloatingCurrencyUIPrefab.GetComponent<FloatingCurrency>(),  ServiceLocator.GetService<HUDManager>().CurrencyDisplay.transform);
        f.transform.position = ServiceLocator.GetService<HUDManager>().CurrencyDisplay.transform.position + Vector3.right * xOffset;
        f.InitUI(value);
    }

    /// <summary>
    /// Checks if the player has has enough currency given in parameters
    /// </summary>
    /// <param name="amount">Amount to check (Cannot be negative)</param>
    /// <returns>true if has enough currency, otherwise false.</returns>
    public bool HasCurrency(int amount)
    {
        if (amount < 0)
        {
            Debug.LogError($"[CurrencyManager.cs] - HasCurrency() : Cannot Check for negative currency. Returning false");
            return false;
        }
        
        if (amount <= Currency)
        {
            return true;
        }

        return false;
    }

    public int RequestCaptureReward(ChessPiece piece)
    {
        int baseReward = 0;
        int additionalReward = 0;
        
        switch (piece.PieceType)
        {
            case ChessPieceType.Pawn:
                baseReward = GlobalGameAssets.Instance.CurrencyBalanceData.PawnReward;
                break;
            case ChessPieceType.Knight:
                baseReward = GlobalGameAssets.Instance.CurrencyBalanceData.KnightReward;
                break;
            case ChessPieceType.Bishop:
                baseReward = GlobalGameAssets.Instance.CurrencyBalanceData.BishopReward;
                break;
            case ChessPieceType.Rook:
                baseReward = GlobalGameAssets.Instance.CurrencyBalanceData.RookReward;
                break;
            case ChessPieceType.Queen:
                baseReward = GlobalGameAssets.Instance.CurrencyBalanceData.QueenReward;
                break;
            case ChessPieceType.King:
                baseReward = GlobalGameAssets.Instance.CurrencyBalanceData.KingReward;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        additionalReward = (piece.Level-1) * GlobalGameAssets.Instance.CurrencyBalanceData.AdditionalCurrencyRewardPerLevel;
        
        AddCurrency(baseReward + additionalReward);
        return baseReward + additionalReward;
        if (GlobalDebug.Instance.ShowCombatMessageLogs) Debug.Log($"\t\t{ChessPiece.ToString(piece)} CAPTURED! - Rewarding currency:\n\t\tBase: {baseReward}\t\tAdditional: {additionalReward}");
    }
}

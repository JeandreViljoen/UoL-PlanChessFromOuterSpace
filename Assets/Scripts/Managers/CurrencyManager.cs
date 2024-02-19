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
        }

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
        }

        Currency -= amount;
        OnCurrencyRemoved?.Invoke(amount);
        return true;
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
}

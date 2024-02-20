using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIBalanceData", menuName = "Custom Assets/New AI Balance Data")]
public class AIBalanceData : ScriptableObject
{
    [Header("Unit Target Values")] 
    
    public int KingValue;
    public int QueenValue;
    public int RookValue;
    public int KnightValue;
    public int BishopValue;
    public int PawnValue;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum containing all the possible chess pieces
public enum ChessPieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public class ChessPiece : MonoBehaviour
{
    // --------------- Member variables and data --------------- //
    public ChessPieceType PieceType;
    public int DefaultSpeed = 1;
    public int Level = 1;
    public int DefaultRange = 2;
    public List<Vector2> RelativeMoveset; // [ (0,1) , (1,0) , (-1,0), (0,-1) ]

    private string _indexCodePosition;

    public string IndexCodePosition
    {
        get
        {
            return _indexCodePosition;
        }
        set
        {
            if (value.Length != 2)
            {
                Debug.LogError("[ChessPiece.cs] - Tried to set IndexCodePosition but code was not 2 characters (E.g. F7). returning early");
                return;
            }
            _indexCodePosition = value;
        }
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void MoveToBlock()
    {
        
    }
}

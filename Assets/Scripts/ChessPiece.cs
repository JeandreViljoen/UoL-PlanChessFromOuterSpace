using System;using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

public enum Team
{
    Friendly,
    Enemy
}

public enum ChessPieceState
{
    INACTIVE,
    START,
    VALIDATE_BEST_MOVE,
    MOVE,
    ATTACK,
    DEAD,
    END

}

public class ChessPiece : MonoBehaviour
{
    // --------------- Member variables and data --------------- //
    public ChessPieceType PieceType;
    public int DefaultSpeed = 1;
    public int Speed = 1;
    public int Level = 1;
    public int DefaultRange = 2;
    public List<Vector2> RelativeMoveset; // [ (0,1) , (1,0) , (-1,0), (0,-1) ]
    public float AnimateSpeed = 1;
    public Team Team;

    private IndexCode _indexCodePosition;

    public IndexCode IndexCodePosition
    {
        get
        {
            return _indexCodePosition;
        }
        set
        {
            _indexCodePosition = value;
        }
    }

    public event Action OnStateEnd; 

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    /// <summary>
    /// Moves Piece to given BoardSquare position in parameters
    /// </summary>
    /// <param name="BoardSquare"></param>
    public void MoveToBlock(BoardSquare square)
    {
        transform.DOMove(square.CenterSurfaceTransform.position, AnimateSpeed).SetEase(Ease.InOutSine);
        IndexCodePosition = square.IndexCode;
    }

    public void RunStateLogic(ChessPieceState state)
    {
        switch (state)
        {
            case ChessPieceState.INACTIVE:
                break;
            case ChessPieceState.START:
                break;
            case ChessPieceState.VALIDATE_BEST_MOVE:
                break;
            case ChessPieceState.MOVE:
                break;
            case ChessPieceState.ATTACK:
                break;
            case ChessPieceState.DEAD:
                break;
            case ChessPieceState.END:
                OnStateEnd?.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}

using System;
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

public abstract class ChessPiece : MonoBehaviour
{
    // --------------- Member variables and data --------------- //
    public int DefaultSpeed = 1;
    public int Speed = 1;
    public int Level = 1;
    public int DefaultRange = 2;
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
    public void MoveTo(BoardSquare square)
    {
        transform.DOMove(square.CenterSurfaceTransform.position, AnimateSpeed).SetEase(Ease.InOutSine);
        IndexCodePosition = square.IndexCode;
    }

    /// <summary>
    /// Get a set of positions the piece can move to.
    /// </summary>
    /// <param name="board">The chess board.</param>
    /// <param name="pos">The position of this piece.</param>
    /// <returns>A set of positions this piece can move to.</returns>
    public abstract IEnumerable<(int, int)> GetMoves(BoardManager board, (int x, int y) pos);

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

    /// <summary>
    /// Convert a player-view-relative offset to absolute offset.
    /// </summary>
    /// <param name="pos">
    /// The offset relative to the player's view; negative Y is forward.
    /// </param>
    /// <returns>The absolute offset.</returns>
    protected (int, int) ViewOffsetToAbsolute((int x, int y) pos)
    {
        if (Team == Team.Enemy) {
            return (pos.x, -pos.y);
        }

        return pos;
    }
}

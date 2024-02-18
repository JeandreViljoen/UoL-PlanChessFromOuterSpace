using System;using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
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
    public SpriteRenderer Sprite;
    private ChessPieceData _data;
    public int Speed;
    public int Level;
    public BoardSquare AssignedSquare;

    private int _range;

   public int Range
    {
        get
        {
            return _range;
        }
        set
        {
            _range = value;
            UpdateMoveset();
        }
    }
    public List<Vector2> RelativeMoveset;
    public List<Vector2> BaseRelativeMoveset;

    private float _animateSpeed;
    public Team Team;

    // private Vector2 _position;
    // private IndexCode _indexCodePosition;
    //
    // public IndexCode IndexCodePosition
    // {
    //     get
    //     {
    //         return _indexCodePosition;
    //     }
    //     set
    //     {
    //         _indexCodePosition = value;
    //     }
    // }

    public event Action OnStateEnd;

    private void Init()
    {
        _data = GetStartData(PieceType);

        Sprite.sprite = _data.Sprite;
        Speed = _data.DefaultSpeed;
        BaseRelativeMoveset = _data.BaseRelativeMoveset;
        Range = _data.DefaultRange; // Has to be retrieved after BaseRelativeMoveSet as it updates the moveset on this set method
        
        HighlightTiles(GetAbsoluteMovesetTiles());
    }

    private void UpdateMoveset()
    {
        List<Vector2> newMoves = new List<Vector2>();
        newMoves.Add(new Vector2(0,0));
        
        if (PieceType == ChessPieceType.King)
        {
            //
        }
        else
        {
            foreach (var move in BaseRelativeMoveset)
            {
                for (int i = 1; i <= Range; i++)
                {
                    newMoves.Add(move * i);
                }
            }
        }

        RelativeMoveset = newMoves;
    }

    public List<Vector2> GetAbsoluteMovesetVectors()
    {
        List<Vector2> absoluteMoves = new List<Vector2>();
        
        foreach (var relative in RelativeMoveset)
        {
            Vector2 position = new Vector2(AssignedSquare.IndexX, AssignedSquare.IndexZ);
            Vector2 absolute = relative + position;

            if (absolute.x < 0 || absolute.x > 7 || absolute.y < 0 || absolute.y > 7)
            {
                continue;
            }
            
            absoluteMoves.Add(absolute);
        }

        return absoluteMoves;
    }

    public List<BoardSquare> GetAbsoluteMovesetTiles()
    {
        List<BoardSquare> tiles = new List<BoardSquare>();

        foreach (var index in GetAbsoluteMovesetVectors())
        {
            BoardSquare tile = ServiceLocator.GetService<BoardManager>().GetTile(((int)index.x,(int)index.y));
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }

        return tiles;
    }

    void Start()
    {
        _animateSpeed = GlobalGameAssets.Instance.ChessPieceAnimateSpeed;
        Init();
    }

    public void HighlightTiles(List<BoardSquare> tiles)
    {
        foreach (var tile in tiles)
        {
            tile.Highlight();
        }
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
        
        transform.DOMove(square.CenterSurfaceTransform.position, _animateSpeed).SetEase(Ease.InOutSine);
        //IndexCodePosition = square.IndexCode;
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

    public static ChessPieceData GetStartData(ChessPieceType PieceType)
    {
        ChessPieceData data;
        
        switch (PieceType)
        {
            case ChessPieceType.Pawn:
                data = GlobalGameAssets.Instance.PawnData;
                break;
            case ChessPieceType.Knight:
                data = GlobalGameAssets.Instance.KnightData;
                break;
            case ChessPieceType.Bishop:
                data = GlobalGameAssets.Instance.BishopData;
                break;
            case ChessPieceType.Rook:
                data = GlobalGameAssets.Instance.RookData;
                break;
            case ChessPieceType.Queen:
                data = GlobalGameAssets.Instance.QueenData;
                break;
            case ChessPieceType.King:
                data = GlobalGameAssets.Instance.KingData;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(PieceType), PieceType, null);
        }

        return data;
    }
}

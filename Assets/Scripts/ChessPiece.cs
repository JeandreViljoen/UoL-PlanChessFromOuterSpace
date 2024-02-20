using System;using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
#region Properties
    
    //TEMP FOR TESTING
    public bool IsBuilding = false;
    //--------------------
    
    // --------------- Member variables and data --------------- //
    public ChessPieceType PieceType;
    public SpriteRenderer Sprite;
    private ChessPieceData _data;
    private int _speed;
    public int Speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
            if (_speedIconUIController == null)
            {
                _speedIconUIController = GetComponentInChildren<SpeedIconUIController>();
            }

            if (_speedIconUIController != null)
            {
                _speedIconUIController.Speed = _speed;
            }
            
        }
    }
    public int Level; //TODO ASAP
    public BoardSquare AssignedSquare;

    public float UnitValue;// => Level, type

    private bool _isSelected;
    public bool IsSelected
    {
        get
        {
            return _isSelected;
        }
        set
        {
            _isSelected = value;

            if (_isSelected)
            {
                Selected();
            }
            else
            {
                Deselected();
            }
        }
    }

    private int _range;
    private Coroutine _highlightRoutine;

    private Tween _highlightMoveTween;
    private Vector3 _spritePosition;

    private SpeedIconUIController _speedIconUIController;
    private UpgradeButtonUIController _upgradeButtonUIController;

    private EasyService<CurrencyManager> _currencyManager;

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
            AbsoluteMovesetTiles = GetAbsoluteMovesetTilesDirect();
        }
    }
    public List<Vector2> RelativeMoveset;
    public List<BoardSquare> AbsoluteMovesetTiles;
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
#endregion

    private void OnEnable()
    {
        _upgradeButtonUIController = GetComponentInChildren<UpgradeButtonUIController>();
        if (_upgradeButtonUIController!= null)
        {
            _upgradeButtonUIController.SpeedButton.EventHandler.OnMouseDown += OnSpeedUpgradePressed;
            _upgradeButtonUIController.RangeButton.EventHandler.OnMouseDown += OnRangeUpgradePressed;
        }
       
    }

    //Handles Speed Upgrade Logic
    private void OnSpeedUpgradePressed(PointerEventData _)
    {
        //TODO: Validation checks & limits
        
        if (_currencyManager.Value.TryRemoveCurrency(GlobalGameAssets.Instance.CurrencyBalanceData.UpgradeSpeedCost) )
        {
            Speed++;
        }
        else
        {
            //TODO: SHow feedback for not enough currency
        }
        
    }
    
    //Handles Range Upgrade Logic
    private void OnRangeUpgradePressed(PointerEventData _)
    {
        //TODO: Validation checks & limits
        
        if (_currencyManager.Value.TryRemoveCurrency(GlobalGameAssets.Instance.CurrencyBalanceData.UpgradeRangeCost) )
        {
            Range++;
        }
        else
        {
            //TODO: SHow feedback for not enough currency
        }
        
    }
    
    private void Init()
    {
        //Fetch Data as assigned in inspector
        _data = GetStartData(PieceType);

        if (!IsBuilding)
        {
            Sprite.sprite = _data.Sprite;
        }
        Speed = _data.DefaultSpeed;
        BaseRelativeMoveset = _data.BaseRelativeMoveset;
        Range = _data.DefaultRange; // Has to be retrieved after BaseRelativeMoveSet as it updates the moveset on this set method

    }

    private void RequestSelection()
    {
        ServiceLocator.GetService<BoardManager>().SelectedUnit = this;
    }

    private void UpdateMoveset()
    {
        List<Vector2> newMoves = new List<Vector2>();
        //Add bas tile
        newMoves.Add(new Vector2(0,0));
        
        //TODO: Logic for other pieces.
        
        if (PieceType == ChessPieceType.King)
        {
            //TODO: KING logic 
        }
        // if (PieceType == ChessPieceType.Knight)
        // {
        //     //TODO: Knight logic 
        // }
        else
        {
            //Each direction vector in base moveset (i.e. up, down, diagonal, etc.)
            foreach (var move in BaseRelativeMoveset)
            {
                //Add a tile for the amount of "range"
                for (int i = 1; i <= _range; i++)
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

    /// <summary>
    /// Returns a list of validated BoardSquares where this ChessPiece can move/attack.
    /// </summary>
    /// <returns></returns>
    public List<BoardSquare> GetAbsoluteMovesetTilesDirect()
    {
        List<BoardSquare> tiles = new List<BoardSquare>();

        for (int i = 0; i < RelativeMoveset.Count; i++)
        {
            //Use range to offset indices when a occupied block is detected
            //(So the range does not extend beyond the blocked tiles)
            int rangeOffset = Range - (i % Range);
            if (rangeOffset == Range) rangeOffset = 0;
            
            //Calculate the absolute position
            Vector2 position = new Vector2(AssignedSquare.IndexX, AssignedSquare.IndexZ);
            Vector2 absolute = RelativeMoveset[i] + position;

            //Check if absolute position is in bounds of the board 
            //TODO: NOT HARD CODE
            if (absolute.x >= 0 && absolute.x <= 7 && absolute.y >= 0 && absolute.y <= 7)
            {
                //Fetch tile at absolute position
                BoardSquare tile = ServiceLocator.GetService<BoardManager>().GetTile(((int)absolute.x,(int)absolute.y));
                
                if (tile != null)
                {
                    //Add its own tile
                    if (tile.ChessPieceAssigned == this)
                    {
                        //tiles.Add(tile);
                    }
                    //If tile is empty add tile
                    if (tile.ChessPieceAssigned == null)
                    {
                        tiles.Add(tile);
                    }
                    //If tile is occupied, add it, but skip the rest of the tiles behind the blocked tile.
                    else
                    {
                        tiles.Add(tile);
                        i += rangeOffset;
                    }
                }
            }
        }
        return tiles;
    }

    
    // public List<BoardSquare> GetAbsoluteMovesetTiles()
    // {
    //     List<BoardSquare> tiles = new List<BoardSquare>();
    //
    //     List < Vector2 > movesetVectors = GetAbsoluteMovesetVectors();
    //     
    //     //int baseDirections = BaseRelativeMoveset.Count;
    //
    //     for (int i = 1; i < movesetVectors.Count; i++)
    //     {
    //         int rangeOffset = i % Range;
    //         
    //         BoardSquare tile = ServiceLocator.GetService<BoardManager>().GetTile(((int)movesetVectors[i].x,(int)movesetVectors[i].y));
    //         if (tile != null)
    //         {
    //             if (tile.ChessPieceAssigned == null)
    //             {
    //                 tiles.Add(tile);
    //             }
    //             else
    //             {
    //                 tiles.Add(tile);
    //                 i += rangeOffset;
    //             }
    //           
    //         }
    //     }
    //     return tiles;
    // }

    void Start()
    {
        _animateSpeed = GlobalGameAssets.Instance.ChessPieceAnimateSpeed;
        Init();

        _spritePosition = Sprite.transform.localPosition;
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseEnter += (_) =>
        {
            HighlightTiles(GetAbsoluteMovesetTilesDirect());
        };
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseExit += (_) =>
        {
            UnHighlightTiles(GetAbsoluteMovesetTilesDirect());
        };
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseDown += (_) =>
        {
            RequestSelection();
        };
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseUp += (_) =>
        {
           
        };
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
    
    //Selected State Logic
    private void Selected()
    {
        ServiceLocator.GetService<CameraManager>().FocusTile(this);
        _upgradeButtonUIController.Show();
    }

    //Deselect State Logic
    private void Deselected()
    {
        _upgradeButtonUIController.Hide();
    }

    /// <summary>
    /// Highlight a list of BoardSquare tiles given in parameters
    /// </summary>
    public void HighlightTiles(List<BoardSquare> tiles)
    {
        _highlightMoveTween?.Kill();
        if(_highlightRoutine != null) StopCoroutine(_highlightRoutine);
        _highlightRoutine = StartCoroutine(DelayedHighlight(tiles));
        _highlightMoveTween = Sprite.gameObject.transform.DOLocalMove(_spritePosition + Vector3.up * 0.01f, 0.2f).SetEase(Ease.InOutSine);
    }

    private IEnumerator DelayedHighlight(List<BoardSquare> tiles)
    {
        float totalAnimationTime = 0.2f;
        float incrementTiming = totalAnimationTime / tiles.Count;
        
        foreach (var tile in tiles)
        {
            tile.Highlight();
            yield return new WaitForSeconds(incrementTiming);
        }
    }
    
    /// <summary>
    /// UnHighlight a list of BoardSquare tiles given in parameters
    /// </summary>
    public void UnHighlightTiles(List<BoardSquare> tiles)
    {
        _highlightMoveTween?.Kill();
        _highlightMoveTween = Sprite.gameObject.transform.DOLocalMove(_spritePosition, 0.2f).SetEase(Ease.InOutSine);
        
        if(_highlightRoutine != null) StopCoroutine(_highlightRoutine);
        
        foreach (var tile in tiles)
        {
            tile.UnHighlight();
        }
    }
    
    private void OnDestroy()
    {
        if (_upgradeButtonUIController != null)
        {
            _upgradeButtonUIController.SpeedButton.EventHandler.OnMouseDown -= OnSpeedUpgradePressed;
            _upgradeButtonUIController.RangeButton.EventHandler.OnMouseDown -= OnRangeUpgradePressed;
        }
       
    }
    
}

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
                UpdateLevel();
                ServiceLocator.GetService<ExecutionOrderManager>().RefreshTimelineOrder();
            }
        }
    }

    public int Level { get; private set; }
    public BoardSquare AssignedSquare;

    private float UnitValue;// => Level, type

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
            UpdateLevel();
        }
    }
    public List<Vector2> RelativeMoveset;
    public List<Vector2> BaseRelativeMoveset;
    private List<BoardSquare> _possibleInteractableTiles;

    [HideInInspector] public TimelineNode _timelineNode;

    public List<BoardSquare> PossibleInteractableTiles
    {
        get
        {
            _possibleInteractableTiles = GetAllPossibleMovesetTiles();
            return _possibleInteractableTiles;
        }
        private set
        {
            _possibleInteractableTiles = value;
        }
    }
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
        if (Team == Team.Enemy)
        {
            return;
        }
        
        _upgradeButtonUIController = GetComponentInChildren<UpgradeButtonUIController>();
        if (_upgradeButtonUIController!= null)
        {
            _upgradeButtonUIController.SpeedButton.EventHandler.OnMouseDown += OnSpeedUpgradePressed;
            _upgradeButtonUIController.RangeButton.EventHandler.OnMouseDown += OnRangeUpgradePressed;
        }
       
    }

    private void UpdateLevel()
    {
        //Takes into account Range and Speed to determine level
        //Will not return less than 1
        Level = Mathf.Max(1, (Range - 1) + (Speed - 1)); 
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
    
    public void Init()
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

    public void RequestSelection(ChessPiece pieceToSelect)
    {
        ServiceLocator.GetService<BoardManager>().SelectedUnit = pieceToSelect;
    }

    private void UpdateMoveset()
    {
        //Temporary, used for testing
        if (IsBuilding)
        {
            return;
        }
        //--------------------------
        
        List<Vector2> newMoves = new List<Vector2>();
        //Add bas tile
        newMoves.Add(new Vector2(0,0));
        
        //TODO: Logic for other pieces.
        
        if (PieceType == ChessPieceType.King)
        {
            //TODO: KING logic 
        }
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
        _possibleInteractableTiles = GetAllPossibleMovesetTiles();
    }

    //Not used for now.
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
    public List<BoardSquare> GetAllPossibleMovesetTiles()
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
                    //check own tile
                    if (tile.ChessPieceAssigned == this)
                    {
                        //empty
                    }
                    //If tile is empty add tile
                    if (tile.IsEmpty())
                    {
                        tiles.Add(tile);
                    }
                    //If tile is occupied, check TEAM
                    else
                    {
                        //Add tile if different team, else dont add tile
                        if (tile.ChessPieceAssigned.Team != Team) 
                        {
                            tiles.Add(tile);
                        }
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
        //Init();

        _spritePosition = Sprite.transform.localPosition;

        SetUpEventHandlers();
    }

    private void SetUpEventHandlers()
    {
        if (Team == Team.Enemy)
        {
            Sprite.color = GlobalDebug.Instance.EnemyTintColor;
        }
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseEnter += (_) =>
        {
            HighlightTiles(PossibleInteractableTiles);
            if (_timelineNode != null)
            {
                _timelineNode.HighlightNode();
            }
        };
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseExit += (_) =>
        {
            UnHighlightTiles(PossibleInteractableTiles);
            if (_timelineNode != null)
            {
                _timelineNode.UnHighlightNode();
            }
        };
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseDown += (_) =>
        {
            RequestSelection(this);
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
                OnDeadState();
                break;
            case ChessPieceState.END:
                OnStateEnd?.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    void OnDeadState()
    {
        
            
        int reward = GlobalGameAssets.Instance.CurrencyBalanceData.BishopReward;
        _currencyManager.Value.AddCurrency(reward);
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
        if(Team == Team.Friendly) _upgradeButtonUIController.Show();
    }

    //Deselect State Logic
    private void Deselected()
    {
        if(Team == Team.Friendly) _upgradeButtonUIController.Hide();
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
        
        //Highlight own square
        AssignedSquare.Highlight(Team);
        yield return new WaitForSeconds(incrementTiming);
        
        foreach (var tile in tiles)
        {
            tile.Highlight(Team);
            tile.EvaluateAttackSignal(this);
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
        
        //UnHighlight own square
        AssignedSquare.UnHighlight();
        
        foreach (var tile in tiles)
        {
            tile.UnHighlight();
        }
    }
    
    private void OnDestroy()
    {
        if (_upgradeButtonUIController != null && Team == Team.Friendly)
        {
            _upgradeButtonUIController.SpeedButton.EventHandler.OnMouseDown -= OnSpeedUpgradePressed;
            _upgradeButtonUIController.RangeButton.EventHandler.OnMouseDown -= OnRangeUpgradePressed;
        }
       
    }
    
}

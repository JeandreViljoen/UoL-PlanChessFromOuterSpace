using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private ChessPieceState _state;

    public ChessPieceState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            RunStateLogic(_state);
        }
    }
    
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
                if(TimelineNode!=null) TimelineNode.RefreshPiece();
                ServiceLocator.GetService<ExecutionOrderManager>().RefreshTimelineOrder();
                ServiceLocator.GetService<UnitOrderTimelineController>().RefreshListIndices();
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

    public bool Dead = false;

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

    [HideInInspector] public TimelineNode TimelineNode;

    public List<BoardSquare> PossibleInteractableTiles
    {
        get
        {
            _possibleInteractableTiles = GetAllPossibleMovesetTiles().ToList();
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

    public event Action OnEndState;
    public event Action<BoardSquare> OnMoveEnd;
    public event Action StateLogicCompleted;
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

        OnMoveEnd += square => { State = ChessPieceState.END; };

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
        _possibleInteractableTiles = GetAllPossibleMovesetTiles().ToList();
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
    public IEnumerable<BoardSquare> GetAllPossibleMovesetTiles()
    {
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
                        yield return tile;
                    }
                    //If tile is occupied, check TEAM
                    else
                    {
                        //Add tile if different team, else dont add tile
                        if (tile.ChessPieceAssigned.Team != Team) 
                        {
                            yield return tile;
                        }
                        i += rangeOffset;
                    }
                }
            }
        }
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

        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseEnter += Highlight;
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseExit += UnHighlight;
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseDown += (_) =>
        {
            RequestSelection(this);
        };
        Sprite.gameObject.GetComponent<MouseEventHandler>().OnMouseUp += (_) =>
        {
           
        };
    }

    private void Highlight(PointerEventData _)
    {
        HighlightTiles(PossibleInteractableTiles, 0.2f);
        if (TimelineNode != null)
        {
            TimelineNode.HighlightNode();
        }
    }

    private void UnHighlight(PointerEventData _)
    {
        UnHighlightTiles(PossibleInteractableTiles);
        if (TimelineNode != null)
        {
            TimelineNode.UnHighlightNode();
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// Moves Piece to given BoardSquare position in parameters
    /// </summary>
    /// <param name="BoardSquare"></param>
    public bool MoveToBlock(BoardSquare square)
    {
        // check for target
        var piece = square.ChessPieceAssigned;
        if (piece)
        {
            if (piece.Team == Team)
                return false; // no friendly fire
            piece.Dead = true;
        }

        // animate
        Sequence s = DOTween.Sequence();
        s.Append( transform.DOMove(square.CenterSurfaceTransform.position, _animateSpeed).SetEase(Ease.InOutSine) );
        s.AppendCallback(() =>
        {
            OnMoveEndLogic(square);

        });
        return true;
        //IndexCodePosition = square.IndexCode;
    }

    private void OnMoveEndLogic(BoardSquare square)
    {
        UnHighlightTiles(PossibleInteractableTiles);
        BoardSquare oldSquare = AssignedSquare;
        AssignedSquare = square;
        AssignedSquare.ChessPieceAssigned = this;
        OnMoveEnd?.Invoke(square); 
        
        oldSquare.ChessPieceAssigned = null;
        
    }

    public void RunStateLogic(ChessPieceState state)
    {
        switch (state)
        {
            case ChessPieceState.INACTIVE:
                break;
            case ChessPieceState.START:
                if (GlobalDebug.Instance.ShowCombatMessageLogs)
                {
                    Debug.Log($"-------------------------- {ToString(this)} ---- START -------------------------\n");
                }
                TimelineNode.HighlightNode();
                UpdateMoveset();
                HighlightTiles(PossibleInteractableTiles, 0.2f);
                State = ChessPieceState.VALIDATE_BEST_MOVE;
                break;
            case ChessPieceState.VALIDATE_BEST_MOVE:
                //Requires AI Hook
                ValidateBestMove();
                break;
            case ChessPieceState.MOVE:
                break;
            case ChessPieceState.ATTACK:
                break;
            case ChessPieceState.DEAD:
                Killed();
                break;
            case ChessPieceState.END:
                UpdateMoveset();
                if (GlobalDebug.Instance.ShowCombatMessageLogs)
                {
                    Debug.Log($"--------------------------------------- END ------------------------------------\n");
                }
                TimelineNode.UnHighlightNode();
                OnEndState?.Invoke();
                ServiceLocator.GetService<UnitOrderTimelineController>().NodeOffset++;
                ServiceLocator.GetService<ExecutionOrderManager>().AdvanceQueue();
                
                State = ChessPieceState.INACTIVE;
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public static string ToString(ChessPiece piece)
    {
        return $"{piece.GetSpeedRepresentation()} {piece.Team} {piece.PieceType} ({piece.AssignedSquare.IndexCode})";
    }

    private void ValidateBestMove()
    {
        StartCoroutine(ValidateBestMoveDelayed());
    }

    private IEnumerator ValidateBestMoveDelayed()
    {
        if (PossibleInteractableTiles.Count < 1 )
        {
            if (GlobalDebug.Instance.ShowCombatMessageLogs) Debug.Log("\t\tNo move choices. Staying put\n");
            yield return new WaitForSeconds(0.5f);
            OnMoveEnd?.Invoke(AssignedSquare);
            yield break;
        }
        
        int choice = UnityEngine.Random.Range(0, PossibleInteractableTiles.Count);
        
        if (GlobalDebug.Instance.ShowCombatMessageLogs) Debug.Log(  $"\t\t{PossibleInteractableTiles.Count} possible moves.\n");
        
        PossibleInteractableTiles[choice].TargetFlash();
        yield return new WaitForSeconds(0.5f);

        if (!PossibleInteractableTiles[choice].IsEmpty())
        {
            if (GlobalDebug.Instance.ShowCombatMessageLogs) Debug.Log($"\t\tATTACKING {PossibleInteractableTiles[choice].ChessPieceAssigned.PieceType} at {PossibleInteractableTiles[choice].IndexCode}\n");

            State = ChessPieceState.ATTACK;
            PossibleInteractableTiles[choice].ChessPieceAssigned.State = ChessPieceState.DEAD;
            MoveToBlock(PossibleInteractableTiles[choice]);
        }
        else
        {
            if (GlobalDebug.Instance.ShowCombatMessageLogs) Debug.Log($"\t\tMOVING to {PossibleInteractableTiles[choice].IndexCode}\n");
            State = ChessPieceState.MOVE;
            MoveToBlock(PossibleInteractableTiles[choice]);
        }
    }

    private string GetSpeedRepresentation()
    {
        string s = "";
        for (int i = 0; i < _speed; i++)
        {
            s +=(">");
        }

        return s;
    }

    private void FalseSearchTiles(List<BoardSquare> tiles, float timePerTile)
    {
        _highlightMoveTween?.Kill();
        if(_highlightRoutine != null) StopCoroutine(_highlightRoutine);
        _highlightRoutine = StartCoroutine(DelayedFalseSearch(tiles, timePerTile));
        _highlightMoveTween = Sprite.gameObject.transform.DOLocalMove(_spritePosition + Vector3.up * 0.01f, 0.2f).SetEase(Ease.InOutSine);
    }

    private IEnumerator DelayedFalseSearch(List<BoardSquare> tiles, float delay)
    {
        foreach (var tile in tiles)
        {
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => { tile.Highlight(Team); });
            s.AppendInterval(delay*2);
            
            s.AppendCallback(() => { tile.UnHighlight(); });
            //s.AppendInterval(delay/2);

            yield return new WaitForSeconds(delay);
        }

        State = ChessPieceState.VALIDATE_BEST_MOVE;

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
    public void HighlightTiles(List<BoardSquare> tiles, float totalAnimTime)
    {
        _highlightMoveTween?.Kill();
        if(_highlightRoutine != null) StopCoroutine(_highlightRoutine);
        _highlightRoutine = StartCoroutine(DelayedHighlight(tiles, totalAnimTime));
        _highlightMoveTween = Sprite.gameObject.transform.DOLocalMove(_spritePosition + Vector3.up * 0.01f, 0.2f).SetEase(Ease.InOutSine);
    }

    private IEnumerator DelayedHighlight(List<BoardSquare> tiles, float totalAnimTime)
    {
        float totalAnimationTime = totalAnimTime;
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

    private void Killed()
    {
        if (Team == Team.Enemy)
        {
            _currencyManager.Value.RequestCaptureReward(this);
        }
        ServiceLocator.GetService<BoardManager>().DestroyPiece(this);
        Destroy(gameObject);
    }

}

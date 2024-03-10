using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Denotes the type of a piece.
/// </summary>
public enum ChessPieceType
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

/// <summary>
/// Denotes the team of a piece.
/// </summary>
public enum Team
{
    /// <summary>
    /// The King and player-placed units.
    /// </summary>
    Friendly,
    /// <summary>
    /// Computer-placed units.
    /// </summary>
    Enemy
}

/// <summary>
/// Denotes the state of a chess piece when moving.
/// </summary>
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

/// <summary>
/// Represents an automatically controlled, chess-piece like unit.
/// </summary>
public class ChessPiece : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// The type of the piece.
    /// </summary>
    public ChessPieceType PieceType;
    /// <summary>
    /// The team of the piece.
    /// </summary>
    public Team Team;

    /// <summary>
    /// The current state of a chess piece.
    /// </summary>
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
    private ChessPieceState _state;

    /// <summary>
    /// Gets or sets the priority of the piece.
    /// </summary>
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
                if(PieceType == ChessPieceType.King) _speedIconUIController.gameObject.SetActive(false);
                UpdateLevel();

                if (!_boardManager.Value.IsBuying())
                {
                    ServiceLocator.GetService<ExecutionOrderManager>().RefreshTimelineOrder();
                    ServiceLocator.GetService<UnitOrderTimelineController>().RefreshListIndices();
                }

            }
        }
    }
    private int _speed;

    /// <summary>
    /// Gets or sets the move range of the piece.
    /// </summary>
    public int Range
    {
        get
        {
            return _range;
        }
        set
        {
            _range = value;
            if (_rangeIconUIController == null)
            {
                _rangeIconUIController = GetComponentInChildren<RangeIconUIController>();
            }

            if (_rangeIconUIController != null)
            {
                _rangeIconUIController.Range = _range;
                if(PieceType == ChessPieceType.King) _rangeIconUIController.gameObject.SetActive(false);
            }

            if(AssignedSquare != null) UpdateMoveset();
            UpdateLevel();
        }
    }
    private int _range;

    /// <summary>
    /// Gets the level of the piece.
    /// </summary>
    public int Level { get; private set; }

    /// <summary>
    /// Gets or sets if the piece is selected.
    /// </summary>
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
    private bool _isSelected;

    /// <summary>
    /// Intended for the piece value of the unit.
    /// </summary>
    private float UnitValue;// => Level, type

    /// <summary>
    /// Gets the tiles this piece can move to.
    /// </summary>
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
    private List<BoardSquare> _possibleInteractableTiles = new List<BoardSquare>();

    /// <summary>
    /// Gets the possible moves of this piece relative to itself.
    /// </summary>
    public List<Vector2> RelativeMoveset;
    public List<Vector2> BaseRelativeMoveset;
    public List<BoardSquare> LastHighlightedTiles = new List<BoardSquare>();

    public SpriteRenderer Sprite;
    public SpriteRenderer SpriteHighlights;
    [HideInInspector] public Sprite Portrait;

    private ChessPieceData _data;

    public BoardSquare AssignedSquare;

    private bool IsCheckingKing = false;

    private Coroutine _highlightRoutine;

    private Tween _highlightMoveTween;
    private Vector3 _spritePosition;

    private SpeedIconUIController _speedIconUIController;
    [SerializeField] private RangeIconUIController _rangeIconUIController;
    private UpgradeButtonUIController _upgradeButtonUIController;

    private EasyService<CurrencyManager> _currencyManager;
    private EasyService<AI> _ai;
    private EasyService<AudioManager> _audioManager;
    private EasyService<ScoreManager> _scoreManager;
    private EasyService<GameStateManager> _stateManager;
    private EasyService<BoardManager> _boardManager;

    [HideInInspector] public TimelineNode TimelineNode;
    private float _animateSpeed;

    [SerializeField] private Light _light;
    [SerializeField] private float _lightIntensity = 3f;
    private Tween _tweenLight;

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
        if (_upgradeButtonUIController != null)
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
        // Level = Mathf.Max(1, (Range - 1) + (Speed - 1)); 
        Level = Speed + (Range) - 1;
        if (TimelineNode != null) TimelineNode.RefreshPiece();
    }

    /// <summary>
    /// Run when the upgrade speed button is clicked.
    /// </summary>
    /// <param name="_"></param>
    private void OnSpeedUpgradePressed(PointerEventData _)
    {
        //TODO: Validation checks & limits

        if (_currencyManager.Value.TryRemoveCurrency(_upgradeButtonUIController.SpeedButton.Cost))
        {
            Speed++;
            _audioManager.Value.PlaySound(Sound.UI_UpgradeSuccess, _upgradeButtonUIController.SpeedButton.gameObject);
            _upgradeButtonUIController.SpeedUpgrades++;
        }
        else
        {
            //TODO: SHow feedback for not enough currency
            _audioManager.Value.PlaySound(Sound.UI_Deny, _upgradeButtonUIController.SpeedButton.gameObject);
        }

    }

    /// <summary>
    /// Run when the upgrade range button is clicked.
    /// </summary>
    /// <param name="_"></param>
    private void OnRangeUpgradePressed(PointerEventData _)
    {
        //TODO: Validation checks & limits

        if (_currencyManager.Value.TryRemoveCurrency(_upgradeButtonUIController.RangeButton.Cost))
        {
            Range++;
            _audioManager.Value.PlaySound(Sound.UI_UpgradeSuccess, _upgradeButtonUIController.RangeButton.gameObject);
            _upgradeButtonUIController.RangeUpgrades++;
        }
        else
        {
            //TODO: SHow feedback for not enough currency
            _audioManager.Value.PlaySound(Sound.UI_Deny, _upgradeButtonUIController.RangeButton.gameObject);
        }

    }

    /// <summary>
    /// Initialize this piece.
    /// </summary>
    public void Init()
    {
        //Fetch Data as assigned in inspector
        _data = GetStartData(PieceType);

        if (Team == Team.Friendly)
        {
            Sprite.sprite = _data.Sprite;
            SpriteHighlights.sprite = _data.SpriteHighlights;
            Color hColor = GlobalGameAssets.Instance.HighlightColor;
            hColor.a = 0.3f;
            SpriteHighlights.color = hColor;
            SpriteHighlights.gameObject.SetActive(true);

            Portrait = _data.Portrait;
        }
        else
        {
            Sprite.sprite = _data.EnemySprite;
            SpriteHighlights.gameObject.SetActive(false);
            Portrait = _data.EnemyPortrait;
        }

        Speed = _data.DefaultSpeed;
        BaseRelativeMoveset = _data.BaseRelativeMoveset;
        Range = _data.DefaultRange; // Has to be retrieved after BaseRelativeMoveSet as it updates the moveset on this set method

    }

    public void RequestSelection(ChessPiece pieceToSelect)
    {
        if (ValidateInteraction()) return;
        ServiceLocator.GetService<BoardManager>().SelectedUnit = pieceToSelect;
    }

    /// <summary>
    /// Force recalculation of the piece's moveset.
    /// </summary>
    public void ForceUpdateMoveset()
    {
        UpdateMoveset();
    }

    /// <summary>
    /// Recalculate the piece's moveset.
    /// </summary>
    private void UpdateMoveset()
    {
        List<Vector2> newMoves = new List<Vector2>();
        //Add bas tile
        newMoves.Add(new Vector2(0, 0));

        //TODO: Logic for other pieces.
        if (PieceType == ChessPieceType.Pawn)
        {
            if (Range == 1)
            {
                foreach (var move in BaseRelativeMoveset)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
            }

            if (_range == 2)
            {
                foreach (var move in BaseRelativeMoveset)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -1; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
                
                foreach (var move in _data.UniqueRelativeAttackSet)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -1; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
            }
            
            if (_range == 3)
            {
                foreach (var move in BaseRelativeMoveset)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -1; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
                
                foreach (var move in _data.UniqueRelativeAttackSet)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -2; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
            }
            
            if (_range == 4)
            {
                foreach (var move in BaseRelativeMoveset)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -2; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
                
                foreach (var move in _data.UniqueRelativeAttackSet)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -2; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
            }
            
            if (_range > 4)
            {
                foreach (var move in BaseRelativeMoveset)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -2; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
                
                foreach (var move in _data.UniqueRelativeAttackSet)
                {
                    //Add a tile for the amount of "range"
                    for (int i = 1; i <= _range -2; i++)
                    {
                        newMoves.Add(move * i);
                    }
                }
            }
        }
        else if (PieceType == ChessPieceType.King)
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

    private void InitFloatingCurrency(int value)
    {
        FloatingCurrency f = Instantiate(GlobalGameAssets.Instance.FloatingCurrencyPrefab.GetComponent<FloatingCurrency>());
        f.transform.localPosition = transform.position + Vector3.up;
        f.Init(value);
    }

    /// <summary>
    /// Calculate a list of tiles this piece can move to.
    /// </summary>
    /// <returns>The list of tiles it can move to.</returns>
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
                BoardSquare tile = ServiceLocator.GetService<BoardManager>().GetTile(((int)absolute.x, (int)absolute.y));

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

    void Start()
    {
        _animateSpeed = GlobalGameAssets.Instance.ChessPieceAnimateSpeed;
        _spritePosition = Sprite.transform.localPosition;

        _boardManager.Value.OnKingChecked += CheckedLogic;
        _boardManager.Value.OnNoKingChecked += NotCheckedLogic;

        SetUpEventHandlers();
    }

    /// <summary>
    /// Runs when the piece checks the king, and when the king is in check.
    /// </summary>
    /// <param name="piece"></param>
    private void CheckedLogic(ChessPiece piece)
    {
        if (piece == this)
        {
            IsCheckingKing = true;
            LightOn();
            AssignedSquare.Highlight(Team.Enemy);
        }

        if (PieceType == ChessPieceType.King)
        {
            LightOn();
            AssignedSquare.Highlight(Team.Friendly);
        }
    }

    /// <summary>
    /// Runs when the king ceases to be in check.
    /// </summary>
    private void NotCheckedLogic()
    {
        IsCheckingKing = false;
        LightOff();
        AssignedSquare.UnHighlight();
    }

    private void SetUpEventHandlers()
    {
        if (Team == Team.Enemy)
        {
            //Sprite.color = GlobalDebug.Instance.EnemyTintColor;
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

    private bool ValidateInteraction()
    {
        if (_stateManager.Value.GameState != GameState.PREP) return true;
        if (ServiceLocator.GetService<BoardManager>().IsBuying()) return true;
        if (PieceType == ChessPieceType.King) return true;
        return false;
    }

    /// <summary>
    /// Run when the piece is hovered over.
    /// </summary>
    /// <param name="_"></param>
    private void Highlight(PointerEventData _)
    {
        if (ValidateInteraction()) return;

        _audioManager.Value.PlaySound(Sound.UI_Hover, gameObject);
        HighlightTiles(PossibleInteractableTiles, 0.2f);
        if (TimelineNode != null)
        {
            TimelineNode.HighlightNode();
        }
    }

    /// <summary>
    /// Run when the mouse pointer leaves the piece.
    /// </summary>
    /// <param name="_"></param>
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
    /// Move to the tile specified.
    /// </summary>
    /// <param name="BoardSquare"></param>
    public void MoveToBlock(BoardSquare square)
    {
        _audioManager.Value.PlaySound(Sound.ENEMY_Move, gameObject);
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove(square.CenterSurfaceTransform.position, _animateSpeed).SetEase(Ease.InOutSine));
        s.AppendCallback(() =>
        {
            Sprite.sortingOrder = 7 - square.IndexX + 1;
            SpriteHighlights.sortingOrder = 7 - square.IndexX + 2;
            OnMoveEndLogic(square);

        });
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

    /// <summary>
    /// Execute the piece's turn.
    /// </summary>
    /// <param name="state"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void RunStateLogic(ChessPieceState state)
    {
        switch (state)
        {
            case ChessPieceState.INACTIVE:
                break;
            case ChessPieceState.START:
                if (PieceType == ChessPieceType.King)
                {
                    State = ChessPieceState.END;
                    return;
                }
                if (GlobalDebug.Instance.ShowCombatMessageLogs)
                {
                    Debug.Log($"-------------------------- {ToString(this)} ---- START -------------------------\n");
                }
                LightOn();
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
                LightOff();
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

    /// <summary>
    /// Decide a tile to move to.
    /// </summary>
    private void ValidateBestMove()
    {
        StartCoroutine(ValidateBestMoveDelayed());
    }

    private IEnumerator ValidateBestMoveDelayed()
    {
        if (PossibleInteractableTiles.Count < 1)
        {
            if (GlobalDebug.Instance.ShowCombatMessageLogs) Debug.Log("\t\tNo move choices. Staying put\n");
            yield return new WaitForSeconds(0.5f);
            OnMoveEnd?.Invoke(AssignedSquare);
            yield break;
        }

        yield return new WaitForSeconds(0.5f);
        var dst = _ai.Value.RecommendMove();
        dst.TargetFlash();

        if (!dst.IsEmpty())
        {
            if (GlobalDebug.Instance.ShowCombatMessageLogs)
                Debug.Log($"\t\tATTACKING {dst.ChessPieceAssigned.PieceType} at {dst.IndexCode}\n");

            State = ChessPieceState.ATTACK;
            dst.ChessPieceAssigned.State = ChessPieceState.DEAD;
            //MoveToBlock(dst);
            _boardManager.Value.MovePiece((AssignedSquare.IndexX, AssignedSquare.IndexZ), (dst.IndexX, dst.IndexZ));
        }
        else
        {
            if (GlobalDebug.Instance.ShowCombatMessageLogs)
                Debug.Log($"\t\tMOVING to {dst.IndexCode}\n");
            State = ChessPieceState.MOVE;
            //MoveToBlock(dst);
            _boardManager.Value.MovePiece((AssignedSquare.IndexX, AssignedSquare.IndexZ), (dst.IndexX, dst.IndexZ));
        }
    }

    private string GetSpeedRepresentation()
    {
        string s = "";
        for (int i = 0; i < _speed; i++)
        {
            s += (">");
        }

        return s;
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
        LightOn();
        ServiceLocator.GetService<CameraManager>().FocusTile(this);
        if (Team == Team.Friendly) _upgradeButtonUIController.Show();
        _audioManager.Value.PlaySound(Sound.ENEMY_Activate, gameObject);
    }

    //Deselect State Logic
    private void Deselected()
    {
        LightOff();
        if (Team == Team.Friendly) _upgradeButtonUIController.Hide();
    }

    /// <summary>
    /// Highlight a list of BoardSquare tiles given in parameters
    /// </summary>
    public void HighlightTiles(List<BoardSquare> tiles, float totalAnimTime)
    {
        _highlightMoveTween?.Kill();
        if (_highlightRoutine != null) StopCoroutine(_highlightRoutine);
        _highlightRoutine = StartCoroutine(DelayedHighlight(tiles, totalAnimTime));
        _highlightMoveTween = Sprite.gameObject.transform.DOLocalMove(_spritePosition + Vector3.up * 0.01f, 0.2f).SetEase(Ease.InOutSine);
        LastHighlightedTiles = tiles;
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
            tile.SetAudio(Team);
            _audioManager.Value.PlaySound(Sound.UI_Subtle, tile.gameObject);
            tile.Highlight(Team);
            tile.EvaluateAttackSignal(this);
            yield return new WaitForSeconds(incrementTiming);
        }
    }

    /// <summary>
    /// Unhighlight a list of BoardSquare tiles given in parameters
    /// </summary>
    public void UnHighlightTiles(List<BoardSquare> tiles)
    {
        _highlightMoveTween?.Kill();
        _highlightMoveTween = Sprite.gameObject.transform.DOLocalMove(_spritePosition, 0.2f).SetEase(Ease.InOutSine);

        if (_highlightRoutine != null) StopCoroutine(_highlightRoutine);

        //UnHighlight own square
        AssignedSquare.UnHighlight();

        foreach (var tile in tiles)
        {
            tile.UnHighlight();
        }
    }

    /// <summary>
    /// Run on destruction of this piece.
    /// </summary>
    private void OnDestroy()
    {

        if (IsCheckingKing)
        {
            IsCheckingKing = false;
            _boardManager.Value.CheckIfKingIsInCheck();
        }

        if (_upgradeButtonUIController != null && Team == Team.Friendly)
        {
            _upgradeButtonUIController.SpeedButton.EventHandler.OnMouseDown -= OnSpeedUpgradePressed;
            _upgradeButtonUIController.RangeButton.EventHandler.OnMouseDown -= OnRangeUpgradePressed;
        }

        _boardManager.Value.OnKingChecked -= CheckedLogic;
        _boardManager.Value.OnNoKingChecked -= NotCheckedLogic;

    }

    /// <summary>
    /// Run when the piece is killed.
    /// </summary>
    private void Killed()
    {
        BeamController beam = Instantiate(GlobalGameAssets.Instance.BeamVFXDeathPrefab).GetComponent<BeamController>();
        beam.transform.position = AssignedSquare.CenterSurfaceTransform.position;
        beam.Order = 7 - AssignedSquare.IndexX + 1;
        beam.PlayVFX();

        if (Team == Team.Enemy)
        {
            int currency = _currencyManager.Value.RequestCaptureReward(this);
            InitFloatingCurrency(currency);
            _scoreManager.Value.AddEnemyDestroyedToScore(this.PieceType);
            _audioManager.Value.PlaySound(Sound.GAME_EnemyDeath, AssignedSquare.gameObject);
        }
        else
        {
            _audioManager.Value.PlaySound(Sound.GAME_FriendlyDeath, AssignedSquare.gameObject);
            _scoreManager.Value.AlliedPieceDestroyed();
            if (PieceType == ChessPieceType.King)
            {
                _stateManager.Value.GameState = GameState.LOSE;
            }
            
        }
        ServiceLocator.GetService<BoardManager>().DestroyPiece(this);
        Destroy(gameObject);
    }

    public void LightOn()
    {
        _tweenLight?.Kill();
        //_light.color = Color.white;
        _light.color = GlobalGameAssets.Instance.HighlightColor;
        _light.DOIntensity(_lightIntensity, 0.5f);
    }

    public void LightOff()
    {
        if (IsCheckingKing)
        {
            return;
        }
        _light.color = Color.white;
        _tweenLight?.Kill();
        _light.DOIntensity(0.0f, 0.5f);
    }

    public void SetLightEnabled(bool flag)
    {
        _light.gameObject.SetActive(flag);
    }

}

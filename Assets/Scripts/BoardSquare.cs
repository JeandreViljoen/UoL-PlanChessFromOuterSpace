using System;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Represents a tile in the board.
/// </summary>
public class BoardSquare : MonoBehaviour
{
    /// <summary>
    /// Get or set the position the tile represents.
    /// </summary>
    public (int x, int y) Position {
        get => (IndexX, IndexZ);
        set => (IndexX, IndexZ) = value;
    }
    public int IndexX;
    public int IndexZ;

    /// <summary>
    /// Gets or sets the textual code of this tile.
    /// </summary>
    public IndexCode IndexCode
    {
        get
        {
            return _indexCode;
        }
        set
        {
            _indexCode = value;
            IndexCodeTextField.text = _indexCode.ToString();
        }
    }
    private IndexCode _indexCode;

    /// <summary>
    /// The chess piece currently at this tile.
    /// </summary>
    public ChessPiece ChessPieceAssigned
    {
        get
        {
            return _chessPieceAssigned;
        }
        set
        {
            _chessPieceAssigned = value;
            if (value != null)
                _chessPieceAssigned.AssignedSquare = this;
        }
    }
    private ChessPiece _chessPieceAssigned;

    /// <summary>
    /// The text representation of the board's index.
    /// </summary>
    public TextMeshPro IndexCodeTextField;
    public Transform CenterSurfaceTransform;

    public SpriteRenderer HighlightSprite;
    private Tween _tweenHighlightMove;
    private Tween _tweenHighlightFade;
    private Vector3 _highlightWorldPosition;

    public SpriteRenderer Floor;

    
    public SpriteRenderer AttackSignalSprite;
    private Tween _tweenAttackSignalMove;
    private Tween _tweenAttackSignalFade;
    private Vector3 _attackSignalPosition;

    public SpriteRenderer TargetSignalSprite;
    private Tween _tweenTargetFlash;
    
    /// <summary>
    /// The color of white tiles.
    /// </summary>
    public Color WhiteTileColor;
    /// <summary>
    /// The color of black tiles.
    /// </summary>
    public Color BlackTileColor;

    public MouseEventHandler EventHandler;
    /// <summary>
    /// The game board.
    /// </summary>
    private EasyService<BoardManager> _boardManager;

    /// <summary>
    /// Fired on attempt to place a piece here.
    /// </summary>
    public event Action<ChessPieceType> TrySpawnPiece;
    public event Action OnUnitBuyHighlight;
    public event Action OnUnitBuyUnHighlight;

    /// <summary>
    /// Sets the color of this tile.
    /// </summary>
    /// <param name="isWhiteTile">The color of this tile.</param>
    public void SetTileColor(bool isWhiteTile)
    {
        if (isWhiteTile)
        {
            Floor.color = WhiteTileColor;
        }
        else
        {
            Floor.color = BlackTileColor;
        }
    }

    private void SetupEventHandlers()
    {
        EventHandler.OnMouseEnter += OnMouseEnterLogic;
        EventHandler.OnMouseExit += OnMouseExitLogic;
        EventHandler.OnMouseUp += OnMouseUpLogic;
    }

    private void OnMouseEnterLogic(PointerEventData _)
    {
        if (!_boardManager.Value.IsBuying()) return;
        if (!IsEmpty()) return;

        OnUnitBuyHighlight?.Invoke();
        //Highlight(Team.Friendly);
    }
    
    private void OnMouseExitLogic(PointerEventData _)
    {
        if (!_boardManager.Value.IsBuying()) return;

        OnUnitBuyUnHighlight?.Invoke();
        //UnHighlight();
    }
    
    private void OnMouseUpLogic(PointerEventData eventData)
    {
        if (!_boardManager.Value.IsBuying()) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //_boardManager.Value.TryBuyUnit();
        }
        else
        {
            //_boardManager.Value.CancelBuyUnit();
        }
    }

    void Start()
    {
        
        _highlightWorldPosition = HighlightSprite.transform.localPosition;
        _attackSignalPosition = AttackSignalSprite.transform.localPosition;
        _tweenHighlightFade = HighlightSprite.DOFade(0f, 0.000001f).SetUpdate(true);
        _tweenAttackSignalFade = AttackSignalSprite.DOFade(0f, 0.000001f).SetUpdate(true);
        _tweenTargetFlash = TargetSignalSprite.DOFade(0f, 0.000001f).SetUpdate(true);

        SetupEventHandlers();
        if (GlobalDebug.Instance.ShowIndexCodes)
        {
            IndexCodeTextField.gameObject.SetActive(true);
            //IndexCodeTextField.color = GlobalDebug.Instance.HighlightColor;
        }
        else
        {
            IndexCodeTextField.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Set the tiles's audio properties based on the piece's team.
    /// </summary>
    /// <param name="team">The team of the piece.</param>
    public void SetAudio(Team team)
    {
        AudioSource src = GetComponent<AudioSource>();

        if (team == Team.Friendly)
        {
            src.volume = 1f;
            src.pitch = 1f;
        }
        else
        {
            src.volume = 0.6f;
            src.pitch = 0.5f;
        }
    }
    
    void Update()
    {
        
    }
    
    // --------------- Public Functions and Methods ---------------

    /// <summary>
    /// Remove the chess piece from this tile.
    /// </summary>
    /// <remarks>
    /// Unity uses manual object management for GameObjects, and you
    /// are responsible for actually deleting it.
    /// </remarks>
    public void Clear()
    {
        ChessPieceAssigned = null;
    }

    /// <summary>
    /// Check if tile tile is unoccupied.
    /// </summary>
    /// <returns>True if this tile is unoccupied.</returns>
    public bool IsEmpty()
    {
        return ChessPieceAssigned == null;
    }
    
    /// <summary>
    /// Destroy the piece at this tile.
    /// </summary>
    /// <remarks>
    /// You should ensure that there are no further references to the
    /// chess piece being deleted before calling this function. Bad
    /// things can occur otherwise.
    /// </remarks>
    public void DestroyChessPiece()
    {
        Debug.Log($"Destroying chess piece at tile [{IndexX}, {IndexZ}]");
        Destroy(ChessPieceAssigned);
    }

    /// <summary>
    /// Update the index code of this piece.
    /// </summary>
    public void SetIndexCodeFromCartesian()
    {
        //Build the string
        string indexCodeString = GetLetter(IndexZ) + GetNumberString(IndexX);
        //Convert to enum
        bool successfulEnumConvert = Enum.TryParse(indexCodeString, out IndexCode indexCode);
        
        if (!successfulEnumConvert)
        {
            Debug.Log("[BoardSquare.cs] - GetIndexCodeFromCartesian() - Enum conversion from string to IndexCode enum failed. String passed was invalid, meaning an issue with cartesian coordinates. Returning A1 for now.");
            IndexCode = IndexCode.ERROR;
        }
        
        IndexCode = indexCode;
        
    }

    /// <summary>
    /// Convert an internal file number to algebraic form.
    /// </summary>
    /// <param name="z">The 0-based index of the file.</param>
    /// <returns>The algebraic notation of the file.</returns>
    private static string GetLetter(int z)
    {
        string letter = "";
        
        switch (z)
        {
            case 0:
                letter = "A";
                break;
            case 1:
                letter = "B";
                break;
            case 2:
                letter = "C";
                break;
            case 3:
                letter = "D";
                break;
            case 4:
                letter = "E";
                break;
            case 5:
                letter = "F";
                break;
            case 6:
                letter = "G";
                break;
            case 7:
                letter = "H";
                break;
            default:
                Debug.LogError("[BoardSquare.cs] - GetLetter() - X index out of bounds, make sure index X stays from 0-7 range.");
                break;
        }

        return letter;
    }

    /// <summary>
    /// Convert an internal rank number to algebraic form.
    /// </summary>
    /// <param name="x">The 0-based index of the rank.</param>
    /// <returns>The algebraic notation of the rank.</returns>
    private string GetNumberString(int x)
    {
        if (x > 7)
        {
            Debug.LogError("[BoardSquare.cs] - GetLetter() - Z index out of bounds, make sure index Z stays from 0-7 range.");
            return "";
        }
        
        return (x + 1).ToString();
    }

    /// <summary>
    /// Highlight the tile.
    /// </summary>
    /// <param name="team">The team of the piece highlighted.</param>
    public void Highlight(Team team)
    {
        if (team == Team.Friendly)
        {
            Color c = GlobalGameAssets.Instance.HighlightColor;
            c.a = 0f;
            HighlightSprite.color = c;
        }
        else
        {
            Color c = GlobalDebug.Instance.EnemyTintColor;
            c.a = 0f;
            HighlightSprite.color = c;
        }

        _tweenHighlightMove?.Kill();
        _tweenHighlightFade?.Kill();
        _tweenHighlightMove = HighlightSprite.transform.DOLocalMove(_highlightWorldPosition + Vector3.up*5f, 0.3f).SetEase(Ease.InOutSine, 3f);
        _tweenHighlightFade = HighlightSprite.DOFade(0.5f, 0.3f).SetEase(Ease.InOutSine, 3f);
    }

    /// <summary>
    /// Stop highlighting the tile.
    /// </summary>
    public void UnHighlight()
    {
        _tweenHighlightMove?.Kill();
        _tweenHighlightFade?.Kill();
        _tweenHighlightMove = HighlightSprite.transform.DOLocalMove(_highlightWorldPosition, 0.3f).SetEase(Ease.InOutSine);
        _tweenHighlightFade = HighlightSprite.DOFade(0f, 0.3f).SetEase(Ease.InOutSine);
        
        HideAttackSignal();
    }

    /// <summary>
    /// Show the capturable indicator if this tile has a piece that can
    /// be captured by the piece specified.
    /// </summary>
    /// <param name="attackingPiece">The attacking piece.</param>
    public void EvaluateAttackSignal(ChessPiece attackingPiece)
    {
        if (attackingPiece == _chessPieceAssigned)
        {
            return;
        }

        if (_chessPieceAssigned != null)
        {
            if (_chessPieceAssigned.Team != attackingPiece.Team)
            {
                ShowAttackSignal();
            }
        }
        else
        {
            HideAttackSignal();
        }
    }

    /// <summary>
    /// Shows the capturable indicator.
    /// </summary>
    private void ShowAttackSignal()
    {
        _tweenAttackSignalFade?.Kill();
        _tweenAttackSignalMove?.Kill();

        _tweenAttackSignalMove = AttackSignalSprite.transform.DOLocalMove(_attackSignalPosition + Vector3.up*10f, 0.3f).SetEase(Ease.InOutSine);
        _tweenAttackSignalFade = AttackSignalSprite.DOFade(0.75f, 0.3f).SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Hides the capturable indicator.
    /// </summary>
    private void HideAttackSignal()
    {
        _tweenAttackSignalFade?.Kill();
        _tweenAttackSignalMove?.Kill();

        _tweenAttackSignalMove = AttackSignalSprite.transform.DOLocalMove(_attackSignalPosition, 0.2f).SetEase(Ease.InOutSine);
        _tweenAttackSignalFade = AttackSignalSprite.DOFade(0f, 0.3f).SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Highlight the target a piece is moving to.
    /// </summary>
    public void TargetFlash()
    {
        _tweenTargetFlash?.Kill();
        Sequence f = DOTween.Sequence();

        f.Append(TargetSignalSprite.DOFade(1f, 0.1f));
        f.Append(TargetSignalSprite.DOFade(0f, 0.1f));
        f.Append(TargetSignalSprite.DOFade(1f, 0.1f));
        f.Append(TargetSignalSprite.DOFade(0f, 0.1f));
        f.Append(TargetSignalSprite.DOFade(1f, 0.1f));
        f.Append(TargetSignalSprite.DOFade(0f, 0.1f));
        f.Append(TargetSignalSprite.DOFade(1f, 0.1f));
        f.Append(TargetSignalSprite.DOFade(0f, 0.5f));
        
        
        _tweenTargetFlash = f;
    }


}

public enum IndexCode
{
    A1,A2,A3,A4,A5,A6,A7,A8,
    B1,B2,B3,B4,B5,B6,B7,B8,
    C1,C2,C3,C4,C5,C6,C7,C8,
    D1,D2,D3,D4,D5,D6,D7,D8,
    E1,E2,E3,E4,E5,E6,E7,E8,
    F1,F2,F3,F4,F5,F6,F7,F8,
    G1,G2,G3,G4,G5,G6,G7,G8,
    H1,H2,H3,H4,H5,H6,H7,H8,
    ERROR
}

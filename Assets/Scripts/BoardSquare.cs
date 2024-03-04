using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardSquare : MonoBehaviour
{

    // --------------- Member variables and data --------------- //
    
    public int IndexX;
    public int IndexZ;

    private ChessPiece _chessPieceAssigned;
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

    public TextMeshPro IndexCodeTextField;
    public Transform CenterSurfaceTransform;
    
    private Tween _bounceAnimateTween;

    private IndexCode _indexCode;

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
    

    public Color WhiteTileColor;
    public Color BlackTileColor;

    public MouseEventHandler EventHandler;
    private EasyService<BoardManager> _boardManager;

    public event Action<ChessPieceType> TrySpawnPiece;
    public event Action OnUnitBuyHighlight;
    public event Action OnUnitBuyUnHighlight;

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
            _boardManager.Value.CancelBuyUnit();
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
        
        _bounceAnimateTween = transform.DOMove(transform.localPosition, 0.2f).SetEase(Ease.InOutSine);
    }
    
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

    public void Clear()
    {
        ChessPieceAssigned = null;
    }
    
    
    // Returns the chess piece assigned to this board square
    public ChessPiece GetChessPiece()
    {
        return ChessPieceAssigned.GetComponent<ChessPiece>();
    }

    // Returns true if there is no chess piece on this board square
    public bool IsEmpty()
    {
        return (ChessPieceAssigned == null);
    }
    
    // Destroys the chess piece assigned to this board square, use cautiously
    public void DestroyChessPiece()
    {
        Debug.Log("Destroying chess piece assigned to board square with index [" + IndexX.ToString() + "," + IndexZ.ToString() + "]");
        Destroy(ChessPieceAssigned);
    }

    //Converts cartesian coordinates to index code. e.g. F7
    public void SetIndexCodeFromCartesian()
    {
        IndexCode indexCode;
        
        //Build the string
        string indexCodeString = GetLetter(IndexZ) + GetNumberString(IndexX);
        //Convert to enum
        bool successfulEnumConvert = Enum.TryParse(indexCodeString, out indexCode);
        
        if (!successfulEnumConvert)
        {
            Debug.Log("[BoardSquare.cs] - GetIndexCodeFromCartesian() - Enum conversion from string to IndexCode enum failed. String passed was invalid, meaning an issue with cartesian coordinates. Returning A1 for now.");
            IndexCode = IndexCode.ERROR;
        }
        
        IndexCode = indexCode;
        
    }

    //Returns corresponding Letter for Z
    private string GetLetter(int z)
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

    //Returns to corresponding number for X
    private string GetNumberString(int x)
    {
        if (x > 7)
        {
            Debug.LogError("[BoardSquare.cs] - GetLetter() - Z index out of bounds, make sure index Z stays from 0-7 range.");
            return "";
        }
        
        return (x + 1).ToString();
    }

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
    
    public void UnHighlight()
    {
        _tweenHighlightMove?.Kill();
        _tweenHighlightFade?.Kill();
        _tweenHighlightMove = HighlightSprite.transform.DOLocalMove(_highlightWorldPosition, 0.3f).SetEase(Ease.InOutSine);
        _tweenHighlightFade = HighlightSprite.DOFade(0f, 0.3f).SetEase(Ease.InOutSine);
        
        HideAttackSignal();
    }

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

    private void ShowAttackSignal()
    {
        _tweenAttackSignalFade?.Kill();
        _tweenAttackSignalMove?.Kill();

        _tweenAttackSignalMove = AttackSignalSprite.transform.DOLocalMove(_attackSignalPosition + Vector3.up*10f, 0.3f).SetEase(Ease.InOutSine);
        _tweenAttackSignalFade = AttackSignalSprite.DOFade(0.75f, 0.3f).SetEase(Ease.InOutSine);
    }
    
    private void HideAttackSignal()
    {
        _tweenAttackSignalFade?.Kill();
        _tweenAttackSignalMove?.Kill();

        _tweenAttackSignalMove = AttackSignalSprite.transform.DOLocalMove(_attackSignalPosition, 0.2f).SetEase(Ease.InOutSine);
        _tweenAttackSignalFade = AttackSignalSprite.DOFade(0f, 0.3f).SetEase(Ease.InOutSine);
    }

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

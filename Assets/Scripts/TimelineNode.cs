using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimelineNode : MonoBehaviour
{
    public ChessPiece Piece;

    [SerializeField] private Image _bulletpoint;
    [SerializeField] private Transform _linkPosition;
    [SerializeField] private Image _portraitBorder;
    [SerializeField] private Image _portrait;
    [SerializeField] private TimelineSpeedIcons _speedIcons;
    [SerializeField] private TimelineRangeIcons _rangeIcons;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private GameObject _panelsContainer;
     private Vector3 _panelsContainerBasePosition;

    private MouseEventHandler _eventHandler;

    private Tween _moveTween;
    private Tween _highlightMoveTween;
    private Tween _tweenBulletpointScale;

    private Transform _assignedNode;
    private bool _isDying = false;

    /// <summary>
    /// Assigns a reference to a chess piece on the game board and then fully initialises other references and dependencies
    /// </summary>
    /// <param name="piece"></param>
    public void SetPiece(ChessPiece piece)
    {
        Piece = piece;
        InitPiece();
        
        _panelsContainerBasePosition = _panelsContainer.transform.localPosition;
        if (Piece.Team == Team.Enemy)
        {
            _portrait.color = GlobalDebug.Instance.EnemyTintColor;
            _portraitBorder.color = GlobalDebug.Instance.EnemyTintColor;
            _portraitBorder.DOFade(0.3f, 0.00001f);
        }

        SetUpEventHandlers();
    }

    /// <summary>
    /// Refresh the node UI to update latest values
    /// </summary>
    public void RefreshPiece()
    {
        _speedIcons.IconsToShow = Piece.Speed;
        _rangeIcons.IconsToShow = Piece.Range;
        _levelText.text = $"LEVEL {Piece.Level}";
    }

    /// <summary>
    /// Baseline logic for initialisation
    /// </summary>
    private void InitPiece()
    {
        Piece.TimelineNode = this;
        _portrait.sprite = Piece.Portrait;
        _speedIcons.IconsToShow = Piece.Speed;
        _rangeIcons.IconsToShow = Piece.Range;
        _levelText.text = $"LEVEL {Piece.Level}";
        _levelText.color = Piece.Team == Team.Friendly
            ? GlobalGameAssets.Instance.HighlightColor
            : GlobalDebug.Instance.EnemyTintColor;
        if (Piece.PieceType == ChessPieceType.King)
        {
            _speedIcons.gameObject.SetActive(false);
            _rangeIcons.gameObject.SetActive(false);
            _levelText.text = $"KING";
            _portraitBorder.color = new Color(1f,1f,1f, 0.05f);
        }
    }

    /// <summary>
    /// Animates moving the node to a given position
    /// </summary>
    /// <param name="node"></param>
    /// <param name="speed"></param>
    public void MoveNode(Transform node, float speed)
    {
        _moveTween?.Kill();
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalMove(node.localPosition, speed).SetEase(Ease.InOutSine));
        _moveTween = s;
    }
    
    private void SetUpEventHandlers()
    {
        if (Piece == null)
        {
            Debug.LogError("[TimelineNode.cs] - SetUpEventHandlers() : Piece is not assigned. skipping event subscriptions");
            return;
        }
        
        gameObject.GetComponent<MouseEventHandler>().OnMouseEnter += (_) =>
        {
            Piece.HighlightTiles(Piece.PossibleInteractableTiles, 0.2f);
            HighlightNode();
        };
        gameObject.GetComponent<MouseEventHandler>().OnMouseExit += (_) =>
        {
            Piece.UnHighlightTiles(Piece.PossibleInteractableTiles);
            UnHighlightNode();
        };
        gameObject.GetComponent<MouseEventHandler>().OnMouseDown += (_) =>
        {
            Piece.RequestSelection(Piece);
        };
        gameObject.GetComponent<MouseEventHandler>().OnMouseUp += (_) =>
        {
           
        };
    }

    public void HighlightNode()
    {
        if (_isDying)
        {
            return;
        }
        _highlightMoveTween?.Kill();
        _tweenBulletpointScale?.Kill();
        _highlightMoveTween = _panelsContainer.transform.DOLocalMove(_panelsContainerBasePosition + Vector3.right*10f, 0.15f).SetEase(Ease.InOutSine);
        _bulletpoint.color = GlobalGameAssets.Instance.HighlightColor;
        _tweenBulletpointScale = _bulletpoint.transform.DOScale(Vector3.one *1.2f, 0.15f);
    }
    
    public void UnHighlightNode()
    {
        if (_isDying)
        {
            return;
        }
        _highlightMoveTween?.Kill();
        _tweenBulletpointScale?.Kill();
        _highlightMoveTween = _panelsContainer.transform.DOLocalMove(_panelsContainerBasePosition, 0.15f).SetEase(Ease.InOutSine);
        _bulletpoint.color = Color.white;
        _tweenBulletpointScale = _bulletpoint.transform.DOScale(Vector3.one, 0.15f);
    }

    public void KillAnimation(float killTime)
    {
        Sequence k = DOTween.Sequence();
        
        _isDying = true;
        _highlightMoveTween?.Kill();
        _tweenBulletpointScale?.Kill();
        _bulletpoint.color = GlobalGameAssets.Instance.HighlightColor;
        _tweenBulletpointScale = _bulletpoint.transform.DOScale(Vector3.one *1.2f, killTime);
        k.Append( _panelsContainer.transform.DOLocalMove(_panelsContainerBasePosition + Vector3.right*100f, killTime).SetEase(Ease.InOutSine));
        k.AppendCallback(() => { Destroy(gameObject);});
    }
    
}

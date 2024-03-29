﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using DG.Tweening.Core;
using Services;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class BoardManager : MonoService
{
    // --------------- Member variables and data --------------- //
    // ChessBoard Settings
    [Header("Chess Board Settings")]
    [SerializeField, Tooltip("Chess board width.")]
    private int _boardWidth = 8;
    [SerializeField, Tooltip("Chess board depth.")]
    private int _boardDepth = 8;
    [SerializeField, Tooltip("Chess board center position.")]
    private Vector3 _centerPosition;
    // This list contains every board square generated on initialization
    private List<BoardSquare> _boardSquares;
    // This list contains every chess piece on the board
    private Dictionary<(int, int), ChessPiece> _pieces;
    public IReadOnlyDictionary<(int, int), ChessPiece> Pieces => _pieces;
    
    //REFACTOR
    public List<ChessPiece> ListofPieces;

    // BoardSquare Settings
    [Header("Board Squares")]
    [SerializeField, Tooltip("Prefab that is used for the board squares.")]
    private GameObject _squareBoardPrefab;
    [SerializeField, Tooltip("Materials for the board squares")]
    private Material[] _materialsArray;
 
    // Chess pieces information
    [Header("Chess Pieces Information and Properties")]
    
    private bool _isBuyingUnit = false;

    public GameObject BuyingTilesAccesHighlight;
    private ChessPieceType _unitToBuy;
    private Tween _tweenBuyMove;

    private ChessPiece _newPieceToBuy;

    private ChessPiece _selectedUnit;
    
    // Tweens
    private Tween _bounceAnimate;

    // Services
    private EasyService<GameStateManager> _gameStateManager;
    private EasyService<AudioManager> _audioManager;

    public event Action OnSuccessfulPurchase;
    public event Action OnCancelledPurchase;

    public ChessPiece SelectedUnit
    {
        get
        {
            return _selectedUnit;
        }
        set
        {
            if (_selectedUnit != null)
            {
                _selectedUnit.IsSelected = false;
            }
            
            _selectedUnit = value;

            if (_selectedUnit != null)
            {
                _selectedUnit.IsSelected = true;
            }
           
        }
    }

    public void ClearUnitSelection()
    {
        foreach (var unit in _pieces)
        {
            unit.Value.IsSelected = false;
        }

        _selectedUnit = null;
    }

    
    /// <summary>
    /// Sets Board manager to buying state
    /// </summary>
    public void EnableBuyingState(ChessPieceType type)
    {

        if (_isBuyingUnit)
        {
            CancelBuyUnit();
        }
        _isBuyingUnit = true;
        _unitToBuy = type;
        InitNewPieceToBuy(_unitToBuy);
        ServiceLocator.GetService<HUDManager>().CancelDeploymentText.gameObject.SetActive(true);
        
    }

    public bool IsBuying()
    {
        return _isBuyingUnit;
    }


    /// <summary>
    /// Attempts to buy and place the selected unity on the board and update all related systems.
    /// </summary>
    public void TryBuyUnit()
    {
        int cost = GlobalGameAssets.Instance.CurrencyBalanceData.GetChessPieceCost(_newPieceToBuy.PieceType);
        if (ServiceLocator.GetService<CurrencyManager>().HasCurrency(cost))
        {
            
            BeamController beam = Instantiate(GlobalGameAssets.Instance.BeamVFXShortPrefab).GetComponent<BeamController>();
            beam.transform.position = _newPieceToBuy.AssignedSquare.CenterSurfaceTransform.position;
            beam.Order = 7 - _newPieceToBuy.AssignedSquare.IndexX + 1;
            beam.PlayVFX();

            ChessPiece piece = CreatePiece(_newPieceToBuy.PieceType, _newPieceToBuy.AssignedSquare.IndexCode, Team.Friendly);
            _audioManager.Value.PlaySound(Sound.GAME_FriendlySpawn);
            
            ServiceLocator.GetService<CurrencyManager>().TryRemoveCurrency(cost);
            if (_unitToBuy == ChessPieceType.King)
            {
                _gameStateManager.Value.HasPlacedKing = true;
                _gameStateManager.Value.KingReference = piece;
                ServiceLocator.GetService<HUDManager>().KingController.Disable();
            }
            OnSuccessfulPurchase?.Invoke();
        }

        CancelBuyUnit();
    }

    public event Action<ChessPiece> OnKingChecked;
    public event Action OnNoKingChecked; 

    /// <summary>
    /// Returns a bool whether King is in danger
    /// </summary>
    public void CheckIfKingIsInCheck()
    {
        bool foundCheck = false;
        
        foreach (var piece in ListofPieces)
        {
            if (piece.Team == Team.Enemy)
            {
                foreach (var tile in piece.PossibleInteractableTiles)
                {
                    if (tile.ChessPieceAssigned != null && tile.ChessPieceAssigned.PieceType == ChessPieceType.King)
                    {
                        OnKingChecked?.Invoke(piece);
                        foundCheck = true;
                    }
                }
            }
        }

        if (!foundCheck)
        {
            OnNoKingChecked?.Invoke();
        }
    }

    private bool HasBeenInitToBoard = false;
    
    
    /// <summary>
    /// Cancels the buy state and returns to normal functioning
    /// </summary>
    public void CancelBuyUnit()
    {
        if (!IsBuying())
        {
            return;
        }
        if(HasBeenInitToBoard) _newPieceToBuy.UnHighlightTiles(_newPieceToBuy.LastHighlightedTiles);
        HasBeenInitToBoard = false;
        Destroy(_newPieceToBuy.gameObject);
        _newPieceToBuy = null;
        DisableBuyingState();
        ServiceLocator.GetService<HUDManager>().CancelDeploymentText.gameObject.SetActive(false);
        OnCancelledPurchase?.Invoke();
        
    }

    public void DisableBuyingState()
    {
        _isBuyingUnit = false;
    }

    /// <summary>
    /// Subscribes all tile behavior callbacks used during the Buy state.
    /// </summary>
    private void SubscribeBuyEvents()
    {
        foreach (var tile in _boardSquares)
        {
            //Limit placement column
            if (tile.IndexX > 1)
            {
                return;
            }
            
            tile.OnUnitBuyHighlight += () =>
            {
                HasBeenInitToBoard = true;
                //UnHighlight previous tile
                if (_newPieceToBuy!=null && _newPieceToBuy.AssignedSquare != null)
                {
                    _newPieceToBuy.UnHighlightTiles(_newPieceToBuy.LastHighlightedTiles);
                }
                _tweenBuyMove?.Kill();
                
                _newPieceToBuy.AssignedSquare = tile;
                _newPieceToBuy.ForceUpdateMoveset();
                _newPieceToBuy.HighlightTiles(_newPieceToBuy.PossibleInteractableTiles, 0.1f);
                _tweenBuyMove = _newPieceToBuy.transform.DOMove(tile.CenterSurfaceTransform.position, 0.1f).SetEase(Ease.InOutSine);
            };

            tile.OnUnitBuyUnHighlight += () =>
            {
                if (tile.ChessPieceAssigned == _newPieceToBuy)
                {
                    tile.ChessPieceAssigned = null;
                }
            };
            
           
        }
    }

    /// <summary>
    /// Creates a temporary piece to show where the piece is going to be placed on the board
    /// </summary>
    private void InitNewPieceToBuy(ChessPieceType type)
    { 
        _newPieceToBuy = Instantiate(GlobalGameAssets.Instance.GetChessPiecePrefab(type)).GetComponent<ChessPiece>();
        _newPieceToBuy.transform.position = _centerPosition + Vector3.up * 5f;
        _newPieceToBuy.Init();
        _newPieceToBuy.Sprite.gameObject.layer = 2;
        _newPieceToBuy.LightOn();
    }

    void Start()
    {
        DisableBuyingState();
        _boardSquares = new List<BoardSquare>();
        _pieces = new Dictionary<(int, int), ChessPiece>();

        GenerateChessBoard();
    }

    void Update()
    {
        //Mouse check during buying state for Qol response
        if (_isBuyingUnit)
        {
            if (!BuyingTilesAccesHighlight.activeSelf)
            {
                BuyingTilesAccesHighlight.SetActive(true);
            }
            
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                CancelBuyUnit();
            }
            else if (Input.GetMouseButtonDown(0) && _newPieceToBuy.AssignedSquare != null)
            {
                TryBuyUnit();
            }
        }
        else
        {
            if (BuyingTilesAccesHighlight.activeSelf)
            {
                BuyingTilesAccesHighlight.SetActive(false);
            }
        }
    }

    // Creates the chess board in the scene and stores all the necessary information
    private void GenerateChessBoard()
    {
        // Avoid system exception if squareBoard prefab is missing
        if (!_squareBoardPrefab)
        {
            Debug.Log("Chess board can be created because the square board prefab is missing.");
            return;
        }

        Vector3 squareBoardSize = Vector3.one;
        Vector3 firstSquarePosition = new Vector3(_centerPosition.x - squareBoardSize.x/2 * _boardDepth, 0,
            _centerPosition.z - squareBoardSize.z/2 * _boardWidth);

        // Create the game objects based on the prefab assigned
        Vector3 squareBoardPosition = firstSquarePosition;
        int materialIteration = 0;
        bool isWhiteTile = true;
        
        for (int i = 0; i < _boardDepth; ++i)
        {
            isWhiteTile = !isWhiteTile;
            
            materialIteration = (i + 1) % 2;
            for (int j = 0; j < _boardWidth; ++j)
            {
                // Instantiate square board and set properties
                GameObject squareBoard = Instantiate(_squareBoardPrefab);
                squareBoard.transform.position = squareBoardPosition;
                BoardSquare boardSquareComponent = squareBoard.GetComponent<BoardSquare>();
                boardSquareComponent.IndexX = i;
                boardSquareComponent.IndexZ = j;
                boardSquareComponent.SetIndexCodeFromCartesian();
                
                boardSquareComponent.SetTileColor(isWhiteTile);
                isWhiteTile = !isWhiteTile;

                // Add the newly created squareBoard to the list containing all board squares
                boardSquareComponent.name = $"TILE: {boardSquareComponent.IndexCode}";
                _boardSquares.Add(boardSquareComponent);

                // Update new X position
                squareBoardPosition.x += squareBoardSize.x;
            }
            // Reset X position
            squareBoardPosition.x = firstSquarePosition.x;

            // Update Z position
            squareBoardPosition.z += squareBoardSize.z;
        }

        SubscribeBuyEvents();
        ExecuteDebugCode();
        
    }

    /// <summary>
    /// Debug related code
    /// </summary>
    private void ExecuteDebugCode()
    {
        if (GlobalDebug.Instance.PopulateBoardOnStart)
        {
            for (int i = 0; i < _boardDepth; ++i)
            {
                for (int j = 0; j < _boardWidth; ++j)
                {
                    float rng = UnityEngine.Random.Range(0f, 1f);

                    if (rng <= GlobalDebug.Instance.ChanceToPopulateTile)
                    {
                        int randomPiece = UnityEngine.Random.Range(0, 5);
                            
                        ChessPieceType type = (ChessPieceType) Enum.ToObject(typeof(ChessPieceType), randomPiece);

                        Team randomTeam;
                        if (UnityEngine.Random.Range(0,2) == 0)
                        {
                            randomTeam = Team.Friendly;
                        }
                        else
                        {
                            randomTeam = Team.Enemy;
                        }
                    
                        ChessPiece piece = CreatePiece(type, i, j, randomTeam);
                    
                    
                        piece.Speed = UnityEngine.Random.Range(1, 5);
                        Debug.Log($"{piece.Team} {piece.PieceType} | Speed : {piece.Speed}");
                    }
                }
            }
        }
    }

    // --------------- Public Functions and Methods ---------------

    /// <summary>
    /// Get a tile with the specified position.
    /// </summary>
    /// <param name="x">The file of the tile.</param>
    /// <param name="y">The rank of the tile.</param>
    /// <returns>The tile at the specified location.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The position is outside the chessboard.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// The tile does not exist.
    /// </exception>
    public BoardSquare GetTile(int x, int y)
    {
        CheckIndex(x, y);

        BoardSquare querySquareBoard =
            _boardSquares.Find((square => square.IndexX == x && square.IndexZ == y));

        // If for some reason the square board does not exist, notify user and return a null object
        if (querySquareBoard == null)
            throw new NullReferenceException($"Tile object at (${x}, ${y}) does not exist");

        return querySquareBoard;
    }


    /// <summary>
    /// Get a tile with the specified position.
    /// </summary>
    /// <param name="code">The position code of the tile.</param>
    /// <returns>The tile at the specified location.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The position is outside the chessboard.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// The tile does not exist.
    /// </exception>
    public BoardSquare GetTile(IndexCode code)
        => GetTile((int)code % 8, (int)code / 8);


    /// <summary>
    /// Get a tile with the specified position.
    /// </summary>
    /// <param name="tile">The position of the tile.</param>
    /// <returns>The tile at the specified location.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The position is outside the chessboard.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// The tile does not exist.
    /// </exception>
    public BoardSquare GetTile((int x, int y) tile)
        => GetTile(tile.x, tile.y);

    /// <summary>
    /// Create a chess piece at the specified location.
    /// </summary>
    /// <param name="type">The type of the piece.</param>
    /// <param name="pos">The position of the piece.</param>
    /// <param name="team">The faction of the piece.</param>
    /// <returns>The newly created chess piece.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The piece type or team is invalid, or the position is outside the board.
    /// </exception>
    public ChessPiece CreatePiece(ChessPieceType type, (int, int) pos, Team team)
    {
        BoardSquare square = GetTile(pos);

        if (!square.IsEmpty())
        {
            Debug.Log("Cannot create chess piece on given BoardSquare because there is already a chess piece assigned.");
            return null;
        }

        ChessPiece piece;

        //Instantiate given piece
        switch (type)
        {
            case ChessPieceType.Pawn:
                piece = Instantiate(GlobalGameAssets.Instance.PawnPrefab).GetComponent<ChessPiece>();
                break;
            case ChessPieceType.Knight:
                piece = Instantiate(GlobalGameAssets.Instance.KnightPrefab).GetComponent<ChessPiece>();
                break;
            case ChessPieceType.Bishop:
                piece = Instantiate(GlobalGameAssets.Instance.BishopPrefab).GetComponent<ChessPiece>();
                break;
            case ChessPieceType.Rook:
                piece = Instantiate(GlobalGameAssets.Instance.RookPrefab).GetComponent<ChessPiece>();
                break;
            case ChessPieceType.Queen:
                piece = Instantiate(GlobalGameAssets.Instance.QueenPrefab).GetComponent<ChessPiece>();
                break;
            case ChessPieceType.King:
                piece = Instantiate(GlobalGameAssets.Instance.KingPrefab).GetComponent<ChessPiece>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (piece == null)
            throw new NullReferenceException($"Prefab for piece type ${type} is broken");

        // TODO: More initialisation required on the chess piece
        piece.gameObject.transform.position = square.CenterSurfaceTransform.position;
        piece.Sprite.sortingOrder = (_boardDepth - 1) - pos.Item1;
        piece.SpriteHighlights.sortingOrder = (_boardDepth - 1) - pos.Item1 + 1;
        piece.Team = team;
        piece.AssignedSquare = square;
        square.ChessPieceAssigned = piece;
        piece.Init();

        ListofPieces.Add(piece);
        _pieces.Add(pos, piece);
        ServiceLocator.GetService<ExecutionOrderManager>().RefreshTimelineOrder();
        ServiceLocator.GetService<UnitOrderTimelineController>().AddNode(piece);
        return piece;
    }

    // Returns the number of [rows, columns]
    public int[] GetBoardDimensions()
    {
        return new int[] {_boardDepth, _boardWidth };
    }
    
    /// <summary>
    /// Create a chess piece at the specified location.
    /// </summary>
    /// <param name="type">The type of the piece.</param>
    /// <param name="pos">The position of the piece.</param>
    /// <param name="team">The faction of the piece.</param>
    /// <returns>The newly created chess piece.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The piece type or team is invalid.
    /// </exception>
    public ChessPiece CreatePiece(ChessPieceType type, IndexCode pos, Team team)
        => CreatePiece(type, ((int)pos % 8, (int)pos / 8), team);

    /// <summary>
    /// Create a chess piece at the specified location.
    /// </summary>
    /// <param name="type">The type of the piece.</param>
    /// <param name="x">The file of the piece.</param>
    /// <param name="y">The rank of the piece.</param>
    /// <param name="team">The faction of the piece.</param>
    /// <returns>The newly created chess piece.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The piece type or team is invalid.
    /// </exception>
    public ChessPiece CreatePiece(ChessPieceType type, int x, int y, Team team)
        => CreatePiece(type, (x, y), team);

    /// <summary>
    /// Get the piece at the tile specified.
    /// </summary>
    /// <param name="x">The file of the tile.</param>
    /// <param name="y">The rank of the tile.</param>
    /// <returns>The piece at the tile, or null if there is none.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The coordinates are outside the board.
    /// </exception>
    public ChessPiece GetPiece(int x, int y)
    {
        CheckIndex(x, y);

        if (_pieces.TryGetValue((x, y), out var piece))
        {
            return piece;
        }

        return null;
    }

    /// <summary>
    /// Get the piece at the tile specified.
    /// </summary>
    /// <param name="pos">The position of the tile.</param>
    /// <returns>The piece at the tile, or null if there is none.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The coordinates are outside the board.
    /// </exception>
    public ChessPiece GetPiece((int x, int y) pos)
        => GetPiece(pos.x, pos.y);

    /// <summary>
    /// Move a piece to the specified tile.
    /// </summary>
    /// <param name="src">The tile of the chess piece.</param>
    /// <param name="dst">The tile to move to.</param>
    /// <returns>Whether the move succeeded.</returns>
    /// <exception cref="ArgumentNullException">
    /// The piece to move does not exist.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either of the positions are outside the board.
    /// </exception>
    public bool MovePiece((int x, int y) src, (int, int) dst)
    {
        CheckIndex(src);
        CheckIndex(dst);

        var piece = GetPiece(src) ?? throw new ArgumentNullException(
                $"attempt to move nonexistent piece at (${src.x}, ${src.y})"
            );

        // check for existing piece at destination
        var capture = GetPiece(dst);
        if (capture != null)
        {
            // destination is occupied, try to capture
            if (capture.Team == piece.Team)
                return false; // friendly fire is disallowed
            _pieces.Remove(dst);
            // TODO: Animate
            Destroy(capture.gameObject);
        }

        // clear previous board square chess piece reference
        var srcTile = GetTile(src);
        srcTile.Clear();
        
        // do the logical move
        _pieces.Remove(src);
        _pieces.Add(dst, piece);

        // update visual location
        var dstTile = GetTile(dst);
        piece.MoveToBlock(dstTile);
        return true;
    }

    /// <summary>
    /// Move a piece to the specified tile.
    /// </summary>
    /// <param name="src">The tile of the chess piece.</param>
    /// <param name="x">The file of the tile to move to.</param>
    /// <param name="y">The rank of the tile to move to.</param>
    /// <returns>Whether the move succeeded.</returns>
    /// <exception cref="ArgumentNullException">
    /// The piece to move does not exist.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either of the positions are outside the board.
    /// </exception>
    public bool MovePiece((int, int) src, int x, int y)
        => MovePiece(src, (x, y));

    /// <summary>
    /// Move a piece to the specified tile.
    /// </summary>
    /// <param name="x">The file of the chess piece.</param>
    /// <param name="y">The rank of the chess piece.</param>
    /// <param name="dst">The tile to move to.</param>
    /// <returns>Whether the move succeeded.</returns>
    /// <exception cref="ArgumentNullException">
    /// The piece to move does not exist.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either of the positions are outside the board.
    /// </exception>
    public bool MovePiece(int x, int y, (int, int) dst)
        => MovePiece((x, y), dst);

    /// <summary>
    /// Move a piece to the specified tile.
    /// </summary>
    /// <param name="srcX">The file of the chess piece.</param>
    /// <param name="srcY">The rank of the chess piece.</param>
    /// <param name="dstX">The file of the tile to move to.</param>
    /// <param name="dstY">The rank of the tile to move to.</param>
    /// <returns>Whether the move succeeded.</returns>
    /// <exception cref="ArgumentNullException">
    /// The piece to move does not exist.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either of the positions are outside the board.
    /// </exception>
    public bool MovePiece(int srcX, int srcY, int dstX, int dstY)
        => MovePiece((srcX, srcY), (dstX, dstY));


    public bool DestroyPiece(ChessPiece piece)
    {
        //bool s = _pieces.Remove((piece.AssignedSquare.IndexX, piece.AssignedSquare.IndexZ ));
        ServiceLocator.GetService<UnitOrderTimelineController>().RemoveTimelineNode(piece.TimelineNode);
        bool s = ListofPieces.Remove(piece);
        ServiceLocator.GetService<ExecutionOrderManager>().DeletePiece(piece);
        return s;
    }

    public int GetDistanceBetweenTiles(BoardSquare t1, BoardSquare t2)
    {
        //TODO: Implement
        return 0;
    }
    

    // --------------- Private Helpers ---------------

    // Returns true if the index is outside the bounds of the chess board
    private void CheckIndex(int x, int y)
    {
        if (x >= _boardDepth || y >= _boardWidth)
        {
            throw new ArgumentOutOfRangeException($"Tile (${x}, ${y}) is out of range");
        }
    }

    private void CheckIndex((int x, int y) pos)
        => CheckIndex(pos.x, pos.y);

    public void DisableAllPieceLights()
    {
        foreach (var p in ListofPieces)
        {
            if (p != SelectedUnit)
            {
                p.SetLightEnabled(false);
            }
        }
    }
    
    public void EnableAllPieceLights()
    {
        foreach (var p in ListofPieces)
        {
            p.SetLightEnabled(true);
        }
    }

}

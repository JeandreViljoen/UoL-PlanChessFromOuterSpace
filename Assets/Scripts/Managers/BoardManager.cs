﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DG.Tweening;
using Services;
using UnityEngine;

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

    // BoardSquare Settings
    [Header("Board Squares")]
    [SerializeField, Tooltip("Prefab that is used for the board squares.")]
    private GameObject _squareBoardPrefab;
    [SerializeField, Tooltip("Materials for the board squares")]
    private Material[] _materialsArray;

    // Chess pieces information
    [Header("Chess Pieces Information and Properties")]
    [SerializeField, Tooltip("Pawn Chess Piece")]
    private GameObject _pawnPrefab;

    // Tweens
    private Tween _bounceAnimate;

    // Services
    private EasyService<GameStateManager> _gameStateManager;

    void Start()
    {
        _boardSquares = new List<BoardSquare>();
        _pieces = new Dictionary<(int, int), ChessPiece>();

        GenerateChessBoard();

        // Pawn creation for testing
        //CreatePiece(ChessPieceType.Pawn, 4, 4, Team.Friendly);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreatePiece(ChessPieceType.Pawn, IndexCode.A8, Team.Friendly);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            CreatePiece(ChessPieceType.Pawn, (0, 7), Team.Enemy);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MovePiece((7, 0), (0, 7));
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

        Vector3 squareBoardSize = _squareBoardPrefab.GetComponent<Renderer>().bounds.size;
        Vector3 firstSquarePosition = new Vector3(_centerPosition.x - squareBoardSize.x * _boardDepth, 0,
            _centerPosition.z - squareBoardSize.z * _boardWidth);

        // Create the game objects based on the prefab assigned
        Vector3 squareBoardPosition = firstSquarePosition;
        int materialIteration = 0;
        for (int i = 0; i < _boardDepth; ++i)
        {
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

                // Add the newly created squareBoard to the list containing all board squares
                _boardSquares.Add(boardSquareComponent);

                // Add material to the square board
                MeshRenderer meshRenderer = squareBoard.GetComponent<MeshRenderer>();
                if (materialIteration >= _materialsArray.Length)
                    materialIteration = 0;
                Material materialToApply = _materialsArray[materialIteration];
                meshRenderer.material = materialToApply;
                materialIteration++;

                // Update new X position
                squareBoardPosition.x += squareBoardSize.x;

                if (GlobalDebug.Instance.PopulateBoardOnStart)
                {
                    CreatePiece(ChessPieceType.Pawn, i, j, Team.Friendly);
                }

            }
            // Reset X position
            squareBoardPosition.x = firstSquarePosition.x;

            // Update Z position
            squareBoardPosition.z += squareBoardSize.z;
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

        ChessPiece piece;

        //Instantiate given piece
        switch (type)
        {
            case ChessPieceType.Pawn:
                piece = Instantiate(_pawnPrefab).GetComponent<ChessPiece>();
                break;
            case ChessPieceType.Knight:
                throw new NotImplementedException();
            case ChessPieceType.Bishop:
                throw new NotImplementedException();
            case ChessPieceType.Rook:
                throw new NotImplementedException();
            case ChessPieceType.Queen:
                throw new NotImplementedException();
            case ChessPieceType.King:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (piece == null)
            throw new NullReferenceException($"Prefab for piece type ${type} is broken");

        // TODO: More initialisation required on the chess piece
        piece.gameObject.transform.position = square.CenterSurfaceTransform.position;
        piece.Team = team;
        square.ChessPieceAssigned = piece;

        _pieces.Add(pos, piece);
        return piece;
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

    //TEMP - DELETE THIS LATER
    private IEnumerator DelayedBounce()
    {
        foreach (var block in _boardSquares)
        {
            block.BounceAnimate();
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void BounceSquaresOnStateChange(GameState state)
    {
        StartCoroutine(DelayedBounce());
    }

}
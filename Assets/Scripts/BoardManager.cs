using System;
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
    public List<ChessPiece> TotalChessPiecesOnBoard;
    public List<ChessPiece> FriendlyPiecesOnBoard { get; private set; }
    public List<ChessPiece> EnemyPiecesOnBoard { get; private set; }

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
        // Initialize unassigned member variables
        _boardSquares = new List<BoardSquare>();
        // Chess Board Initialization
        GenerateChessBoard();
        
        // Pawn creation for testing
        CreatePiece(ChessPieceType.Pawn, 4, 4, Team.Friendly);
        
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CreatePiece(ChessPieceType.Pawn, IndexCode.A8, Team.Friendly);
        }
        
        if (Input.GetKeyDown(KeyCode.M))
        {
            GetTile(IndexCode.A8).ChessPieceAssigned.MoveToBlock(GetTile(IndexCode.H1));
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
    /// <exception cref="IndexOutOfRangeException">
    /// The position is outside the chessboard, or the tile does not exist.
    /// </exception>
    public BoardSquare GetTile(int x, int y)
    { 
        if (IndexOutsideBounds(x, y))
            throw new IndexOutOfRangeException($"Tile (${x}, ${y}) is out of range");

        BoardSquare querySquareBoard =
            _boardSquares.Find((square => square.IndexX == x && square.IndexZ == y));

        // If for some reason the square board does not exist, notify user and return a null object
        if (querySquareBoard == null)
            throw new IndexOutOfRangeException($"Tile object at (${x}, ${y}) does not exist");

        return querySquareBoard;
    }
    
    
    /// <summary>
    /// Get a tile with the specified position.
    /// </summary>
    /// <param name="code">The position code of the tile.</param>
    /// <returns>The tile at the specified location.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// The position is invalid, or the tile does not exist.
    /// </exception>
    public BoardSquare GetTile(IndexCode code)
        => GetTile((int)code % 8, (int)code / 8);
    
    
    /// <summary>
    /// Get a tile with the specified position.
    /// </summary>
    /// <param name="tile">The position of the tile.</param>
    /// <returns>The tile at the specified location.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// The position is outside the chessboard, or the tile does not exist.
    /// </exception>
    public BoardSquare GetTile((int, int) tile)
        => GetTile(tile.Item1, tile.Item2);

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
    public ChessPiece CreatePiece(ChessPieceType type, (int, int) pos, Team team)
    {

        //Get square to spawn at
        BoardSquare square = GetTile(pos);

        ChessPiece newPiece;

        //Instantiate given piece
        switch (type)
        {
            case ChessPieceType.Pawn:
                newPiece = Instantiate(_pawnPrefab).GetComponent<ChessPiece>();
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
        
        if (newPiece == null)
            throw new NullReferenceException($"Prefab for piece type ${type} is broken");

        //Set new piece to squares surface position and assign piece to square
        newPiece.gameObject.transform.position = square.CenterSurfaceTransform.position;
        square.ChessPieceAssigned = newPiece;
        
        //TODO: More initialisation required on the chess piece

        //Add piece to list depending on which team it is on
        switch (team)
        {
            case Team.Friendly:
                FriendlyPiecesOnBoard.Add(newPiece);
                break;
            case Team.Enemy:
                EnemyPiecesOnBoard.Add(newPiece);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(team), team, null);
        }

        return newPiece;
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

    // --------------- Private Helpers ---------------
    // Returns true if index is within the bounds of the chess board
    private bool IndexWithinBounds(int xIndex, int zIndex)
    {
        return !(xIndex >= _boardDepth || zIndex >= _boardWidth);
    }
    
    // Returns true if the index is outside the bounds of the chess board
    private bool IndexOutsideBounds(int xIndex, int zIndex)
    {
        return (xIndex >= _boardDepth || zIndex >= _boardWidth);
    }

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

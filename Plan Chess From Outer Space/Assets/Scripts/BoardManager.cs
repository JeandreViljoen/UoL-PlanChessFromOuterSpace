using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BoardManager : MonoBehaviour
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
    
    void Start()
    {
        // Initialize unassigned member variables
        _boardSquares = new List<BoardSquare>();
        // Chess Board Initialization
        GenerateChessBoard();
        
        // Pawn creation for testing
        CreateChessPieceAtIndex(4, 4, ChessPieceType.Pawn);
    }
    
    void Update()
    {
        // Empty for now
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
    
    // Returns the corresponding square board given X and Z values
    public BoardSquare GetSquareBoardByIndex(int xIndex, int zIndex)
    { 
        // Notify the user if the query is out of bounds
        if (IndexOutsideBounds(xIndex, zIndex))
        {
            Debug.Log("The index provided are out of the bounds of the chess board.");
            return null;
        }

        BoardSquare querySquareBoard =
            _boardSquares.Find((square => square.IndexX == xIndex && square.IndexZ == zIndex));

        // If for some reason the square board does not exist, notify user and return a null object
        if (querySquareBoard == null)
        {
            Debug.Log("There is no square board with index X = " + xIndex.ToString() + ", and index Z = " + zIndex.ToString());
            return null;
        }

        return querySquareBoard;
    }

    // Creates a chess piece at a given location. Returns true if the piece was successfully created in the board
    public bool CreateChessPieceAtIndex(int xIndex, int zIndex, ChessPieceType pieceType)
    {
        // If position is not valid, chess piece cannot be created
        if (IndexOutsideBounds(xIndex, zIndex))
        {
            Debug.Log("Chess piece cannot be created because the position given is invalid in the chess board.");
            return false;
        }

        BoardSquare squareBoard = GetSquareBoardByIndex(xIndex, zIndex);
        
        // Handle exception if the square board could not be queried for some reason
        if (!squareBoard)
        {
            Debug.Log("Chess piece cannot be created because querying by index was not successful.");
            return false;
        }
        
        // TODO: Create chess piece, its properties and add it to the given square board
        if (squareBoard.IsEmpty())
        {
            // Set variables to create the chess piece in an appropriate position
            Vector3 squareBoardSize = _squareBoardPrefab.GetComponent<Renderer>().bounds.size;
            Vector3 squareBoardPosition = squareBoard.transform.position;
            Vector3 newPiecePosition = new Vector3(squareBoardPosition.x,
                squareBoardPosition.y + squareBoardSize.y / 2, squareBoardPosition.z);
            switch (pieceType)
            {
                case ChessPieceType.Pawn:
                    GameObject newPiece = Instantiate(_pawnPrefab);
                    newPiece.transform.position = newPiecePosition;
                    break;
                default:
                    // Do nothing
                    break;
            }
        }


        // Creation of the chess piece was successful
        return true;
    }
    
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
    
}

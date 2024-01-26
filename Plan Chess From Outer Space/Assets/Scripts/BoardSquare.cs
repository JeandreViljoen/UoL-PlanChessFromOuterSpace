using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{

    // --------------- Member variables and data --------------- //
    public int IndexX;
    public int IndexZ;
    public GameObject ChessPieceAssigned;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
    
    // --------------- Public Functions and Methods ---------------
    
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
}

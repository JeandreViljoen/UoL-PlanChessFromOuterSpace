using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

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
            _chessPieceAssigned.IndexCodePosition = _indexCode;
        }
    }

    public TextMeshPro IndexCodeTextField;
    public Transform CenterSurfaceTransform;
    
    private Tween _bounceAnimateTween;

    private IndexCode _indexCode;
    

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

    void Start()
    {
        if (GlobalDebug.Instance.ShowIndexCodes)
        {
            IndexCodeTextField.gameObject.SetActive(true);
        }
        else
        {
            IndexCodeTextField.gameObject.SetActive(false);
        }
        
        _bounceAnimateTween = transform.DOMove(transform.localPosition, 0.2f).SetEase(Ease.InOutSine);
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
    
    //TEMP
    public void BounceAnimate()
    {
        if (_bounceAnimateTween.IsPlaying())
        {
            return;
        }
        
        
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMove( transform.position + Vector3.up *3 , 1f).SetEase(Ease.InOutSine));
        s.Append(transform.DOMove( transform.position , 1f).SetEase(Ease.InOutSine));

        _bounceAnimateTween = s;
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

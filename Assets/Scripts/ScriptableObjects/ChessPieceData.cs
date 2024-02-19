using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChessPieceData", menuName = "Custom Assets/New Chess Piece Data")]
public class ChessPieceData : ScriptableObject
{
   public ChessPieceType PieceType;
   public Sprite Sprite;
   public int DefaultSpeed = 1;
   public int DefaultRange = 2;
   public List<Vector2> BaseRelativeMoveset; // [ (0,1) , (1,0) , (-1,0), (0,-1) ]
   public List<Vector2> UniqueRelativeAttackSet; // [ (0,1) , (1,0) , (-1,0), (0,-1) ]
   
}

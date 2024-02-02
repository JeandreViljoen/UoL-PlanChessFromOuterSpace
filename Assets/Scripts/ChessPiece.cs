using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanChess
{
    // Enum containing all the possible chess pieces
    public enum ChessPieceType
    {
        Pawn,
        Knight,
        Bishop
    }

    public class ChessPiece : MonoBehaviour
    {
        // --------------- Member variables and data --------------- //
        public ChessPieceType PieceType;
        public int Speed = 1;
        public int Level = 1;
        public int Range = 2;
        public List<Vector2> RelativeMoveset;



        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}


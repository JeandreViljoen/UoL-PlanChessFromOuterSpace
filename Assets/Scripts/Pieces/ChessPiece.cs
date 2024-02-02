using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanChess
{
    /// <summary>
    /// Denotes a chess piece.
    /// </summary>
    public abstract class ChessPiece : MonoBehaviour
    {
        /// <summary>
        /// The priority of the piece.
        /// </summary>
        public int Speed = 1;
        /// <summary>
        /// The attack and movement range of the piece.
        /// </summary>
        public int Range = 1;

        /// <summary>
        /// The game board the piece belongs to.
        /// </summary>
        public BoardManager board;

        /// <summary>
        /// The file of the piece, with A being 0 and H being 7.
        /// </summary>
        public int x;
        /// <summary>
        /// The rank of the piece, with 0 being rank 1 in algebraic
        /// notation.
        /// </summary>
        public int y;

        /// <summary>
        /// Get a list of squares the piece can move to.
        /// </summary>
        public abstract IEnumerable<(int, int)> GetMoves { get; }
    }
}


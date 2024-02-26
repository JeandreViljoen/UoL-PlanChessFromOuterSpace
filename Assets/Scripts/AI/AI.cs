using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI
{
    private IEnumerable<ChessPiece> moveQueue;

    /// <summary>
    /// Restart AI computation.
    /// </summary>
    /// <param name="board">The state of the board.</param>
    /// <param name="moveQueue">The move order.</param>
    public void SyncAI(BoardManager board,
        IEnumerable<ChessPiece> moveQueue)
    {
        this.moveQueue = moveQueue;
    }

    private int ScoreCapture(ChessPieceType pieceType, AIBalanceData scores)
    {
        switch (pieceType)
        {
            case ChessPieceType.Pawn: return scores.PawnValue;
            case ChessPieceType.Knight: return scores.KnightValue;
            case ChessPieceType.Bishop: return scores.BishopValue;
            case ChessPieceType.Rook: return scores.RookValue;
            case ChessPieceType.Queen: return scores.QueenValue;
            case ChessPieceType.King: return scores.KingValue;
        }

        return 0;
    }

    private class Move : IComparable<Move> {
        public BoardSquare destination;
        public int score;

        public Move(BoardSquare destination, int score) {
            this.destination = destination;
            this.score = score;
        }

        public int CompareTo(Move other)
        {
            return score.CompareTo(other.score);
        }
    }

    /// <summary>
    /// Recommend a move for the current turn.
    /// </summary>
    /// <param name="board">The state of the board.</param>
    /// <returns></returns>
    public BoardSquare RecommendMove(BoardManager board)
    {
        if (moveQueue.Count() == 0)
            throw new ArgumentException("no moves");

        var piece = moveQueue.First();
        var destinations = piece.GetAllPossibleMovesetTiles();

        var balanceData = GlobalGameAssets.Instance.AIBalanceData;

        var moves = new SortedSet<Move>();

        foreach (var dst in destinations)
        {
            var capture = dst.ChessPieceAssigned;
            int score = 0;

            if (capture)
            {
                score = ScoreCapture(capture.PieceType, balanceData);
            }

            moves.Add(new Move(dst, score));
        }

        int highscore = moves.First().score;
        var top = moves.TakeWhile((m) => m.score == highscore).ToArray();
        int rand = Random.Range(0, 1 << 24) % top.Length;
        return top[rand].destination;
    }
}

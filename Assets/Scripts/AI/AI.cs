using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : MonoService
{
    private IEnumerable<ChessPiece> moveQueue;

    private EasyService<BoardManager> currentBoard;

    private AIBalanceData Balance => GlobalGameAssets.Instance.AIBalanceData;

    /// <summary>
    /// Restart AI computation.
    /// </summary>
    /// <param name="moveQueue">The move order.</param>
    public void SyncAI(IEnumerable<ChessPiece> moveQueue)
    {
        this.moveQueue = moveQueue;
    }

    private int ScoreCapture(ChessPieceType pieceType)
    {
        switch (pieceType)
        {
            case ChessPieceType.Pawn: return Balance.PawnValue;
            case ChessPieceType.Knight: return Balance.KnightValue;
            case ChessPieceType.Bishop: return Balance.BishopValue;
            case ChessPieceType.Rook: return Balance.RookValue;
            case ChessPieceType.Queen: return Balance.QueenValue;
            case ChessPieceType.King: return Balance.KingValue;
        }

        return 0;
    }

    private int Score(ChessPiece piece, BoardSquare dst)
    {
        int score = 0;
        (int x, int y) dstPos = (dst.IndexX, dst.IndexZ);

        // score based on distance to King
        if (piece.Team == Team.Enemy)
        {
            (int x, int y) = currentBoard.Value.Pieces.First(
                tile => tile.Value.PieceType == ChessPieceType.King
                        && tile.Value.Team == Team.Friendly
            ).Key;
            int distance = Math.Max(
                Math.Abs(x - dstPos.x),
                Math.Abs(y - dstPos.y));
            if (distance == 0) score += Balance.KingValue;
            else score += 8 - distance;
        }

        // score piece capture
        var capture = dst.ChessPieceAssigned;
        if (capture)
        {
            score += ScoreCapture(capture.PieceType);
        }

        // score based on danger
        foreach (var enemy in currentBoard.Value.Pieces.Values.Where(
            enemy => enemy.Team != piece.Team))
        {
            if (enemy.GetAllPossibleMovesetTiles().Contains(dst)) {
                score -= ScoreCapture(piece.PieceType);
                break;
            }
        }

        return score;
    }

    /// <summary>
    /// Recommend a move for the current turn.
    /// </summary>
    /// <returns>The recommended tile to move to.</returns>
    public BoardSquare RecommendMove()
    {
        if (moveQueue.Count() == 0)
            throw new ArgumentException("no piece to move");

        var piece = moveQueue.First();
        var destinations = piece.GetAllPossibleMovesetTiles();

        var moves = new SortedSet<Move>();

        foreach (var dst in destinations)
        {
            int score = Score(piece, dst);

            moves.Add(new Move(dst, score));
        }

        int highscore = moves.Reverse().First().score;
        var top = moves.Reverse().TakeWhile(m => m.score == highscore).ToArray();

        return top[Random.Range(0, top.Length)].destination;
    }

    private struct Move : IComparable<Move>
    {
        public BoardSquare destination;
        public int score;

        public Move(BoardSquare destination, int score)
        {
            this.destination = destination;
            this.score = score;
        }

        public int CompareTo(Move other)
        {
            int result = score.CompareTo(other.score);
            if (result != 0) return result;
            result = destination.IndexX.CompareTo(other.destination.IndexX);
            if (result != 0) return result;
            return destination.IndexZ.CompareTo(other.destination.IndexZ);
        }
    }
}

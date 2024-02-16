using System.Collections.Generic;

public class PiecePawn : ChessPiece
{
    public override IEnumerable<(int, int)> GetMoves(BoardManager board, (int x, int y) pos)
    {
        var (_, forward) = ViewOffsetToAbsolute((0, -1));

        // moves forward
        if (!(board.GetPiece(pos.x, pos.y + forward)))
            yield return (pos.x, pos.y + forward);

        // attacks diagonally
        if (board.GetPiece(pos.x - 1, pos.y + forward))
            yield return (pos.x - 1, pos.y + forward);
        if (board.GetPiece(pos.x + 1, pos.y + forward))
            yield return (pos.x + 1, pos.y + forward);
    }
}

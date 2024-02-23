using System;
using Services;
using UnityEngine;

class BoardTest : MonoBehaviour
{
    private EasyService<BoardManager> boardService;

    private bool started = false;

    private void ExecuteDebugCode()
    {
        BoardManager board = boardService.Value;
        
        if (GlobalDebug.Instance.PopulateBoardOnStart)
        {
            for (int i = 0; i < board.Depth; ++i)
            {
                for (int j = 0; j < board.Width; ++j)
                {
                    float rng = UnityEngine.Random.Range(0f, 1f);

                    if (rng <= GlobalDebug.Instance.ChanceToPopulateTile)
                    {
                        int randomPiece = UnityEngine.Random.Range(0, 5);
                            
                        ChessPieceType type = (ChessPieceType) Enum.ToObject(typeof(ChessPieceType), randomPiece);

                        Team randomTeam;
                        if (UnityEngine.Random.Range(0,2) == 0)
                        {
                            randomTeam = Team.Friendly;
                        }
                        else
                        {
                            randomTeam = Team.Enemy;
                        }
                    
                        ChessPiece piece = board.CreatePiece(type, i, j, randomTeam);
                    
                    
                        piece.Speed = UnityEngine.Random.Range(1, 5);
                        Debug.Log($"{piece.Team} {piece.PieceType} | Speed : {piece.Speed}");
                    }
                }
            }
        }
    }

    void Update()
    {
        BoardManager board = boardService.Value;

        if (!started) {
            ExecuteDebugCode();
            started = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            board.CreatePiece(ChessPieceType.Rook, (5, 4), Team.Friendly);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            board.CreatePiece(ChessPieceType.Bishop, (4, 4), Team.Friendly);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CreatePiece(ChessPieceType.Pawn, (4, 4), Team.Friendly);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            board.CreatePiece(ChessPieceType.King, (4, 4), Team.Friendly);
        }


        if (Input.GetKeyDown(KeyCode.N))
        {
            board.CreatePiece(ChessPieceType.Queen, (4, 4), Team.Friendly);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            board.CreatePiece(ChessPieceType.Knight, (4, 4), Team.Friendly);
        }

        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     board.MovePiece((7, 0), (0, 7));
        // }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

public class ExecutionOrderManager : MonoService
{
    private EasyService<BoardManager> _boardManager;
    public IEnumerable<ChessPiece> UnitOrderList;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void RefreshTimelineOrder()
    {
        UnitOrderList = _boardManager.Value.Pieces.Values
            .OrderByDescending(piece => piece.Speed)
            .ThenBy(piece => piece.Team);
    }
}

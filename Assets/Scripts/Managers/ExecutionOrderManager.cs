using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

public class ExecutionOrderManager : MonoService
{
    private EasyService<BoardManager> _boardManager;
    public IEnumerable<ChessPiece> UnitOrderList;

    public event Action OnTimeLineRefresh;

    private int _currentActiveUnit = -1;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RefreshTimelineOrder();
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            _currentActiveUnit++;
            if (_currentActiveUnit < UnitOrderList.ToList().Count)
            {
                UnitOrderList.ToList()[_currentActiveUnit].State = ChessPieceState.START;
            }
            else
            {
                _currentActiveUnit = 0;
                UnitOrderList.ToList()[_currentActiveUnit].State = ChessPieceState.START;
            }
        }
    }

    public void RefreshTimelineOrder()
    {
        //Debug.Log($"Atttempting refresh of Timeline: Pieces.Count = {_boardManager.Value.ListofPieces.Count}");
        
        UnitOrderList = _boardManager.Value.ListofPieces
            .OrderByDescending(piece => piece.Speed)
            .ThenBy(piece => piece.Team);
        
        OnTimeLineRefresh?.Invoke();
    }

    public void AdvanceQueue()
    {
        _currentActiveUnit++;
        if (_currentActiveUnit < UnitOrderList.ToList().Count)
        {
            UnitOrderList.ToList()[_currentActiveUnit].State = ChessPieceState.START;
        }
    }
    
}

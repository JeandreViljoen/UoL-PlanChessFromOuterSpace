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
        }
            
    }

    public void RefreshTimelineOrder()
    {
        UnitOrderList = _boardManager.Value.Pieces.Values
            .OrderByDescending(piece => piece.Speed)
            .ThenBy(piece => piece.Team);
        
        OnTimeLineRefresh?.Invoke();
    }
    
    
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

public class ExecutionOrderManager : MonoService
{
    private EasyService<BoardManager> _boardManager;
    public List<ChessPiece> UnitOrderList;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void RefreshTimelineOrder()
    {
        List<ChessPiece> friendlyUnitList = _boardManager.Value.FriendlyPiecesOnBoard;
        List<ChessPiece> orderedFriendlyUnitList = friendlyUnitList.OrderByDescending( unit => unit.Speed).ToList();
        
        List<ChessPiece> enemyUnitList = _boardManager.Value.EnemyPiecesOnBoard;
        List<ChessPiece> orderedEnemyUnitList = enemyUnitList.OrderByDescending(unit => unit.Speed).ToList();

        List<ChessPiece> unitOrderList = new List<ChessPiece>();
        
        //TODO: sorting logic between lists

        
    }
}

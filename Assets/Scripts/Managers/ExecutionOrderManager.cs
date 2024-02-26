using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

public class ExecutionOrderManager : MonoService
{
    private EasyService<BoardManager> _boardManager;
    private EasyService<GameStateManager> _stateManager;
    public IEnumerable<ChessPiece> UnitOrderList;

    public event Action OnTimeLineRefresh;
    public event Action OnTimeLineInit;
    public event Action OnCombatComplete;

    private int _currentActiveUnit = -1;

    void Start()
    {
        _stateManager.Value.OnStateChanged += StateChangeLogic;
        ServiceLocator.GetService<TransitionController>().OnTransitionStart += StateTransitionLogic;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            RefreshTimelineOrder();
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
        else
        {
            OnCombatComplete?.Invoke();
            _stateManager.Value.GameState = GameState.PREP;
        }
    }

    private void StateChangeLogic(GameState state)
    {
        switch (state)
        {
            case GameState.TRANSITION:
                break;
            case GameState.START:
                break;
            case GameState.PREP:
                //StartPrep();
                break;
            case GameState.COMBAT:
                StartCombat();
                break;
            case GameState.WIN:
                break;
            case GameState.LOSE:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        
    }
    
    private void StateTransitionLogic(GameState state)
    {
        switch (state)
        {
            case GameState.TRANSITION:
                break;
            case GameState.START:
                break;
            case GameState.PREP:
                StartPrep();
                break;
            case GameState.COMBAT:
                break;
            case GameState.WIN:
                break;
            case GameState.LOSE:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        
    }

    private void StartCombat()
    {
        ServiceLocator.GetService<CameraManager>().ResetCameraPosition();
        ServiceLocator.GetService<UnitOrderTimelineController>().NodeOffset = 0;
        _currentActiveUnit = 0;
        UnitOrderList.ToList()[0].State = ChessPieceState.START;
    }

    private void StartPrep()
    {
        RefreshTimelineOrder();
        OnTimeLineInit?.Invoke();
        //TODO: SHow shop

    }
}

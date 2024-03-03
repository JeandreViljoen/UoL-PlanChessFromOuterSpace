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
    private EasyService<AI> _ai;
    public IEnumerable<ChessPiece> UnitOrder => _unitOrder;

    public event Action OnTimeLineRefresh;
    public event Action OnTimeLineInit;
    public event Action OnCombatComplete;

    private LinkedList<ChessPiece> _unitOrder;
    private LinkedListNode<ChessPiece> _currentActiveUnit;

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
        if (_unitOrder == null) _unitOrder = new LinkedList<ChessPiece>();
        
        _unitOrder.Clear();
        foreach (var piece in _boardManager.Value.ListofPieces
            .OrderByDescending(piece => piece.Speed)
            .ThenBy(piece => piece.Team))
        {
            _unitOrder.AddLast(piece);
        }

        OnTimeLineRefresh?.Invoke();
    }

    public void AdvanceQueue()
    {
        _currentActiveUnit = _currentActiveUnit.Next;
        if (_currentActiveUnit != null)
        {
            _currentActiveUnit.Value.State = ChessPieceState.START;
            _ai.Value.SyncAI(_currentActiveUnit.Forward());
        }
        else
        {
            OnCombatComplete?.Invoke();
            _stateManager.Value.GameState = GameState.SPAWN;
        }
    }

    public void DeletePiece(ChessPiece piece)
    {
        _unitOrder.Remove(piece);
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
                ServiceLocator.GetService<HUDManager>().ShopMenu.Show();
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
                _boardManager.Value.CancelBuyUnit();
                ServiceLocator.GetService<HUDManager>().ShopMenu.Hide();
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
        _currentActiveUnit = _unitOrder.First;
        _ai.Value.SyncAI(UnitOrder);
        _currentActiveUnit.Value.State = ChessPieceState.START;
    }

    private void StartPrep()
    {
        RefreshTimelineOrder();
        OnTimeLineInit?.Invoke();
        //TODO: SHow shop

    }
}

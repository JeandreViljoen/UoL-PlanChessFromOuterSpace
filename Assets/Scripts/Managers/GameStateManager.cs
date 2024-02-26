using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

public class GameStateManager : MonoService
{
    private GameState _gameState;

    /// <summary>
    /// The interval between each move in seconds.
    /// </summary>
    public int thinkTime;

    private AI _ai;
    private int _thinkTimeLeft;

    private EasyService<BoardManager> _board;
    private EasyService<ExecutionOrderManager> _moveOrder;
    private Queue<ChessPiece> _queue;

    /// <summary>
    /// Queue up a new set of moves.
    /// </summary>
    private void QueueMoves()
    {
        foreach (var piece in _moveOrder.Value.UnitOrderList.Prepend(null))
            _queue.Enqueue(piece);
    }

    /// <summary>
    /// Shift the queue so the head is a live chess piece.
    /// </summary>
    private void ShiftQueue()
    {
        while (true)
        {
            if (_queue.First() == null)
            {
                QueueMoves();
                _queue.Dequeue();
                continue;
            }
            if (_queue.First().Dead)
            {
                Destroy(_queue.First());
                _queue.Dequeue();
                continue;
            }

            return;
        }
    }

    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            if (_gameState == value)
            {
                Debug.LogWarning($"[GameStateManager.cs] - : Attempted to set the game state to {value} but the GameState is already set to {value}. Returning early without executing state logic.");
                return;
            }

            _gameState = value;

            switch (_gameState)
            {
                case GameState.START:
                    break;
                case GameState.PREP:
                    break;
                case GameState.COMBAT:
                    // prepend null for concatenation
                    QueueMoves();
                    ShiftQueue();
                    _thinkTimeLeft = thinkTime;
                    break;
                case GameState.WIN:
                    break;
                case GameState.LOSE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_gameState), _gameState, null);
            }

            OnStateChanged?.Invoke(_gameState);
        }
    }

    public event Action<GameState> OnStateChanged;

    void Start()
    {
    }

    void Update()
    {
        switch (_gameState)
        {
            case GameState.WIN:
            case GameState.LOSE:
                break;
            case GameState.COMBAT:
                if (_thinkTimeLeft == 0)
                {
                    var dst = _ai.RecommendMove(_board.Value);
                    var piece = _queue.Dequeue();

                    piece.MoveToBlock(dst);

                    ShiftQueue();
                    _ai.SyncAI(_board.Value, _queue);
                }
                break;
            default:
                break;
        }
    }

}

public enum GameState
{
    START,
    PREP,
    COMBAT,
    WIN,
    LOSE
}

/*
TODO:
Set up data objects for different chess pieces and figure out movement mechanics + range upgrades
















*/
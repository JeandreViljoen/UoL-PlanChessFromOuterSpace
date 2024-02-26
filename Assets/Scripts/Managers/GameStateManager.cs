using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

public class GameStateManager : MonoService
{
    private GameState _gameState;
    private float _stateChangeTime;
    private EasyService<TransitionController> _transitionController;

    /// <summary>
    /// The interval between each move in seconds.
    /// </summary>
    public float thinkTime;

    private AI _ai;
    private float _thinkTimeLeft;

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

            if (_gameState != GameState.TRANSITION)
            {
                _gameState = GameState.TRANSITION;
            }
            else
            {
                _gameState = value;
            }

            switch (_gameState)
            {
                case GameState.TRANSITION:
                    _transitionController.Value.Transition(value);
                    break;
                case GameState.START:
                    break;
                case GameState.SPAWN:
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
            _stateChangeTime = Time.time;
        }
    }

    public event Action<GameState> OnStateChanged;

    void Start()
    {
        GameState = GameState.PREP;
    }

    void Update()
    {
        switch (_gameState)
        {
            case GameState.WIN:
            case GameState.LOSE:
                break;
            case GameState.COMBAT:
                _thinkTimeLeft -= Time.deltaTime;
                if (_thinkTimeLeft <= 0)
                {
                    var dst = _ai.RecommendMove(_board.Value);
                    var piece = _queue.Dequeue();

                    piece.MoveToBlock(dst);

                    ShiftQueue();
                    _ai.SyncAI(_board.Value, _queue);

                    _thinkTimeLeft = thinkTime;
                }
                break;
            default:
                break;
        }
    }

    public float GetTimeSinceStateChange()
    {
        return Time.time - _stateChangeTime;
    }

}



public enum GameState
{
    START,
    SPAWN,
    PREP,
    COMBAT,
    WIN,
    LOSE,
    TRANSITION
}

/*
TODO:
Set up data objects for different chess pieces and figure out movement mechanics + range upgrades
















*/
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;

public class UnitOrderTimelineController : MonoService
{
    private EasyService<ExecutionOrderManager> _orderManager;
    private EasyService<GameStateManager> _stateManager;

    [SerializeField] private Transform TopSlot;
    [SerializeField] private List<Transform> NodeSlots;
    [SerializeField] private Transform BottomSlot;
    
    [SerializeField] private Transform TimelineNodePrefab;

    private List<TimelineNode> _nodes = new List<TimelineNode>();

    private int _nodeOffset;
    public int _maxNodeOffset;

    public int NodeOffset
    {
        get
        {
            return _nodeOffset;
        }
        set
        {
            
            setMaxOffset();

            int validatedOffset = 0;
            if (value > 0)
            {
                validatedOffset = Math.Min(value, _maxNodeOffset);
            }

            _nodeOffset = validatedOffset;
            
            RefreshTimelinePositions();
        }
    }

    void setMaxOffset()
    {
        switch (ServiceLocator.GetService<GameStateManager>().GameState)
        {
            case GameState.TRANSITION:
                _maxNodeOffset = _nodes.Count - NodeSlots.Count;
                break;
            case GameState.START:
                _maxNodeOffset = _nodes.Count - NodeSlots.Count;
                break;
            case GameState.SPAWN:
                _maxNodeOffset = _nodes.Count - NodeSlots.Count;
                break;
            case GameState.PREP:
                _maxNodeOffset = _nodes.Count - NodeSlots.Count;
                break;
            case GameState.COMBAT:
                _maxNodeOffset = _nodes.Count;
                break;
            case GameState.WIN:
                _maxNodeOffset = _nodes.Count - NodeSlots.Count;
                break;
            case GameState.LOSE:
                _maxNodeOffset = _nodes.Count - NodeSlots.Count;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (_maxNodeOffset < 0) _maxNodeOffset = 0;
    }

    void Start()
    {
        _orderManager.Value.OnTimeLineInit += InitTimeline;
    }
    
    void Update()
    {
     
        //Check mouse scroll to update offset index - Ignore changes if units are less than the amount of node slots on screen
        if (_stateManager.Value.GameState == GameState.PREP && _nodes.Count > NodeSlots.Count)
        {
            if (Input.mouseScrollDelta.y > 0f)
            {
                NodeOffset--;
            }
            else if (Input.mouseScrollDelta.y < 0f)
            {
                NodeOffset++;
            }
        }
       
    }

    public void InitTimeline()
    {
        ClearTimeline();

        foreach (ChessPiece unit in _orderManager.Value.UnitOrder)
        {
            TimelineNode node = Instantiate(TimelineNodePrefab, transform).GetComponent<TimelineNode>();
            node.transform.localPosition = BottomSlot.transform.localPosition;
            node.SetPiece(unit);
            _nodes.Add(node);
        }

        TimelineStartupAnimation();
    }

    private void TimelineStartupAnimation()
    {
        StartCoroutine(DelayedStartup());
    }

    private IEnumerator DelayedStartup()
    {
        for (int i = 0; i < NodeSlots.Count; i++)
        {
            if (i < _nodes.Count)
            {
                _nodes[i].MoveNode(NodeSlots[i], 0.5f);
                yield return new WaitForSeconds(0.2f);
            }
        }
        NodeOffset = 0;
    }

    private void ClearTimeline()
    {
        //TODO: Nice remove animation
        foreach (var node in _nodes)
        {
            Destroy(node.gameObject);
        }
        
        _nodes.Clear();
    }

    public void RefreshTimelinePositions()
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            //Move nodes offscreen top
            if (i < NodeOffset)
            {
                _nodes[i].MoveNode(TopSlot, 0.15f);
            }
            //Move nodes onto screen space slots
            else if (i -_nodeOffset <= NodeSlots.Count-1)
            {
                _nodes[i].MoveNode(NodeSlots[i -_nodeOffset], 0.15f);
            }
            //Move notes off screen, bottom
            else
            {
                _nodes[i].MoveNode(BottomSlot, 0.15f);
            }
        }
    }

    public void RefreshListIndices()
    {
        if (_stateManager.Value.GameState != GameState.PREP)
        {
            return;
        }

        for (int i = 0; i < _orderManager.Value.UnitOrder.ToList().Count; i++)
        {
            ChessPiece correctUnit = _orderManager.Value.UnitOrder.ToList()[i];
            
            if (correctUnit != _nodes[i].Piece)
            {
                _nodes.Remove(correctUnit.TimelineNode);
                _nodes.Insert(i, correctUnit.TimelineNode);
                
            } 
        }
        RefreshTimelinePositions();
    }

    public void AddNode(ChessPiece piece)
    {
        TimelineNode node = Instantiate(TimelineNodePrefab, transform).GetComponent<TimelineNode>();
        node.transform.localPosition = BottomSlot.transform.localPosition;
        node.SetPiece(piece);
        _nodes.Add(node);

        RefreshListIndices();
    }

    private IEnumerator DelayedRefreshTimelinePositions()
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            if (i < NodeOffset)
            {
                _nodes[i].MoveNode(TopSlot, 0.15f);
            }
            else if (i -_nodeOffset <= NodeSlots.Count-1)
            {
                _nodes[i].MoveNode(NodeSlots[i -_nodeOffset], 0.15f);
            }
            else
            {
                _nodes[i].MoveNode(BottomSlot, 0.15f);
            }
            yield return new WaitForSeconds(0.00f);
        }
    }
    
    public void RemoveTimelineNode(TimelineNode nodeToRemove)
    {
        StartCoroutine(DelayedKill(nodeToRemove));
    }

    private IEnumerator DelayedKill(TimelineNode nodeToRemove)
    {
        _nodes.Remove(nodeToRemove);
        nodeToRemove.KillAnimation(1f);
        yield return new WaitForSeconds(1.2f);
        RefreshTimelinePositions();
    }
}

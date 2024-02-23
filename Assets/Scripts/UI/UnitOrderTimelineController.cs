using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class UnitOrderTimelineController : MonoBehaviour
{
    private EasyService<ExecutionOrderManager> _orderManager;

    [SerializeField] private Transform TopSlot;
    [SerializeField] private List<Transform> NodeSlots;
    [SerializeField] private Transform BottomSlot;
    
    [SerializeField] private Transform TimelineNodePrefab;

    private List<TimelineNode> _nodes = new List<TimelineNode>();

    private int _nodeOffset;

    public int NodeOffset
    {
        get
        {
            return _nodeOffset;
        }
        set
        {
            int validatedOffset = 0;
            if (value > 0)
            {
                validatedOffset = Math.Min(value, _nodes.Count - NodeSlots.Count);
            }

            _nodeOffset = validatedOffset;
            
            RefreshTimelinePositions();
        }
    }

    void Start()
    {
        _orderManager.Value.OnTimeLineRefresh += InitTimeline;
    }
    
    void Update()
    {

        if (Input.mouseScrollDelta.y > 0f)
        {
            NodeOffset--;
        }
        else if (Input.mouseScrollDelta.y < 0f)
        {
            NodeOffset++;
        }
        // if (Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     NodeOffset--;
        // }
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     NodeOffset++;
        // }
    }

    public void InitTimeline()
    {
        ClearTimeline();
        
        foreach (ChessPiece unit in _orderManager.Value.UnitOrderList)
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
        StartCoroutine(DelayedRefreshTimelinePositions());
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
}

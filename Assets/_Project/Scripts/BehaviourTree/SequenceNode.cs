using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.BehaviourTree;
using UnityEngine;

/// <summary>
/// 모든 자식 노드들이 성공을 반환하면 성공, 실패 혹은 진행중인 상태가 1개라도 존재할 경우 바로 실패를 반환하는 노드
/// </summary>
public class SequenceNode : Node
{
    protected List<INode> child;
    
    public SequenceNode(List<INode> child) => this.child = child;

    public override INode.NodeState Evaluate()
    {
        if (child == null || child.Count <= 0) return INode.NodeState.Failure;

        foreach (var node in child)
        {
            var state = node?.Evaluate() ?? INode.NodeState.Failure;
            if (state != INode.NodeState.Success) return state;
        }
        
        return INode.NodeState.Success;
    }

    public override void Clear()
    {
        foreach (var node in child)
        {
            node?.Clear();
        }
        
        child?.Clear();
        child = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.BehaviourTree;
using UnityEngine;

/// <summary>
/// 성공한 자식노드가 있을 경우 곧바로 성공을 반환하는 노드
/// </summary>
public class SelectorNode : Node
{
    protected List<INode> child;
    
    public SelectorNode(List<INode> child) => this.child = child;

    public override INode.NodeState Evaluate()
    {
        if (child == null || child.Count <= 0) return INode.NodeState.Failure;

        foreach (var node in child)
        {
            var state = node?.Evaluate() ?? INode.NodeState.Failure;
            if (state == INode.NodeState.Success) return state;
        }
        
        return INode.NodeState.Failure;
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

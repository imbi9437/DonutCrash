using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.BehaviourTree;
using UnityEngine;

/// <summary>
/// 조건을 만족할 경우 자식 노드를 실행하는 노드
/// </summary>
public class DecoratorNode : Node
{
    protected INode child;
    protected Func<bool> condition;

    public DecoratorNode(INode child, Func<bool> condition)
    {
        this.child = child;
        this.condition = condition;
    }

    public override INode.NodeState Evaluate()
    {
        if (condition == null || condition?.Invoke() == false) return INode.NodeState.Failure;
        return child?.Evaluate() ?? INode.NodeState.Failure;
    }
    
    public override void Clear()
    {
        child?.Clear();
        child = null;
        condition = null;
    }
}

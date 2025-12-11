using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.BehaviourTree;
using UnityEngine;


/// <summary>
/// 행동 노드 : 단순히 함수를 실행하는 노드 EX) 특정 위치로 이동
/// </summary>
public class ActionNode : Node
{
    protected Func<INode.NodeState> update;
    
    public ActionNode(Func<INode.NodeState> update) => this.update = update;
    protected ActionNode() { }
    
    public override INode.NodeState Evaluate() => update?.Invoke() ?? INode.NodeState.Failure;
    public override void Clear() => update = null;
}

using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.BehaviourTree;
using UnityEngine;

/// <summary>
/// Behaviour Tree 노드의 최상위 객체
/// </summary>
public abstract class Node : INode
{
    public abstract INode.NodeState Evaluate();
    public abstract void Clear();
}

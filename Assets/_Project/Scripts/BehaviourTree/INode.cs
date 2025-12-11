using UnityEngine;

namespace _Project.Scripts.BehaviourTree
{
    /// <summary>
    /// Behaviour Tree의 노드를 구현하기 위한 Interface
    /// </summary>
    public interface INode
    {
        public enum NodeState { Success, Failure, Running }
        
        public NodeState Evaluate();
        public void Clear();
    }
}

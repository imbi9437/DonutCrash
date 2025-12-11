using _Project.Scripts.BehaviourTree;
using _Project.Scripts.UI;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.DialogSystem
{
    public class TutorialDialogTree : Node
    {
        public Transform canvas;
        public DialogPanel dialogPrefab;
    
        public INode.NodeState isMenuTutorial;
        public INode.NodeState isMergeTutorial;
        public DialogSequence dialogSequence;
    
        private List<INode> _children;

        public void Initialize(List<INode> children, Transform parent, DialogPanel dialogPrefab)
        {
            _children = children;
            canvas = parent;
            this.dialogPrefab = dialogPrefab;
        
            isMenuTutorial = INode.NodeState.Failure;
            isMergeTutorial = INode.NodeState.Failure;
        }
    
        public override INode.NodeState Evaluate()
        {
            foreach (INode i in _children)
            {
                if (i.Evaluate() == INode.NodeState.Success)
                    return INode.NodeState.Success;
            }
            return INode.NodeState.Failure;
        }

        public override void Clear()
        {
            _children.Clear();
        }

        public void UpdateDialogTree()
        {
            dialogSequence?.CancelSequence();
            
            Evaluate();
        }
    }
}

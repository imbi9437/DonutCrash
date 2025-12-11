using _Project.Scripts.BehaviourTree;
using _Project.Scripts.DialogSystem;
using _Project.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeTutorialAction : ActionNode
{
    private TutorialDialogTree _tutorialDialogTree;
    
    public MergeTutorialAction(TutorialDialogTree tutorialDialogTree)
    {
        _tutorialDialogTree = tutorialDialogTree;
        update = StartMergeTutorial;
    }

    private INode.NodeState StartMergeTutorial()
    {
        Debug.Log("StartMergeTutorial");
        if (_tutorialDialogTree.isMergeTutorial == INode.NodeState.Success)
            return _tutorialDialogTree.isMergeTutorial;
        
        _tutorialDialogTree.dialogSequence?.CancelSequence();
        _tutorialDialogTree.dialogSequence = new DialogSequence(_tutorialDialogTree.canvas,
            new[]
            {
                new DialogFactor(_tutorialDialogTree.dialogPrefab, null, "Merge Tutorial", "It's <color=\"red\">Merge</color> tutorial", 50),
                new DialogFactor(_tutorialDialogTree.dialogPrefab, null, "Merge Tutorial", "Can be organize multiple dialog", 50, OnMergeTutorialComplete)
            });
        _tutorialDialogTree.dialogSequence.StartSequence();
        return INode.NodeState.Success;
    }

    private void OnMergeTutorialComplete()
    {
        _tutorialDialogTree.isMergeTutorial = INode.NodeState.Success;
        _tutorialDialogTree.UpdateDialogTree();
    }
}

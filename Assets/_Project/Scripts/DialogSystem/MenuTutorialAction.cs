using _Project.Scripts.BehaviourTree;
using _Project.Scripts.DialogSystem;
using _Project.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTutorialAction : ActionNode
{
    private TutorialDialogTree _tutorialDialogTree;
    
    public MenuTutorialAction(TutorialDialogTree tutorialDialogTree)
    {
        _tutorialDialogTree = tutorialDialogTree;
        update = StartMenuTutorial;
    }

    private INode.NodeState StartMenuTutorial()
    {
        Debug.Log("StartMenuTutorial");
        if (_tutorialDialogTree.isMenuTutorial == INode.NodeState.Success)
            return _tutorialDialogTree.isMenuTutorial;
            
        _tutorialDialogTree.dialogSequence?.CancelSequence();
        _tutorialDialogTree.dialogSequence = new DialogSequence(_tutorialDialogTree.canvas,
            new[]
            {
                new DialogFactor(_tutorialDialogTree.dialogPrefab, null, "Menu Tutorial", "It's <color=\"red\">MENU</color> tutorial", 50, OnMenuTutoComplete)
            });
        _tutorialDialogTree.dialogSequence.StartSequence();
        return INode.NodeState.Running;
    }
    
    private void OnMenuTutoComplete()
    {
        _tutorialDialogTree.isMenuTutorial = INode.NodeState.Success;
        _tutorialDialogTree.UpdateDialogTree();
    }
}

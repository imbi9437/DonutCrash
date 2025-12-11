using _Project.Scripts.InGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.InGame.StatePattern;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TurnGameSample : MonoBehaviour
{
    // public TurnController turnController;
    //
    // public Text text;
    // public Button myShoot;
    // public Button enemyShoot;
    //
    // public Button stopButton;
    // public Button endGame;
    //
    // public Toggle myReady;
    // public Toggle enemyReady;
    //
    //
    // public List<MonoState> states;
    //
    // private void Start()
    // {
    //     myReady.onValueChanged.AddListener(CheckReady);
    //     enemyReady.onValueChanged.AddListener(CheckReady);
    //     
    //     myShoot.onClick.AddListener(() => Shoot(true));
    //     enemyShoot.onClick.AddListener(() => Shoot(false));
    //     
    //     stopButton.onClick.AddListener(() =>
    //     {
    //         turnController.ChangeState(TurnState.End);
    //         StartCoroutine(Wait(0.3f));
    //     });
    //     
    //     endGame.onClick.AddListener(() =>turnController.ChangeState(TurnState.None));
    // }
    //
    //
    // private void Update()
    // {
    //     if (states.Count <= 0) return;
    //     var state = states.FirstOrDefault(s => s.gameObject.activeSelf);
    //     if (state == null) return;
    //     string s = $"{(BattleState)state.index}";
    //
    //     if (state.index is 1 or 2 or 3)
    //     {
    //         string player = turnController.isPlayer1Turn ? "My " : "Enemy ";
    //         s = $"{player}{s}";
    //         text.text = s;
    //     }
    //     else text.text = s;
    //     
    //     
    // }
    //
    // private void OnDestroy()
    // {
    //     myReady.onValueChanged.RemoveListener(CheckReady);
    //     enemyReady.onValueChanged.RemoveListener(CheckReady);
    //     
    //     myShoot.onClick.RemoveAllListeners();
    //     enemyShoot.onClick.RemoveAllListeners();
    //     
    //     stopButton.onClick.RemoveAllListeners();
    //     endGame.onClick.RemoveAllListeners();
    // }
    //
    // private void CheckReady(bool isOn)
    // {
    //     if (myReady.isOn && enemyReady.isOn) turnController.ChangeState(TurnState.Start);
    // }
    //
    // private void Shoot(bool isMyTurn)
    // {
    //     if (turnController.isPlayer1Turn != isMyTurn) return;
    //     
    //     Debug.Log("발사!");
    //     turnController.ChangeState(TurnState.Running);
    // }
    //
    // private IEnumerator Wait(float time)
    // {
    //     yield return new WaitForSeconds(time);
    //     turnController.ChangeState(TurnState.Start);
    // }
}

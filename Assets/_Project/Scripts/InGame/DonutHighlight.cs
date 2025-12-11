using _Project.Scripts.InGame;
using System;
using UnityEngine;

public class DonutHighlight : MonoBehaviour
{
    // 아래의 렌더러를 가져오는 구문은 변경 가능성 존재. 비주얼 적인게 지금 도넛 오브젝트에 귀속되어 있지 않기 때문
    // private Renderer _renderer;

    //하이라이트
    public GameObject selectHighlight;
    //public GameObject selectedHighlight;

    // [Header("색깔 변경")]
    // public Material player1UnusedColor;
    // public Material player1UsedColor;
    // public Material player2UnusedColor;
    // public Material player2UsedColor;
    
    // void Awake()
    // {
    //     _renderer = GetComponent<Renderer>();
    // }

    public void SetSelectHighlights(InGameOwner owner, InGameOwner turnOwner, bool interactable, InGameOwner client, BattleState turn)
    {
        if (client != turnOwner || turn != BattleState.TurnStart)
        {
            selectHighlight.SetActive(false);
            return;
        }

        selectHighlight.SetActive(owner == turnOwner && interactable);
    }
    
    public void SetSelectedHighlights(Vector2 force)
    {
        bool isDragging = force.magnitude > .1f;

        selectHighlight.SetActive(false);
    }

    // public void SetColor(InGameOwner owner, bool interactable)
    // {
    //     _renderer.sharedMaterial = owner switch
    //     {
    //         InGameOwner.Left => interactable ? player1UnusedColor : player1UsedColor,
    //         InGameOwner.Right => interactable ? player2UnusedColor : player2UsedColor,
    //         _ => _renderer.sharedMaterial
    //     };
    // }
}

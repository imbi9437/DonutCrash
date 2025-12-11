using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButtons : MonoBehaviour
{
    [SerializeField] private Button sortLevelButton;
    [SerializeField] private Button sortTireButton;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    private void OnEnable()
    {
        sortLevelButton.onClick.AddListener(()=>
        {
            sortLevelButton.image.sprite = selectedSprite;
            sortTireButton.image.sprite = unselectedSprite;
        });
        sortTireButton.onClick.AddListener(()=> 
        {
            sortTireButton.image.sprite = selectedSprite;
            sortLevelButton.image.sprite = unselectedSprite;
        });
    }

    private void OnDisable()
    {
        sortLevelButton.onClick.RemoveAllListeners();
        sortTireButton.onClick.RemoveAllListeners();
        
        sortLevelButton.image.sprite = unselectedSprite;
        sortTireButton.image.sprite = unselectedSprite;
    }

}

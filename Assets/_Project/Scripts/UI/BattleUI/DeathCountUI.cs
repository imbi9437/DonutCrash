using _Project.Scripts.InGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using IGS = _Project.Scripts.EventStructs.InGameStructs;

public class DeathCountUI : MonoBehaviour
{
    [SerializeField] private InGameOwner owner;
    [SerializeField] private Sprite countUpIcon;
    [SerializeField] private Sprite countDownIcon;
    [SerializeField] private GameObject counts;

    [SerializeField] private Color countUpColor;
    [SerializeField] private Color countDownColor;
    
    private List<Image> _countSprites = new ();

    private void Start()
    {
        _countSprites = counts.GetComponentsInChildren<Image>().ToList();
        
        EventHub.Instance?.RegisterEvent<IGS.ChangeDeathCount>(OnSetDeathCountUI);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.ChangeDeathCount>(OnSetDeathCountUI);
    }

    private void OnSetDeathCountUI(IGS.ChangeDeathCount evt) => SetDeathCountUI(evt.deathOwner, evt.deathCount);

    private void SetDeathCountUI(InGameOwner changer, int deathCount)
    {
        if (owner != changer)
            return;

        for (int i = 0; i < _countSprites.Count; i++)
        {
            _countSprites[i].sprite = i - deathCount < 0 ? countUpIcon : countDownIcon;
            _countSprites[i].color = i - deathCount < 0 ? countUpColor : countDownColor;
        }
    }
}

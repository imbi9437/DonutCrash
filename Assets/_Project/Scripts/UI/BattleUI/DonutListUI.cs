using _Project.Scripts.InGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using IGS = _Project.Scripts.EventStructs.InGameStructs;

public class DonutListUI : MonoBehaviour
{
    [SerializeField] private InGameOwner owner;
    [SerializeField] private GameObject stagedDonuts;
    [SerializeField] private GameObject unstagedDonuts;
    
    private List<DonutIconUI> stagedDonutIcons = new();
    private List<DonutIconUI> unstagedDonutIcons = new();
    
    private void Start()
    {
        stagedDonutIcons = stagedDonuts.GetComponentsInChildren<DonutIconUI>().ToList();
        unstagedDonutIcons = unstagedDonuts.GetComponentsInChildren<DonutIconUI>().ToList();
        
        EventHub.Instance?.RegisterEvent<IGS.ChangeDonutList>(OnChangeDonutList);
    }

    private void OnDestroy()
    {
        EventHub.Instance?.UnRegisterEvent<IGS.ChangeDonutList>(OnChangeDonutList);
    }

    private void OnChangeDonutList(IGS.ChangeDonutList evt) => ChangeDonutList(evt.owner, evt.stagedDonuts, evt.unstagedDonuts);

    private void ChangeDonutList(InGameOwner changer, List<DonutInstanceData> stagedDonut, List<DonutInstanceData> unstagedDonut)
    {
        if (owner != changer)
            return;

        for (int i = 0; i < stagedDonutIcons.Count; i++)
            stagedDonutIcons[i].Initialize(i < stagedDonut.Count ? stagedDonut[i] : null);

        for (int i = 0; i < unstagedDonutIcons.Count; i++)
            unstagedDonutIcons[i].Initialize(i < unstagedDonut.Count ? unstagedDonut[i] : null);
    }
}

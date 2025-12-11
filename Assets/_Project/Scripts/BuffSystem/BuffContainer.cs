using _Project.Scripts.BuffSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffContainer : MonoBehaviour, IBuffContainer
{
    [SerializeField] private DonutInstanceData _donutInstanceData;
    [SerializeField] private DonutInGameData _donutInGameData;
    private List<BuffBase> _buffs = new List<BuffBase>();

    public void Initialize(DonutInstanceData instanceData)
    {
        _donutInstanceData = instanceData;
        _donutInGameData = new DonutInGameData(instanceData);
    }

    private void OnChangeBuff()
    {
        DonutInGameData result = new DonutInGameData(_donutInstanceData);
        _buffs.ForEach(x => x.ModifyAdd(result));
        _buffs.ForEach(x => x.ModifyMulti(result));
        _donutInGameData = result;
        Debug.Log($"현재 버프 갯수: {_buffs.Count}");
    }

    public void AddBuff(BuffBase buff)
    {
        BuffBase sameBuff = _buffs.Find(x => x.GetType() == buff.GetType());
        if (sameBuff == null)
            _buffs.Add(buff);
        else
            sameBuff.StackedUp(1);
        
        OnChangeBuff();
    }

    public void RemoveBuff(BuffBase buff)
    {
        if (_buffs.Contains(buff))
            _buffs.Remove(buff);
    }
}

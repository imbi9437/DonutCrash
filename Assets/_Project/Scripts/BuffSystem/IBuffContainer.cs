using _Project.Scripts.BuffSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffContainer
{
    public void AddBuff(BuffBase buff);
    public void RemoveBuff(BuffBase buff);
}

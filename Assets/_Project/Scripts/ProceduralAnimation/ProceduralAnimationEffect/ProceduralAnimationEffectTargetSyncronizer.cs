using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralAnimationEffectTargetSyncronizer : MonoBehaviour
{
    public string proceduralAnimationEffectName; 
    public Transform origin;
    public Transform target;

    private IProceduralAnimationEffect proceduralAnimationEffect;
    private Vector3 targetVector;

    private void Start()
    {
        proceduralAnimationEffect = GetComponents<IProceduralAnimationEffect>().ToList().Find(obj => obj.GetName() == proceduralAnimationEffectName);
    }

    private void Update()
    {
        targetVector = target.position - origin.position;
        proceduralAnimationEffect.SetTargetVector3(-targetVector);
    }
}

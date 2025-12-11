using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProceduralAnimation : MonoBehaviour
{
    public string animationName;
    private Dictionary<string, IProceduralAnimationEffect> effectDict;
    public float animationTime;
    public bool isLoop;
    public float loopLength;
    public float animationSpeed = 1f;
    [Range(0, 1)] public float weight;
    public ProceduralAnimationEvent[] proceduralAnimationEvent;

    [Serializable]
    public class ProceduralAnimationEvent
    {
        public float timing;
        public UnityEvent events;
    }
    
    private TransformContainer transformContainer;
    private TransformContainer result = new TransformContainer();
    private float prevAnimaTime;

    private void Start()
    {
        effectDict = new Dictionary<string, IProceduralAnimationEffect>();
        foreach (IProceduralAnimationEffect i in GetComponents<IProceduralAnimationEffect>())
        {
            effectDict.Add(i.GetName(), i);
        }
    }

    public TransformContainer GetTransform(float deltaTime)
    {
        result.Reset();
        if (weight == 0)
        {
            animationTime = 0;
            return result;
        }
        prevAnimaTime = animationTime;
        animationTime += deltaTime * animationSpeed;

        foreach (var i in proceduralAnimationEvent)
        {
            if (prevAnimaTime <= i.timing && i.timing < animationTime) i.events?.Invoke();
        }
        
        foreach (IProceduralAnimationEffect effect in effectDict.Values)
        {
            transformContainer = effect.GetTransform(animationTime);
            result += transformContainer;
        }
        
        result *= weight;

        if (isLoop) animationTime = animationTime % loopLength;

        return result;
    }
    
    public IProceduralAnimationEffect FindIEffect(string effectName) => effectDict[effectName];
    public ProceduralAnimationEffect FindEffect(string effectName) => effectDict[effectName] as ProceduralAnimationEffect;
}

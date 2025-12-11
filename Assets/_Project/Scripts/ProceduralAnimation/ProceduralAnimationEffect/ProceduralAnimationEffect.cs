using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProceduralAnimationEffect : MonoBehaviour, IProceduralAnimationEffect
{
    public string effectName;
    public Vector3 targetVector3;
    public float length;
    public float offset;
    public bool isLoop;
    public bool usePosition;
    public bool useRotation;
    public bool useScale;
    [Range(0, 1)]public float weight;

    protected float normalizedTime;

    protected static Vector3 defaultVector3 = Vector3.zero;

    public abstract TransformContainer GetTransform(float time);
    public void SetWeight(float value) => weight = value;
    public void SetTargetVector3(Vector3 target) => targetVector3 = target;
    public bool Equals(string value) => effectName == value;
    public string GetName() => effectName;

    protected void RawToResult(Vector3 resultRaw, TransformContainer result)
    {
        result.Reset();

        result.position = usePosition ? resultRaw : Vector3.zero;
        result.rotation = useRotation ? resultRaw : Vector3.zero;
        result.scale = useScale ? resultRaw + Vector3.one : Vector3.one;
    }
}

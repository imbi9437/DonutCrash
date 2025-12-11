using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveProceduralAnimationEffect : ProceduralAnimationEffect
{
    public bool cubicBezier;
    [HideInInspector] public Vector3 point0;
    [HideInInspector] public Vector3 point1;
    [Range(1,1000)] public int repeat = 1;
    public bool decrease;
    [HideInInspector] public CurveType decreaseType = CurveType.Linear;
    [HideInInspector] public float decreasePower = 1;
    public bool increase;
    [HideInInspector] public CurveType increaseType = CurveType.Linear;
    [HideInInspector] public float increasePower = 1;

    private Vector3 resultRaw;
    private TransformContainer result = new TransformContainer();
    private float modTime;

    public override TransformContainer GetTransform(float time)
    {
        if (weight == 0)
        {
            result.Reset();
            return result;
        }
        
        normalizedTime = (time + offset) / length;

        if (normalizedTime > 1f)
            normalizedTime = isLoop ? normalizedTime % 1 : 1f;

        modTime = (normalizedTime * repeat) % 1;

        if (cubicBezier)
        {
            resultRaw = defaultVector3 + 3 * (1 - modTime) * (1 - modTime) * modTime * point0 + 3 * (1 - modTime) * modTime * modTime * point1 + modTime * modTime * modTime * targetVector3;
        }
        else
        {
            resultRaw = defaultVector3 + (targetVector3 - defaultVector3) * modTime;
        }

        if (decrease)
        {
            switch (decreaseType)
            {
                case CurveType.Linear:
                    resultRaw *= (1 - normalizedTime) * decreasePower;
                    break;
                case CurveType.Square:
                    resultRaw *= (1 - normalizedTime * normalizedTime) * decreasePower;
                    break;
            }
        }

        if (increase)
        {
            switch (increaseType)
            {
                case CurveType.Linear:
                    resultRaw *= (normalizedTime) * increasePower;
                    break;
                case CurveType.Square:
                    resultRaw *= (normalizedTime * normalizedTime) * increasePower;
                    break;
            }
        }

        resultRaw *= weight;

        RawToResult(resultRaw, result);

        return result;
    }
    
    public enum CurveType
    {
        Linear,
        Square,
    }
}

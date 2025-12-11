using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightProceduralAnimationEffect : ProceduralAnimationEffect
{
    private Vector3 resultRaw;
    private TransformContainer result = new TransformContainer();
    
    public override TransformContainer GetTransform(float time)
    {
        if (weight == 0)
        {
            result.Reset();
            return result;
        }
        
        resultRaw = targetVector3 * weight;
        RawToResult(resultRaw, result);
        return result;
    }
}

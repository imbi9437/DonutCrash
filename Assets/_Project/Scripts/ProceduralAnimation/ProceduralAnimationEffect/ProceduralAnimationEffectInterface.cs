using UnityEngine;

public interface IProceduralAnimationEffect
{
    public TransformContainer GetTransform(float time);
    public void SetWeight(float value);
    public void SetTargetVector3(Vector3 target);
    public bool Equals(string value);
    public string GetName();
}
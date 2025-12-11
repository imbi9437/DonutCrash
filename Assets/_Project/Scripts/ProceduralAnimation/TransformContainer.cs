using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TransformContainer
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public TransformContainer()
    {
        position = Vector3.zero;
        rotation = Vector3.zero;
        scale = Vector3.one;
    }

    public TransformContainer(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }

    public void Reset()
    {
        position = Vector3.zero;
        rotation = Vector3.zero;
        scale = Vector3.one;
    }

    public static TransformContainer operator + (TransformContainer left, TransformContainer right)
    {
        TransformContainer result = new TransformContainer();
        result.position = left.position + right.position;
        result.rotation = left.rotation + right.rotation;
        result.scale = left.scale + right.scale - Vector3.one;
        return result;
    }
    public static TransformContainer operator - (TransformContainer left, TransformContainer right)
    {
        TransformContainer result = new TransformContainer();
        result.position = left.position - right.position;
        result.rotation = left.rotation - right.rotation;
        result.scale = left.scale - right.scale;
        return result;
    }
    public static TransformContainer operator * (TransformContainer left, float right)
    {
        TransformContainer result = new TransformContainer();
        result.position = left.position * right;
        result.rotation = left.rotation * right;
        result.scale = left.scale * right;
        return result;
    }

    public static bool operator == (TransformContainer left, TransformContainer right) => left == right;
    public static bool operator != (TransformContainer left, TransformContainer right) => left != right;
    public override bool Equals(object obj) => this == (obj as TransformContainer);
    public override int GetHashCode() => base.GetHashCode();

    public static implicit operator bool(TransformContainer left) => left != null;
}

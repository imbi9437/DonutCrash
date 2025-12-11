using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeightProceduralAnimationEffect))]
public class WeightProceduralAnimationEffectEditor : Editor
{
    private SerializedProperty effectNameProp;
    private SerializedProperty targetVector3Prop;
    private SerializedProperty usePositionProp;
    private SerializedProperty useRotationProp;
    private SerializedProperty useScaleProp;
    private SerializedProperty weightProp;

    private void OnEnable()
    {
        effectNameProp = serializedObject.FindProperty("effectName");
        targetVector3Prop = serializedObject.FindProperty("targetVector3");
        usePositionProp = serializedObject.FindProperty("usePosition");
        useRotationProp = serializedObject.FindProperty("useRotation");
        useScaleProp = serializedObject.FindProperty("useScale");
        weightProp = serializedObject.FindProperty("weight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(effectNameProp);
        EditorGUILayout.PropertyField(targetVector3Prop);
        EditorGUILayout.PropertyField(usePositionProp);
        EditorGUILayout.PropertyField(useRotationProp);
        EditorGUILayout.PropertyField(useScaleProp);
        EditorGUILayout.PropertyField(weightProp);
        
        serializedObject.ApplyModifiedProperties();
    }
}

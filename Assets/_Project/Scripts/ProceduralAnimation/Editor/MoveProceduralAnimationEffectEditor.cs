using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveProceduralAnimationEffect))]
public class MoveProceduralAnimationEffectEditor : Editor
{
    private SerializedProperty effectNameProp;
    private SerializedProperty targetVector3Prop;
    private SerializedProperty lengthProp;
    private SerializedProperty offsetProp;
    private SerializedProperty isLoopProp;
    private SerializedProperty usePositionProp;
    private SerializedProperty useRotationProp;
    private SerializedProperty useScaleProp;
    private SerializedProperty weightProp;
    private SerializedProperty repeatProp;
    private SerializedProperty cubicBezierProp;
    private SerializedProperty point0Prop;
    private SerializedProperty point1Prop;
    private SerializedProperty decreaseProp;
    private SerializedProperty decreaseTypeProp;
    private SerializedProperty decreasePowerProp;
    private SerializedProperty increaseProp;
    private SerializedProperty increaseTypeProp;
    private SerializedProperty increasePowerProp;

    private bool cubicBezierFoldout = true;
    private bool decreaseFoldout = true;
    private bool increaseFoldout = true;

    private void OnEnable()
    {
        effectNameProp = serializedObject.FindProperty("effectName");
        targetVector3Prop = serializedObject.FindProperty("targetVector3");
        lengthProp = serializedObject.FindProperty("length");
        offsetProp = serializedObject.FindProperty("offset");
        isLoopProp = serializedObject.FindProperty("isLoop");
        usePositionProp = serializedObject.FindProperty("usePosition");
        useRotationProp = serializedObject.FindProperty("useRotation");
        useScaleProp = serializedObject.FindProperty("useScale");
        weightProp = serializedObject.FindProperty("weight");
        repeatProp = serializedObject.FindProperty("repeat");
        cubicBezierProp = serializedObject.FindProperty("cubicBezier");
        point0Prop = serializedObject.FindProperty("point0");
        point1Prop = serializedObject.FindProperty("point1");
        decreaseProp = serializedObject.FindProperty("decrease");
        decreaseTypeProp = serializedObject.FindProperty("decreaseType");
        decreasePowerProp = serializedObject.FindProperty("decreasePower");
        increaseProp = serializedObject.FindProperty("increase");
        increaseTypeProp = serializedObject.FindProperty("increaseType");
        increasePowerProp = serializedObject.FindProperty("increasePower");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(effectNameProp);
        EditorGUILayout.PropertyField(targetVector3Prop);
        EditorGUILayout.PropertyField(lengthProp);
        EditorGUILayout.PropertyField(offsetProp);
        EditorGUILayout.PropertyField(isLoopProp);
        EditorGUILayout.PropertyField(usePositionProp);
        EditorGUILayout.PropertyField(useRotationProp);
        EditorGUILayout.PropertyField(useScaleProp);
        EditorGUILayout.PropertyField(weightProp);
        EditorGUILayout.PropertyField(repeatProp);

        EditorGUILayout.PropertyField(cubicBezierProp);
        if (cubicBezierProp.boolValue)
        {
            cubicBezierFoldout = EditorGUILayout.Foldout(cubicBezierFoldout, "CubicBezierPoints", true);
            if (cubicBezierFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(point0Prop);
                EditorGUILayout.PropertyField(point1Prop);
                EditorGUI.indentLevel--;
            }
        }
        

        EditorGUILayout.PropertyField(decreaseProp);
        if (decreaseProp.boolValue)
        {
            decreaseFoldout = EditorGUILayout.Foldout(decreaseFoldout, "DecreaseOption", true);
            if (decreaseFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(decreaseTypeProp);
                EditorGUILayout.PropertyField(decreasePowerProp);
                EditorGUI.indentLevel--;
            }
        }
        
        EditorGUILayout.PropertyField(increaseProp);
        if (increaseProp.boolValue)
        {
            increaseFoldout = EditorGUILayout.Foldout(increaseFoldout, "IncreaseOption", true);
            if (increaseFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(increaseTypeProp);
                EditorGUILayout.PropertyField(increasePowerProp);
                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

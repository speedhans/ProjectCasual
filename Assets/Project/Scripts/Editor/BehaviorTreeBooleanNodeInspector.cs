using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CanEditMultipleObjects]
//[CustomEditor(typeof(BooleanNode), editorForChildClasses: true)]
public class BehaviorTreeBooleanNodeInspector :Editor
{
    //SerializedProperty m_RootNode;

    private void OnEnable()
    {
        //m_RootNode = serializedObject.FindProperty("m_RootNode");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorUtility.SetDirty(serializedObject.targetObject);

        //EditorGUILayout.PropertyField(m_RootNode);

        serializedObject.ApplyModifiedProperties();
    }
}

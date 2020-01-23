using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(LocationComparisonNode), editorForChildClasses: true)]
public class LocationComparisonNodeInspector : Editor
{
    SerializedProperty m_Location;
    private void OnEnable()
    {
        m_Location = serializedObject.FindProperty("m_Location");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();
        //m_Location.vector3Value = v3;

        //m_TEST.objectReferenceValue = m_T;

        serializedObject.ApplyModifiedProperties();
    }
}

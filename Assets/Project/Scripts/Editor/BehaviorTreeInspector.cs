using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(BehaviorTree))]
public class BehaviorTreeInspector : Editor
{
    SerializedProperty rootnode;
    private void OnEnable()
    {
        rootnode = serializedObject.FindProperty("m_RootNode");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        //EditorGUILayout.PropertyField(rootnode);

        if (rootnode.objectReferenceValue == null && GUILayout.Button("Create RootNode"))
        {
            string path = "";
            var scriptable = ScriptableObject.CreateInstance<RootNode>();

            string name = target.name;
            bool find = false;
            foreach(string s in AssetDatabase.GetSubFolders("Assets/Project/BehaviorTree"))
            {
                if (s.Contains(name))
                {
                    find = true;
                    break;
                }
            }

            if (!find)
            {
                Debug.Log("Create");
                path = AssetDatabase.CreateFolder("Assets/Project/BehaviorTree", name);
                path = AssetDatabase.GUIDToAssetPath(path);
            }
            else
            {   
                Debug.Log("default");
                path = "Assets/Project/BehaviorTree/" + name;
            }

            rootnode.objectReferenceValue = scriptable;
            scriptable.m_RootNode = scriptable;
            AssetDatabase.CreateAsset(scriptable, path + "/BehaviorTreeNode.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            serializedObject.ApplyModifiedProperties();
        }
        else if (rootnode.objectReferenceValue != null && GUILayout.Button("BehaviorTree Open"))
        {
            BehaviorTreeEditor window = (BehaviorTreeEditor)EditorWindow.GetWindow(typeof(BehaviorTreeEditor));
            window.Show();
            window.m_RootProperty = (ScriptableObject)rootnode.objectReferenceValue;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

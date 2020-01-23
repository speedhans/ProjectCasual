using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(RootNode), editorForChildClasses: true)]
public class BehaviorTreeRootNodeInspector : BehaviorTreeNodeInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUILayout.Button("BehaviorTree Open"))
        {
            BehaviorTreeEditor window = (BehaviorTreeEditor)EditorWindow.GetWindow(typeof(BehaviorTreeEditor));
            window.Show();
            window.m_RootProperty = (ScriptableObject)target;
        }

        if (GUILayout.Button("Refresh"))
        {
            RootNode root = serializedObject.targetObject as RootNode;
            EditorUtility.SetDirty(root);
            root.m_ListActionNode.Clear();
            RefreshChild(root, root);
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void RefreshChild(RootNode _Root, BehaviorTreeNode _Node)
    {
        if (_Node.m_ChildNode == null || _Node.m_ChildNode.Count < 1)
        {
            ActionNode a = _Node as ActionNode;
            if (a != null)
            {
                _Root.m_ListActionNode.Add(a);
            }
            return;
        }

        for (int i = 0; i < _Node.m_ChildNode.Count; ++i)
        {
            _Node.m_RootNode = _Root;
            if (_Node.m_ChildNode[i] == null)
                _Node.m_ChildNode.RemoveAt(i);
            else
            {
                RefreshChild(_Root, _Node.m_ChildNode[i]);
            }
        }
    }
}


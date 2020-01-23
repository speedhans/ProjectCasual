using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BehaviorTreeEditor : EditorWindow
{
    public ScriptableObject m_RootProperty;

    int m_FoldIndex = 0;
    List<bool> m_ListIsFoldout = new List<bool>();

    //GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(1000), GUILayout.Height(10) };

    [MenuItem("BehaviorTree/Editor")]
    static public void ShowWindow()
    {
        BehaviorTreeEditor window = (BehaviorTreeEditor)EditorWindow.GetWindow(typeof(BehaviorTreeEditor));
        window.minSize = new Vector2(600,600);
    }

    private void OnGUI()
    {
        m_RootProperty = EditorGUILayout.ObjectField("RootNode", m_RootProperty, typeof(ScriptableObject), false) as ScriptableObject;
        GUILayout.Space(5.0f);

        RootNode root = (RootNode)m_RootProperty;
        if (root == null) return;
        //for (int i = 0; i < root.m_ListActionNode.Count; ++i)
        //{
        //    if (root.m_ListActionNode[i] == null) root.m_ListActionNode.RemoveAt(i);
        //}

        m_FoldIndex = 0;
        DrawChildNode(root);

        //GUILayout.Label("Create Node");
        //if (GUILayout.Button("Selector_Defaule"))
        //{
        //    SelectorNode b = CreateNewNode<SelectorNode>(m_RootPath, E_TYPE.SELECTOR);
        //}
        //if (GUILayout.Button("Sequence_Defaule"))
        //{
        //    SequenceNode b = CreateNewNode<SequenceNode>(m_RootPath, E_TYPE.SEQUENCE);
        //}
    }

    bool IsFoldout(int _Number)
    {
        if (m_ListIsFoldout.Count <= _Number)
        {
            m_ListIsFoldout.Insert(_Number, false);
        }

        return m_ListIsFoldout[_Number];
    }

    void DrawChildNode(BehaviorTreeNode _Node)
    {
        if (_Node.m_ChildNode == null) return;

        //string[] split = _Node.ToString().Split('(');
        //string[] split2 = split[1].Split(')');
        if (_Node.m_ChildNode.Count > 0 && (m_ListIsFoldout[m_FoldIndex] = EditorGUILayout.Foldout(IsFoldout(m_FoldIndex++), "branch")))
        {
            GUI.backgroundColor = Color.cyan;
            EditorGUI.indentLevel += 2;
            for (int i = 0; i < _Node.m_ChildNode.Count; ++i)
            {
                //GUI.color = Color.cyan;
                _Node.m_ChildNode[i] = EditorGUILayout.ObjectField(_Node.m_ChildNode[i], typeof(BehaviorTreeNode), false) as BehaviorTreeNode;
                //GUI.color = Color.white;
                if (_Node.m_ChildNode[i] != null)
                {
                    if (_Node.m_ChildNode[i].m_ChildNode != null)
                        DrawChildNode(_Node.m_ChildNode[i]);
                }
                //else
                //{
                //    _Node.m_ChildNode.RemoveAt(i);
                //}
            }
            EditorGUI.indentLevel -= 2;
        }
        //GUILayout.Space(5.0f);
    }

    //void DoWindow(int _windowID)
    //    {
    //        SerializedProperty list = m_CurrentDrawObject.FindProperty("m_ChildNode");
    //        Debug.Log(m_CurrentDrawObject.targetObject.name);
    //        if (list == null) return;
    //        for (int i = 0; i < list.arraySize; ++i)
    //        {
    //            EditorGUILayout.LabelField(list.GetArrayElementAtIndex(0).name);
    //            EditorGUILayout.BeginVertical();
    //            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent("ChildNode"));
    //            EditorGUILayout.EndVertical();
    //            EditorGUILayout.Space();
    //        }
    //    }

    //    void DrawNode(ref int _ID, SerializedObject _Target, int _PositionX, int _PositionY, int _Witdh, int _Height)
    //    {
    //        if (_Target == null) return;

    //        //GUILayout.Window(_ID, new Rect(_PositionX, _PositionY, _Witdh, _Height), DoWindow, "Node");

    //        SerializedProperty list = _Target.FindProperty("m_ChildNode");
    //        if (list == null) return;

    //        for (int i = 0; i < list.arraySize; ++i)
    //        {
    //            BehaviorTreeNode node = list.GetArrayElementAtIndex(i).objectReferenceValue as BehaviorTreeNode;
    //            if (node)
    //            {
    //                _ID++;
    //                m_CurrentDrawObject = list.GetArrayElementAtIndex(i).serializedObject;
    //                Debug.Log(m_CurrentDrawObject.targetObject.name + " : " + node.name);

    //                SerializedProperty list2 = m_CurrentDrawObject.FindProperty("m_ChildNode");
    //                if (list2 != null)
    //                {
    //                    Debug.Log(list2.arraySize.ToString() + " : " + node.name);
    //                }

    //                //DrawNode(ref _ID, m_CurrentDrawObject, _PositionX, _PositionY + 100, _Witdh, _Height);
    //            }
    //        }
    //    }


    private void OnInspectorUpdate()
    {
        this.Repaint();
    }
}

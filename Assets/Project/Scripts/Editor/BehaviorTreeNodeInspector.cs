using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(BranchNode), editorForChildClasses:true)]
public class BehaviorTreeNodeInspector : Editor
{
    BT.E_NODE m_Type;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        BranchNode branch = (BranchNode)serializedObject.targetObject;
        
        SerializedProperty childnode = serializedObject.FindProperty("m_ChildNode");
        
        ShowChildNode(childnode);

        if (branch.m_RootNode == null) return;

        EditorUtility.SetDirty(branch);
        for (int i = 0; i < branch.m_RootNode.m_ListActionNode.Count; ++i)
        {
            if (branch.m_RootNode.m_ListActionNode[i] == null) branch.m_RootNode.m_ListActionNode.RemoveAt(i);
        }
        GUILayout.Space(7);
        GUILayout.Label("시퀸스 노드는 모든 노드가 True 일때만 True 를 리턴합니다");
        GUILayout.Label("셀렉트 노드는 노드중 하나라도 True 이면 True 를 리턴합니다");
        GUILayout.Label("불리언 노드는 1번 노드의 결과가 True 일때");
        GUILayout.Label("2번 노드를 실행, False 일떄 3번 노드를 실행합니다");

        m_Type = (BT.E_NODE)EditorGUILayout.EnumPopup(m_Type);

        if (GUILayout.Button("Add Node"))
        {
            if (m_Type == BT.E_NODE.NONE) return;

            var scriptable = CreateNode();
            string path = AssetDatabase.GetAssetPath(target);
            string[] split1 = path.Split('.');
            string[] split2 = split1[0].Split('/');
            string rootpath = "";

            for (int i = 0; i < split2.Length - 1; ++i)
            {
                rootpath += split2[i];
                if (i < split2.Length - 2)
                    rootpath += "/";
            }

            string strrange = "abscefghijklnmopqrstuvwxyz123456789!@#$%&ABCDEFGHIJKLNMOPQRSTUVWXYZ";
            string name = "";
            for (int i = 0; i < 8; ++i)
            {
                name += strrange[UnityEngine.Random.Range(0, strrange.Length - 1)];
            }

            string[] paths = AssetDatabase.FindAssets("", new string[] { rootpath });
            
            for (int i = 0; i < paths.Length; ++i)
            {
                if (paths[i].Contains(name))
                {
                    i = 0;
                    name = "";
                    for (int j = 0; j < 8; ++j)
                    {
                        name += strrange[UnityEngine.Random.Range(0, strrange.Length - 1)];
                    }
                }
            }


            ((BehaviorTreeNode)scriptable).m_RootNode = branch.m_RootNode;
            Debug.Log(path);
            //AssetDatabase.AddObjectToAsset();
            string savepath = rootpath +"/" + name + ".asset";
            AssetDatabase.CreateAsset(scriptable, savepath);
            AssetDatabase.Refresh();

            if (m_Type == BT.E_NODE.Selector || m_Type == BT.E_NODE.Sequence || m_Type == BT.E_NODE.BooleanNode)
            {
                branch.AddChildNode((BranchNode)scriptable);
            }
            else
            {
                branch.m_RootNode.m_ListActionNode.Add((ActionNode)scriptable);
                branch.AddActionNode((ActionNode)scriptable);
            }

            AssetDatabase.SaveAssets();
            serializedObject.ApplyModifiedProperties();
        }
        else
            serializedObject.ApplyModifiedProperties();
    }

    ScriptableObject CreateNode()
    {
        switch(m_Type)
        {
            case BT.E_NODE.Selector:
                return CreateInstance<SelectorNode>();
            case BT.E_NODE.Sequence:
                return CreateInstance<SequenceNode>();
            case BT.E_NODE.BooleanNode:
                return CreateInstance<BooleanNode>();
            case BT.E_NODE.WaitForTime:
                return CreateInstance<WaitForTimeNode>();
            case BT.E_NODE.MoveToLocation:
                return CreateInstance<MoveToLocationNode>();
            case BT.E_NODE.LocationComparisonNode:
                return CreateInstance<LocationComparisonNode>();
            case BT.E_NODE.SetRandomMoveLocation:
                return CreateInstance<SetRandomMoveLocation>();
            case BT.E_NODE.StateCheckNode:
                return CreateInstance<StateCheckNode>();
            case BT.E_NODE.AnimationPlayNode:
                return CreateInstance<AnimationPlayNode>();
            case BT.E_NODE.GlobalTimeCheckNode:
                return CreateInstance<GlobalTimeCheckNode>();
            case BT.E_NODE.CompareNode:
                return CreateInstance<CompareNode>();
            case BT.E_NODE.PatrolMoveNode:
                return CreateInstance<PatrolMoveNode>();
            case BT.E_NODE.SearchObject:
                return CreateInstance<SearchObject>();
            case BT.E_NODE.StopMoving:
                return CreateInstance<StopMoving>();
            case BT.E_NODE.HeadFocusNode:
                return CreateInstance<HeadFocusNode>();
        }

        return null;
    }

    public void ShowChildNode(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list);
        EditorGUI.indentLevel++;
        //if (list.isExpanded)
        //{
        EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
        for (int i = 0; i < list.arraySize; i++)
        {
            SerializedProperty property = list.GetArrayElementAtIndex(i);

            EditorGUILayout.PropertyField(property);

            //EditorGUILayout.ObjectField(list.GetArrayElementAtIndex(i));
        }
        //}
        EditorGUI.indentLevel--;
        //serializedObject.ApplyModifiedProperties();
    }
}

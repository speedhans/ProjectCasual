using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "GameSceneData", menuName = "CreateScriptableObject/GameSceneData")]
public class GameSceneData : ScriptableObject
{
    public E_QUESTLEVEL m_Level;
    public string m_SceneName;
    public Sprite m_SceneImage;
    public string m_StageName;
    public int m_StaminaValue;
    public List<QuestData.S_DropItemData> m_DropItemList;
    public bool m_Lock;
}

//[CustomEditor(typeof(GameSceneData))]
//public class GameSceneDataEditor : Editor
//{
//    GameSceneData m_Target;
//    SerializedProperty m_SceneNameProperty;
//    [SerializeField]
//    SceneAsset m_Scene;
//    private void OnEnable()
//    {
//        m_Target = target as GameSceneData;
//        m_SceneNameProperty = serializedObject.FindProperty("m_SceneName");
//    }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        serializedObject.Update();
//        if (m_Scene != null)
//        {
//            m_Target.m_SceneName = m_Scene.name;
//            EditorUtility.SetDirty(m_Target);
//        }
//        serializedObject.ApplyModifiedProperties();
//    }
//}

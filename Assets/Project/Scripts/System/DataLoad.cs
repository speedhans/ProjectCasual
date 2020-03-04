using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoad
{
    public enum E_SCENETYPE
    {
        SINGLE,
        MULTIPLAY,
    }

    static public List<QuestData> QuestSceneDataAllLoad(E_SCENETYPE _Type)
    {
        List<QuestData> list = new List<QuestData>();

        // 임시 리스트
        string[] arryStr = new string[] { "Stage_01", "Stage_02", "Stage_03" };
        for (int i = 0; i < arryStr.Length; ++i)
        {
            QuestData data = QuestSceneDataLoad(arryStr[i], _Type);
            list.Add(data);
        }

        return list;
    }

    static public QuestData QuestSceneDataLoad(string _SceneName)
    {
        GameSceneData scene = Resources.Load<GameSceneData>(_SceneName + "_data");
        QuestData data = new QuestData();
        data.m_Icon = scene.m_SceneImage;
        data.m_Level = scene.m_Level;
        data.m_ListDropItem = scene.m_DropItemList;
        data.m_Name = scene.m_StageName;
        data.m_SceneName = scene.m_SceneName;
        data.m_StaminaValue = scene.m_StaminaValue;
        data.m_Lock = scene.m_Lock;
        return data;
    }

    static public QuestData QuestSceneDataLoad(string _SceneName, E_SCENETYPE _Type)
    {
        GameSceneData scene = Resources.Load<GameSceneData>(_SceneName + "_data");
        QuestData data = new QuestData();
        data.m_Icon = scene.m_SceneImage;
        data.m_Level = scene.m_Level;
        data.m_Multiplay = _Type == E_SCENETYPE.SINGLE ? false : true;
        data.m_ListDropItem = scene.m_DropItemList;
        data.m_Name = scene.m_StageName;
        data.m_SceneName = scene.m_SceneName;
        data.m_StaminaValue = scene.m_StaminaValue;
        data.m_Lock = scene.m_Lock;
        return data;
    }
}

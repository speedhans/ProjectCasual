using System;
using System.Collections.Generic;
using UnityEngine;

public enum E_QUESTLEVEL
{
    NORMAL,
    HARD,
    VERYHARD,
}

public class QuestData
{
    [System.Serializable]
    public struct S_DropItemData
    {
        public Item m_Item;
        public int m_DropChance;
    }

    public Sprite m_Icon;
    public string m_Name;
    public E_QUESTLEVEL m_Level;
    public int m_StaminaValue;
    public string m_SceneName;
    public bool m_Multiplay;
    public List<S_DropItemData> m_ListDropItem;
    public bool m_Lock = false;
}

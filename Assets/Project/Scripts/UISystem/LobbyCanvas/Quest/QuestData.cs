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
    public Sprite m_Icon;
    public string m_Name;
    public E_QUESTLEVEL m_Level;
    public string m_SceneName;
    public bool m_Multiplay;
}

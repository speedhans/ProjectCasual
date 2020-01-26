using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : LobbyUI
{
    QuestListUI m_QuestListUI;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_QuestListUI = transform.Find("QuestListUI").GetComponent<QuestListUI>();
        m_QuestListUI.Initialize(_LobbyCanvasUI);
    }

    public void SingleBattleButton()
    {
        List<QuestData> tmpList = new List<QuestData>()
        {
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.NORMAL, m_Name = "Stage1", m_SceneName = "Stage_01", m_Multiplay = false },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.HARD, m_Name = "Stage2", m_SceneName = "Stage_02", m_Multiplay = false },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.VERYHARD, m_Name = "Stage3", m_SceneName = "Stage_03", m_Multiplay = false }
        };

        m_QuestListUI.QuestListOpen(tmpList); // 데이터 불러와서 넣어줘야함
    }

    public void MultiBattelButton()
    {
        List<QuestData> tmpList = new List<QuestData>()
        {
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.NORMAL, m_Name = "MultiStage1", m_SceneName = "MultiStage_01", m_Multiplay = true },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.HARD, m_Name = "MultiStage2", m_SceneName = "MultiStage_02", m_Multiplay = true },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.VERYHARD, m_Name = "MultiStage3", m_SceneName = "MultiStage_03", m_Multiplay = true }
        };

        m_QuestListUI.QuestListOpen(tmpList); // 데이터 불러와서 넣어줘야함
    }
}

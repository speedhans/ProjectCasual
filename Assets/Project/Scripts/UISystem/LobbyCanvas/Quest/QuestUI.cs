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
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.NORMAL, m_Name = "Stage1", m_SceneName = "Stage1" },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.HARD, m_Name = "Stage2", m_SceneName = "Stage2" },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.VERYHARD, m_Name = "Stage3", m_SceneName = "Stage3" }
        };

        m_QuestListUI.QuestListOpen(tmpList); // 데이터 불러와서 넣어줘야함
    }

    public void MultiBattelButton()
    {
        List<QuestData> tmpList = new List<QuestData>()
        {
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.NORMAL, m_Name = "MultiBoss1", m_SceneName = "MultiStage1" },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.HARD, m_Name = "MultiBoss2", m_SceneName = "MultiStage2" },
            new QuestData() { m_Icon = null, m_Level = E_QUESTLEVEL.VERYHARD, m_Name = "MultiBoss3", m_SceneName = "MultiStage3" }
        };

        m_QuestListUI.QuestListOpen(tmpList); // 데이터 불러와서 넣어줘야함
    }
}

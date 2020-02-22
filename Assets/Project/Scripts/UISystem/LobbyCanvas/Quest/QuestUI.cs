using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : LobbyUI
{
    QuestListUI m_QuestListUI;

    [SerializeField]
    QuestMultiTypeSelectUI m_QuestMultiTypeSelectUI;
    [SerializeField]
    QuestRoomListUI m_QuestRoomListUI;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_QuestListUI = transform.Find("QuestListUI").GetComponent<QuestListUI>();
        m_QuestListUI.Initialize(_LobbyCanvasUI);

        m_QuestMultiTypeSelectUI.Initialize(_LobbyCanvasUI);
    }

    public void SingleBattleButton()
    {
        List<QuestData> list = DataLoad.QuestSceneDataAllLoad(DataLoad.E_SCENETYPE.SINGLE);
        m_QuestListUI.QuestListOpen(list); // 데이터 불러와서 넣어줘야함
    }

    public void MultiBattelButton()
    {
        List<QuestData> list = DataLoad.QuestSceneDataAllLoad(DataLoad.E_SCENETYPE.MULTIPLAY);
        m_QuestListUI.QuestListOpen(list); // 데이터 불러와서 넣어줘야함
    }

    public void OpenQuestMultiTypeSelectUI(QuestData _Data)
    {
        m_QuestMultiTypeSelectUI.OpenMultiTypeSelectUI(_Data);
    }

    public void RoomListRefresh()
    {
        m_QuestRoomListUI.Refresh();
    }
}

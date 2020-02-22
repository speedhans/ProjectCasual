using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestMultiTypeSelectUI : LobbyUI
{
    [SerializeField]
    QuestRoomListUI m_QuestRoomListUI;

    QuestData m_QuestData;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);
        m_QuestRoomListUI.Initialize(_LobbyCanvasUI);
    }

    public void OpenMultiTypeSelectUI(QuestData _Data)
    {
        Open();
        m_QuestData = _Data;
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }

    public void CreateRoomButton()
    {
        if (GameManager.Instance.m_PlayerData.m_CurrentStamina < m_QuestData.m_StaminaValue) // 네트워크 확인 필요
        {
            MessageBox.CreateOneButtonType("스테미나가 부족합니다");
            return;
        }

        m_LobbyCanvasUI.GetWaitingRoomUI().CreateWaitingRoom(m_QuestData);
    }

    public void JoinRoomButton()
    {
        m_QuestRoomListUI.OpenRoomList(m_QuestData);
    }
}

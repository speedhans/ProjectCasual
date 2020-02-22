using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;

public class QuestRoomSlot : DefaultUI
{
    LobbyCanvasUI m_LobbyCanvasUI;

    [SerializeField]
    UnityEngine.UI.Image m_QuestIcon;
    [SerializeField]
    TMPro.TMP_Text m_QuestRoomName;
    [SerializeField]
    TMPro.TMP_Text m_PlayerCountText;

    QuestData m_QuestData;
    string m_RoomName;
    int m_PlayerCount;

    public void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        m_LobbyCanvasUI = _LobbyCanvasUI;
    }

    public void SetSlotData(QuestData _Data, RoomInfo _Info)
    {
        m_QuestData = _Data;
        m_RoomName = _Info.Name;
        m_PlayerCount = _Info.PlayerCount;
        m_QuestIcon.sprite = _Data.m_Icon;
        m_QuestRoomName.text = m_RoomName;
        m_PlayerCountText.text = m_PlayerCount.ToString();
    }

    public void ClickEvent()
    {
        MessageBox.CreateTwoButtonType("방에 들어가시겠습니까?", "YES", JoinRoom, "NO");
    }

    void JoinRoom()
    {
        if (GameManager.Instance.m_PlayerData.m_CurrentStamina < m_QuestData.m_StaminaValue)
        {
            MessageBox.CreateOneButtonType("스테미나가 부족합니다");
            return;
        }

        m_LobbyCanvasUI.GetWaitingRoomUI().JoinWaitingRoom(m_RoomName);
    }
}

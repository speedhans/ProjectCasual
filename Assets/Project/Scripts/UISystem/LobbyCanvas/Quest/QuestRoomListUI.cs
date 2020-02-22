using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class QuestRoomListUI : LobbyUI
{
    [SerializeField]
    TMPro.TMP_Text m_QuestTypeText;
    [SerializeField]
    QuestRoomSlot[] m_Slot;

    QuestData m_QuestData;

    bool m_RefreshProgress = true;
    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);
        for (int i = 0; i < m_Slot.Length; ++i)
        {
            m_Slot[i].Initialize(_LobbyCanvasUI);
            m_Slot[i].gameObject.SetActive(false);
        }
    }

    public void OpenRoomList(QuestData _QuestData)
    {
        Open();
        m_QuestData = _QuestData;
        m_QuestTypeText.text = m_QuestData.m_Name;
        Refresh();
    }

    void RoomListUpdate(List<RoomInfo> _Rooms)
    {
        if (!m_RefreshProgress) return;
        m_RefreshProgress = false;
        int count = 0;

        for (int i = 0; i < _Rooms.Count; ++i)
        {
            if (_Rooms[i].PlayerCount >= _Rooms[i].MaxPlayers) continue;

            m_Slot[count].SetSlotData(m_QuestData, _Rooms[i]);
            m_Slot[count].gameObject.SetActive(true);
            ++count;
            if (count >= m_Slot.Length)
                break;
        }
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        NetworkManager.Instance.m_RoomUpdateCallback += RoomListUpdate;
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
        NetworkManager.Instance.m_RoomUpdateCallback -= RoomListUpdate;
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.m_RoomUpdateCallback -= RoomListUpdate;
    }

    public override void Refresh()
    {
        if (!gameObject.activeSelf) return;
        base.Refresh();
        if (NetworkManager.Instance.GetRoomList(m_QuestData.m_SceneName))
        {
            m_RefreshProgress = true;
            for (int i = 0; i < m_Slot.Length; ++i)
            {
                m_Slot[i].gameObject.SetActive(false);
            }
        }
    }
}

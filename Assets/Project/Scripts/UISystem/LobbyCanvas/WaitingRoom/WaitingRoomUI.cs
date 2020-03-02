using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class WaitingRoomUI : LobbyUI
{
    public const string m_SceneNameKey = "SceneName";
    public const string m_GameNameKey = "GameName";

    [SerializeField]
    WaitingRoomConnectUI m_WaitingRoomConnectUI;
    [SerializeField]
    WaitingRoomInRoomUI m_WaitingRoomInRoomUI;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_WaitingRoomConnectUI.gameObject.SetActive(false);
        m_WaitingRoomInRoomUI.gameObject.SetActive(false);
    }

    public void CreateWaitingRoom(QuestData _QuestData)
    {
        Open();
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash[m_SceneNameKey] = _QuestData.m_SceneName;
        hash[m_GameNameKey] = _QuestData.m_Name;
        hash[Main_Stage.MultiPlayKey] = _QuestData.m_Multiplay;
        NetworkManager.Instance.CreateRoom(NetworkManager.Instance.CreateInstanceRoomName(), _QuestData.m_SceneName, hash);
    }

    public void JoinWaitingRoom(string _RoomName)
    {
        Open();
        NetworkManager.Instance.JoinRoom(_RoomName);
    }

    void JoinRoomSuccess()
    {
        m_WaitingRoomConnectUI.gameObject.SetActive(false);
        m_WaitingRoomInRoomUI.gameObject.SetActive(true);
        NetworkManager.Instance.RoomController.SetLocalPlayerProperties(WaitingRoomSlot.m_CharacterID, InventoryManager.Instance.GetPlayerModelName());
        m_WaitingRoomInRoomUI.Initialize(this);
    }

    void CreateRoomFailed(short returnCode, string message)
    {
        if (!gameObject.activeSelf) return;
        m_WaitingRoomConnectUI.gameObject.SetActive(false);
        MessageBox.CreateOneButtonType("방을 생성하지 못했습니다. " + returnCode.ToString(), "Close", Close);
    }

    void JoinRoomFailed(short returnCode, string message)
    {
        if (!gameObject.activeSelf) return;
        m_WaitingRoomConnectUI.gameObject.SetActive(false);
        MessageBox.CreateOneButtonType("방에 들어가지 못했습니다. " + returnCode.ToString(), "Close", Close);

        m_LobbyCanvasUI.GetQuestUI().RoomListRefresh();
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        m_WaitingRoomConnectUI.gameObject.SetActive(true);
        NetworkManager.Instance.m_CreateRoomFailedCallback += CreateRoomFailed;
        NetworkManager.Instance.m_JoinRoomCallback += JoinRoomSuccess;
        NetworkManager.Instance.m_JoinRoomFailedCallback += JoinRoomFailed;

        m_LobbyCanvasUI.GetLobbyDefaultUI().BottomUIGroupVisible(false);
    }

    public override void Close()
    {
        base.Close();
        m_WaitingRoomConnectUI.gameObject.SetActive(false);
        m_WaitingRoomInRoomUI.gameObject.SetActive(false);
        NetworkManager.Instance.RoomController.LeaveRoom();

        NetworkManager.Instance.m_CreateRoomFailedCallback -= CreateRoomFailed;
        NetworkManager.Instance.m_JoinRoomCallback -= JoinRoomSuccess;
        NetworkManager.Instance.m_JoinRoomFailedCallback -= JoinRoomFailed;

        m_LobbyCanvasUI.GetLobbyDefaultUI().BottomUIGroupVisible(true);
        gameObject.SetActive(false);
        m_LobbyCanvasUI.GetQuestUI().RoomListRefresh();
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.m_CreateRoomFailedCallback -= CreateRoomFailed;
        NetworkManager.Instance.m_JoinRoomCallback -= JoinRoomSuccess;
        NetworkManager.Instance.m_JoinRoomFailedCallback -= JoinRoomFailed;
    }
}

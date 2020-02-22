using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class WaitingRoomInRoomUI : DefaultUI, IInRoomCallbacks
{
    WaitingRoomUI m_WaitingRoomUI;
    PhotonView m_PhotonView;
    string m_SceneName;
    string m_GameName;

    [SerializeField]
    TMP_Text m_GameNameText;
    [SerializeField]
    TMP_Text m_RoomNameText;
    [SerializeField]
    GameObject m_StartButton;
    [SerializeField]
    GameObject m_ReadyButton;
    [SerializeField]
    GameObject m_CancelButton;
    [SerializeField]
    GameObject m_BackButton;

    [SerializeField]
    WaitingRoomSlot[] m_Slots;
    private void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public void Initialize(WaitingRoomUI _WaitingRoomUI)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            m_StartButton.SetActive(true);
            m_ReadyButton.SetActive(false);
            m_CancelButton.SetActive(false);
        }
        else
        {
            m_StartButton.SetActive(false);
            m_ReadyButton.SetActive(true);
            m_CancelButton.SetActive(false);
        }

        m_WaitingRoomUI = _WaitingRoomUI;
        Room room = PhotonNetwork.CurrentRoom;
        m_RoomNameText.text = room.Name;
        m_SceneName = NetworkManager.Instance.RoomController.GetRoomPropertie<string>(WaitingRoomUI.m_SceneNameKey);
        m_GameName = NetworkManager.Instance.RoomController.GetRoomPropertie<string>(WaitingRoomUI.m_GameNameKey);
        m_GameNameText.text = m_GameName;
        NetworkManager.Instance.RoomController.SetLocalPlayerProperties(WaitingRoomSlot.m_CharacterID, InventoryManager.Instance.GetPlayerModelName());
        NetworkManager.Instance.RoomController.SetLocalPlayerProperties(WaitingRoomSlot.m_ReadyKey, false);
        WaitingRoomUpdateQuery();
    }

    void WaitingRoomUpdateQuery()
    {
        m_PhotonView.RPC("WaitingRoomUpdateQuery_RPC", RpcTarget.All);
    }

    [PunRPC]
    void WaitingRoomUpdateQuery_RPC()
    {
        Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < m_Slots.Length; ++i)
        {
            if (i >= players.Length)
                m_Slots[i].SetSlotData(null);
            else
                m_Slots[i].SetSlotData(players[i]);
        }
    }

    public void StartButton()
    {
        Player[] players = PhotonNetwork.PlayerListOthers;
        bool allready = true;
        foreach(Player p in players)
        {
            if (!NetworkManager.Instance.RoomController.GetOtherPlayerPropertie<bool>(p, WaitingRoomSlot.m_ReadyKey))
                allready = false;
        }

        if (!allready)
        {
            MessageBox.CreateOneButtonType("준비완료 상태가 아닌 플레이어가 있습니다");
            return;
        }

        m_PhotonView.RPC("WaitingRoomGameStart", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void WaitingRoomGameStart()
    {
        SceneManager.Instance.LoadScene(m_SceneName);
    }

    public void ReadyButton()
    {
        m_ReadyButton.SetActive(false);
        m_CancelButton.SetActive(true);
        NetworkManager.Instance.RoomController.SetLocalPlayerProperties(WaitingRoomSlot.m_ReadyKey, true);
        WaitingRoomUpdateQuery();
    }

    public void CancelButton()
    {
        m_ReadyButton.SetActive(true);
        m_CancelButton.SetActive(false);
        NetworkManager.Instance.RoomController.SetLocalPlayerProperties(WaitingRoomSlot.m_ReadyKey, false);
        WaitingRoomUpdateQuery();
    }

    public void BackButton()
    {
        m_WaitingRoomUI.Close();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.IsMasterClient)
        {
            NetworkManager.Instance.RoomController.LeaveRoom();
            MessageBox.CreateOneButtonType("방장이 방에서 나갔습니다", "Close", BackButton);
        }
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {

    }
}

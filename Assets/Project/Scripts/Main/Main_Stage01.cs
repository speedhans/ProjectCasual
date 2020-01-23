using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Main_Stage01 : Main
{
    public TMPro.TMP_Text m_TestText;

    PlayerCharacter m_MyCharacter;

    public Shader m_DissolveShader;

    protected override void Awake()
    {
        base.Awake();
        
        gameObject.AddComponent<FrameChecker>();

        Instantiate(Resources.Load<GameObject>("ControlCanvas"));
        Instantiate(Resources.Load<GameObject>("GameUICanvas"));
        //NetworkManager.Instance.ServerConnet();
        NetworkManager.Instance.CreateRoom("TEST01");
        StartCoroutine(C_Initialize());
    }

    IEnumerator C_Initialize()
    {
        while(m_MyCharacter == null)
        {
            m_MyCharacter = GameManager.Instance.m_MyCharacter;
            yield return null;
        }

        GameObject ui = Instantiate(Resources.Load<GameObject>("SkillUICanvas"), Vector3.zero, Quaternion.identity);
        
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        m_TestText.text = "OnJoinedRoom";
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("PlayerCharacter", Vector3.zero, Quaternion.identity, 0, new object[] { 0,0,0,0,0, InventoryManager.Instance.GetPlayerModel() });
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        m_TestText.text = "OnConnectedToMaster";
    }

    public override void OnConnected()
    {
        base.OnConnected();
        m_TestText.text = "OnConnected";
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        m_TestText.text = "OnJoinedLobby";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);

        NetworkManager.Instance.JoinRoom("TEST01");
    }
}
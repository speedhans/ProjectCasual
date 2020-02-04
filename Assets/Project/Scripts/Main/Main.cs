using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Main : MonoBehaviourPunCallbacks
{
    public VerticalFollowCamera m_Camera;
    Dictionary<int, PlayerCharacter> m_DicPlayerCharacter = new Dictionary<int, PlayerCharacter>();

    public bool IsLoadingComplete { get; protected set; }

    protected virtual void Awake()
    {
        GameManager.Instance.m_Main = this;
    }

    protected virtual void Start()
    {

    }

    // Players Data
    #region Players Data
    public void AddPlayerCharacter(PlayerCharacter _PlayerCharacter)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        m_DicPlayerCharacter.Add(_PlayerCharacter.m_PhotonView.Owner.ActorNumber, _PlayerCharacter);
    }

    public void SubPlayerCharacter(PlayerCharacter _PlayerCharacter)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        m_DicPlayerCharacter.Remove(_PlayerCharacter.m_PhotonView.Owner.ActorNumber);
    }

    public PlayerCharacter GetPlayerCharacter(int _ActorNumber)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return null;
        return m_DicPlayerCharacter[_ActorNumber];
    }

    public PlayerCharacter GetPlayerCharacter(Player _PhotonPlayer)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return null;
        return m_DicPlayerCharacter[_PhotonPlayer.ActorNumber];
    }

    public Player GetPhotonWithPlayerCharacter(int _ActorNumber)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return null;
        return m_DicPlayerCharacter[_ActorNumber].m_PhotonView.Owner;
    }

    #endregion Players Data
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(PhotonView))]
public class Main : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public CameraSystem m_Camera;
    Dictionary<int, PlayerCharacter> m_DicPlayerCharacter = new Dictionary<int, PlayerCharacter>();

    protected PhotonView m_PhotonView;
    public bool IsBeginLoadingComplete { get; protected set; }
    public bool IsAfterLoadingComplete { get; protected set; }

    public AudioClip m_BGMStart;
    public AudioClip m_BGMLoop;

    protected virtual void Awake()
    {
        GameManager.Instance.m_Main = this;
        m_PhotonView = GetComponent<PhotonView>();
    }

    protected virtual void Start()
    {
        if (m_BGMStart != null && m_BGMLoop != null)
            SoundManager.Instance.PlayBGM(m_BGMStart, m_BGMLoop);
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

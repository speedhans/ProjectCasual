using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LoadingManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    UnityEngine.UI.Slider m_Slider;

    private void Awake()
    {
        SceneManager.Instance.m_LoadingManager = this;

        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.IsConnected)
            NetworkManager.Instance.ServerConnet();
    }

    public void SetValue(float _Value)
    {
        m_Slider.value = Mathf.Clamp01(_Value);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        SceneManager.Instance.LoadSceneDirect("Intro");
    }
}

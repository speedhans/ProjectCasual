using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IntroManager : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        Application.targetFrameRate = 60;

        InventoryManager.Instance.LoadItemList();
        NetworkManager.Instance.ServerConnet();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        SceneManager.Instance.LoadScene("Scene_01");
    }
}

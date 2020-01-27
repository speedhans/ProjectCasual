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
        
    }

    public void StartGame()
    {
        SceneManager.Instance.LoadScene("LobbyScene");
    }
}

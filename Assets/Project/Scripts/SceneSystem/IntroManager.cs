using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IntroManager : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        Application.targetFrameRate = 60;

#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = false;
#endif
        GameManager.Instance.m_TestMode = false;

        // 임시 데이터 불러오기
        InventoryManager.Instance.LoadItemList();
        GameManager.Instance.CreatePlayerData();
        TestModeDefaultItemSetting list = Resources.Load<TestModeDefaultItemSetting>("TestModeDefaultItemList");
        list.SetTestDefaultItemInventory();
    }

    public void StartGame()
    {
        SceneManager.Instance.LoadScene("LobbyScene");
    }
}

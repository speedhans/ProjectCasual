using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    LobbyCanvasUI m_LobbyCanvasUI;

    QuestData m_QuestData;
    UnityEngine.UI.Image m_Icon;
    TMPro.TMP_Text m_NameText;
    TMPro.TMP_Text m_LevelText;

    public void Initialize(QuestData _Data, LobbyCanvasUI _LobbyCanvasUI)
    {
        if (!m_Icon) m_Icon = transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();
        if (!m_NameText) m_NameText = transform.Find("Name_Text").GetComponent<TMPro.TMP_Text>();
        if (!m_LevelText) m_LevelText = transform.Find("Level_Text").GetComponent<TMPro.TMP_Text>();

        gameObject.SetActive(true);

        m_QuestData = _Data;
        m_Icon.sprite = _Data.m_Icon;
        m_NameText.text = _Data.m_Name;
        m_LevelText.text = _Data.m_Level.ToString();

        m_LobbyCanvasUI = _LobbyCanvasUI;
    }

    public void QuestSelect()
    {
        if (m_QuestData.m_Multiplay)
        {
            //m_LobbyCanvasUI.GetWaitingRoomUI().CreateWaitingRoom(m_QuestData.m_SceneName);
            m_LobbyCanvasUI.GetQuestUI().OpenQuestMultiTypeSelectUI(m_QuestData);
        }
        else 
        {
            Debug.Log(m_NameText.text);
            MessageBox.CreateTwoButtonType(m_QuestData.m_Name + "미션을 도전하시겠습니까?", "YES", QuestStart);
        }
    }

    public void QuestInformation()
    {
        QuestInformationBox.Create(m_QuestData);
    }

    public void QuestStart()
    {
        if (GameManager.Instance.m_PlayerData.m_CurrentStamina < m_QuestData.m_StaminaValue)
        {
            MessageBox.CreateOneButtonType("스테미나가 부족합니다");
            return;
        }

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        hash[Main_Stage.MultiPlayKey] = m_QuestData.m_Multiplay;
        NetworkManager.Instance.CreateRoom(NetworkManager.Instance.CreateInstanceRoomName(), "single", hash, false, false, 1); // 임시 방 생성 코드
        SceneManager.Instance.LoadScene(m_QuestData.m_SceneName);
    }
}

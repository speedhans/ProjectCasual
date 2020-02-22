using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class WaitingRoomSlot : MonoBehaviour
{
    public const string m_CharacterID = "CharacterID";
    public const string m_ReadyKey = "SlotReady";

    [SerializeField]
    UnityEngine.UI.Image m_CharacterPoster;
    [SerializeField]
    UnityEngine.UI.Image m_CharacterPosterOutLine;
    [SerializeField]
    TMPro.TMP_Text m_PlayerNameText;

    public void SetSlotData(Player _PhotonPlayer)
    {
        if (_PhotonPlayer == null)
        {
            m_CharacterPoster.sprite = null;
            m_PlayerNameText.text = "";
            m_CharacterPosterOutLine.color = Color.black;
        }
        else 
        {
            m_PlayerNameText.text = _PhotonPlayer.NickName;
            string characterid = NetworkManager.Instance.RoomController.GetOtherPlayerPropertie<string>(_PhotonPlayer, m_CharacterID);
            m_CharacterPoster.sprite = Resources.Load<Sprite>("CharacterPoster/" + characterid + "Poster");
            bool ready = NetworkManager.Instance.RoomController.GetOtherPlayerPropertie<bool>(_PhotonPlayer, m_ReadyKey);
            if (_PhotonPlayer.IsMasterClient)
                m_CharacterPosterOutLine.color = Color.green;
            else
                m_CharacterPosterOutLine.color = ready ? Color.yellow : Color.black;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySettingUI : LobbyUI
{
    [SerializeField]
    LoobySettingVolumeUI m_LoobySettingVolumeUI;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_LoobySettingVolumeUI.Initialize(_LobbyCanvasUI);
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }
}

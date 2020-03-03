using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : LobbyUI
{
    [SerializeField]
    LobbySettingUI m_LobbySettingUI;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_LobbySettingUI.Initialize(_LobbyCanvasUI);
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

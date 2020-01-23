using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : DefaultUI
{
    protected LobbyCanvasUI m_LobbyCanvasUI;

    public virtual void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        m_LobbyCanvasUI = _LobbyCanvasUI;
    }

    public override void Open()
    {
        base.Open();

        m_LobbyCanvasUI.AddUIDepth(this);
    }

    public override void Close()
    {
        base.Close();

        m_LobbyCanvasUI.SubUIDepth(this);
    }
}

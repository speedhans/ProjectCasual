using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyDefaultUI : MonoBehaviour
{
    LobbyCanvasUI m_LobbyCanvasUI;

    public void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        m_LobbyCanvasUI = _LobbyCanvasUI;
    }

    public void StatusButton()
    {
        m_LobbyCanvasUI.ResetUIDepth();
        m_LobbyCanvasUI.GetStatusUI().Open();
    }

    public void InventoryButton()
    {
        m_LobbyCanvasUI.ResetUIDepth();
        m_LobbyCanvasUI.GetInventoryUI().Open();
    }

    public void MenuButton()
    {
        m_LobbyCanvasUI.ResetUIDepth();
        m_LobbyCanvasUI.GetMenuUI().Open();
    }

    public void BackButton()
    {
        m_LobbyCanvasUI.CloseLastUIDepth();
    }
}

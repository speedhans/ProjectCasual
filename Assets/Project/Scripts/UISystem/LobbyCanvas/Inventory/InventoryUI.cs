using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : LobbyUI
{
    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);
    }

    public override void Open()
    {
        //base.Open();
        //gameObject.SetActive(true);

        m_LobbyCanvasUI.GetInventoryViewer().InventoryOpen();
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }

    public void RefreshInventoryAllData()
    {
        m_LobbyCanvasUI.GetInventoryViewer().DefaultRefresh();
    }

    public void RefreshInventorySlotData()
    {
        m_LobbyCanvasUI.GetInventoryViewer().RefreshSlots();
    }
}

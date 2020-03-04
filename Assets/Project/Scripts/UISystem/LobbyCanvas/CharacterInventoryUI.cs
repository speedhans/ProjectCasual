using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInventoryUI : LobbyUI
{
    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

    }

    public override void Open()
    {
        m_LobbyCanvasUI.GetInventoryViewer().InventoryOpen(Item.E_TYPE.CHARACTER);
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

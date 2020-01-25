using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : LobbyUI
{
    [SerializeField]
    InventoryByTypeUI m_ItemInventory;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_ItemInventory.Initialize(_LobbyCanvasUI);
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);

        m_ItemInventory.InventoryOpen();
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }
}

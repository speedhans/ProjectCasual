using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUI : LobbyUI
{
    [SerializeField]
    CharacterPreviewUI m_CharacterPreviewUI;
    [SerializeField]
    InventoryByTypeUI m_EquipmentItemInventory;

    EquipmentSlotUI[] m_EquipmentSlotUI = new EquipmentSlotUI[3];

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_CharacterPreviewUI.Initialize();
        m_EquipmentItemInventory.Initialize();

        for (int i = 0; i < 3; ++i)
        {
            m_EquipmentSlotUI[i] = transform.Find("EquipSlot" + (i + 1).ToString()).GetComponent<EquipmentSlotUI>();
            m_EquipmentSlotUI[i].Initialize();
        }
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

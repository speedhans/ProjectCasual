using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUI : LobbyUI
{
    [SerializeField]
    CharacterPreviewUI m_CharacterPreviewUI;

    EquipmentSlotUI[] m_EquipmentSlotUI = new EquipmentSlotUI[3];

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_CharacterPreviewUI.Initialize();

        for (int i = 0; i < 3; ++i)
        {
            m_EquipmentSlotUI[i] = transform.Find("EquipSlot" + (i + 1).ToString()).GetComponent<EquipmentSlotUI>();
            m_EquipmentSlotUI[i].Initialize(_LobbyCanvasUI);
        }
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);

        Refresh();
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }

    public override void Refresh()
    {
        base.Refresh();

        if (!gameObject.activeSelf) return;

        EquipmentItem firstequip = null;
        for (int i = 0; i < m_EquipmentSlotUI.Length; ++i)
        {
            EquipmentItem item = InventoryManager.Instance.GetEquippedItem(i);
            if (item != null && item.GetWeaponModel() != null)
                firstequip = item;
            m_EquipmentSlotUI[i].SetData(item);
        }

        GameObject model = Instantiate(Resources.Load<GameObject>(InventoryManager.Instance.GetPlayerModelName()), Vector3.zero, Quaternion.identity);

        m_CharacterPreviewUI.SetPreviewModel(model);
        m_CharacterPreviewUI.SetPerviewWeapon(firstequip.GetWeaponModel().name, firstequip.GetWeaponType());
    }
}

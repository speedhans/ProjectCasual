using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : CustomTouchEvent
{
    LobbyCanvasUI m_LobbyCanvasUI;

    EquipmentItem m_Item;
    UnityEngine.UI.Image m_ItemImage;

    public void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        m_LobbyCanvasUI = _LobbyCanvasUI;
        m_ItemImage = transform.Find("ItemImage").GetComponent<UnityEngine.UI.Image>();
    }

    public override void OnPointerDown(PointerEventData eventData) // 장비 목록 보는 작업 필요
    {
        base.OnPointerDown(eventData);

        if (m_Item != null)
        {
            m_LobbyCanvasUI.GetItemDataViewer().SetData(m_Item);
            m_LobbyCanvasUI.GetItemDataViewer().Open();
        }
    }

    public void SetData(EquipmentItem _Item)
    {
        m_Item = _Item;
        if (m_Item != null)
            m_ItemImage.sprite = m_Item.m_ItemImage;
        else
            m_ItemImage.sprite = null;
    }
}

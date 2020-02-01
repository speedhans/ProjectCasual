using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : CustomTouchEvent
{
    [SerializeField]
    Sprite m_StarBlack;
    [SerializeField]
    Sprite m_StarGold;
    UnityEngine.UI.Image[] m_Stars;

    LobbyCanvasUI m_LobbyCanvasUI;

    EquipmentItem m_Item;
    UnityEngine.UI.Image m_ItemImage;

    public void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        m_LobbyCanvasUI = _LobbyCanvasUI;
        m_ItemImage = transform.Find("ItemImage").GetComponent<UnityEngine.UI.Image>();

        m_Stars = new UnityEngine.UI.Image[Common.MAXREINFORECEVALUE];
        for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
        {
            m_Stars[i] = transform.Find("Star " + (i + 1).ToString()).GetComponent<UnityEngine.UI.Image>();
        }
    }

    public override void OnPointerDown(PointerEventData eventData) // 장비 목록 보는 작업 필요
    {
        base.OnPointerDown(eventData);

        if (m_Item != null)
        {
            m_LobbyCanvasUI.GetItemDataViewer().SetData(m_Item);
            m_LobbyCanvasUI.GetItemDataViewer().Open();
        }
        else
        {
            m_LobbyCanvasUI.GetInventoryViewer().InventoryOpen(Item.E_TYPE.EQUIPMENT);
        }
    }

    public void SetData(EquipmentItem _Item)
    {
        m_Item = _Item;
        if (!m_Item)
        {
            m_ItemImage.sprite = null;
            for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
            {
                m_Stars[i].gameObject.SetActive(false);
            }
            return;
        }

        m_ItemImage.sprite = m_Item.m_ItemImage;
        for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
        {
            m_Stars[i].gameObject.SetActive(true);
            if (m_Item.GetReinforceCount() > i)
                m_Stars[i].sprite = m_StarGold;
            else
                m_Stars[i].sprite = m_StarBlack;
        }
    }
}

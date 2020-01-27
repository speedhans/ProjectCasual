using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    Sprite m_OutlineBlack;
    [SerializeField]
    Sprite m_OutlineYellow;

    LobbyCanvasUI m_LobbyCanvasUI;

    Item m_Item;
    UnityEngine.UI.Image m_ItemImage;
    UnityEngine.UI.Image m_OutlineBoxImage;

    public void Initialize(Item _Item, LobbyCanvasUI _LobbyCanvasUI)
    {
        gameObject.SetActive(true);
        m_Item = _Item;
        m_LobbyCanvasUI = _LobbyCanvasUI;

        if (!m_ItemImage) m_ItemImage = transform.Find("ItemImage").GetComponent<UnityEngine.UI.Image>();
        if (!m_OutlineBoxImage) m_OutlineBoxImage = transform.Find("OutLineImage").GetComponent<UnityEngine.UI.Image>();

        m_ItemImage.sprite = _Item.m_ItemImage;

        if (_Item.m_EquipState.IsEquip)
            m_OutlineBoxImage.sprite = m_OutlineYellow;
        else
            m_OutlineBoxImage.sprite = m_OutlineBlack;
    }

    public void ItemDataView()
    {
        m_LobbyCanvasUI.GetItemDataViewer().SetData(m_Item);
        m_LobbyCanvasUI.GetItemDataViewer().Open();
    }

    public void Refresh()
    {
        if (m_Item.m_EquipState.IsEquip)
            m_OutlineBoxImage.sprite = m_OutlineYellow;
        else
            m_OutlineBoxImage.sprite = m_OutlineBlack;
    }
}

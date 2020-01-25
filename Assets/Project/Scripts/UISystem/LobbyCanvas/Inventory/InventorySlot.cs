using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    LobbyCanvasUI m_LobbyCanvasUI;

    Item m_Item;
    UnityEngine.UI.Image m_ItemImage;

    public void Initialize(Item _Item, LobbyCanvasUI _LobbyCanvasUI)
    {
        gameObject.SetActive(true);
        m_Item = _Item;
        m_LobbyCanvasUI = _LobbyCanvasUI;

        if (!m_ItemImage) m_ItemImage = transform.Find("ItemImage").GetComponent<UnityEngine.UI.Image>();

        m_ItemImage.sprite = _Item.m_ItemImage;
    }

    public void ItemDataView()
    {
        m_LobbyCanvasUI.GetItemDataViewer().SetData(m_Item);
        m_LobbyCanvasUI.GetItemDataViewer().Open();
    }
}

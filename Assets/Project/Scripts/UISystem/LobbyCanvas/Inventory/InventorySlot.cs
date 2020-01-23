using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    UnityEngine.UI.Image m_ItemImage;

    public void Initialize(Item _Item)
    {
        gameObject.SetActive(true);

        if (!m_ItemImage) m_ItemImage = transform.Find("ItemImage").GetComponent<UnityEngine.UI.Image>();

        m_ItemImage.sprite = _Item.m_ItemImage;
    }
}

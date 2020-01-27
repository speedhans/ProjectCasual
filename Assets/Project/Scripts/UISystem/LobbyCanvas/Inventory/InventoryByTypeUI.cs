using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryByTypeUI : MonoBehaviour
{
    LobbyCanvasUI m_LobbyCanvasUI;

    [SerializeField]
    Item.E_TYPE m_InventoryType = Item.E_TYPE.NONE;

    [SerializeField]
    GameObject m_SlotPrefab;

    Transform m_Grid;
    List<InventorySlot> m_ListInventorySlot = new List<InventorySlot>();
    int m_CurrentInventoryCount;

    public void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        m_LobbyCanvasUI = _LobbyCanvasUI;

        m_Grid = transform.Find("MaskField/Grid");
    }

    public void InventoryOpen()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (m_Grid == null) m_Grid = transform.Find("MaskField/Grid");

        List<Item> list = m_InventoryType == Item.E_TYPE.NONE ? InventoryManager.Instance.GetItemList() : InventoryManager.Instance.GetTypeItemList(m_InventoryType);
        m_CurrentInventoryCount = list.Count;

        if (m_CurrentInventoryCount > m_Grid.childCount)
        {
            int interval = m_CurrentInventoryCount - m_Grid.childCount;

            for (int i = 0; i < interval; ++i)
            {
                GameObject g = Instantiate(m_SlotPrefab, m_Grid);
                m_ListInventorySlot.Add(g.GetComponent<InventorySlot>());
            }
        }
        else if (m_CurrentInventoryCount < m_Grid.childCount)
        {
            for (int i = m_CurrentInventoryCount - 1; i < m_Grid.childCount; ++i)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < m_CurrentInventoryCount; ++i)
        {
            m_ListInventorySlot[i].Initialize(list[i], m_LobbyCanvasUI);
        }
    }

    public void RefreshSlots()
    {
        for (int i = 0; i < m_CurrentInventoryCount; ++i)
        {
            m_ListInventorySlot[i].Refresh();
        }
    }
}

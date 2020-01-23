using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryByTypeUI : MonoBehaviour
{
    [SerializeField]
    Item.E_TYPE m_InventoryType = Item.E_TYPE.NONE;

    [SerializeField]
    GameObject m_SlotPrefab;

    Transform m_Grid;
    List<InventorySlot> m_ListInventorySlot = new List<InventorySlot>();

    public void Initialize()
    {
        m_Grid = transform.Find("MaskField/Grid");
    }

    public void InventoryOpen()
    {
        if (m_Grid == null) m_Grid = transform.Find("MaskField/Grid");

        List<Item> list = m_InventoryType == Item.E_TYPE.NONE ? InventoryManager.Instance.GetItemList() : InventoryManager.Instance.GetTypeItemList(m_InventoryType);

        if (list.Count > m_Grid.childCount)
        {
            int interval = list.Count - m_Grid.childCount;

            for (int i = 0; i < interval; ++i)
            {
                GameObject g = Instantiate(m_SlotPrefab, m_Grid);
                m_ListInventorySlot.Add(g.GetComponent<InventorySlot>());
            }
        }
        else if (list.Count < m_Grid.childCount)
        {
            for (int i = list.Count - 1; i < m_Grid.childCount; ++i)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < list.Count; ++i)
        {
            m_ListInventorySlot[i].Initialize(list[i]);
        }
    }
}

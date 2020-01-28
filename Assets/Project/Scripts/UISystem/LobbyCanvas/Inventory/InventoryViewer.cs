using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryViewer : LobbyUI
{
    static public List<InventoryViewer> ViewerList = new List<InventoryViewer>();
    static public void RefreshAllSlots()
    {
        foreach(InventoryViewer v in ViewerList)
        {
            v.RefreshSlots();
        }
    }

    [SerializeField]
    GameObject m_SlotPrefab;

    Transform m_Grid;
    List<InventorySlot> m_ListInventorySlot = new List<InventorySlot>();
    int m_CurrentInventoryCount;

    Item.E_TYPE m_CurrentTargetType;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        ViewerList.Add(this);

        m_Grid = transform.Find("MaskField/Grid");
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

    public void InventoryOpen(Item.E_TYPE _FindType = Item.E_TYPE.NONE)
    {
        Open();
        m_CurrentTargetType = _FindType;
        Refresh();
    }

    public void InventoryOpenTheTypeToID(int _ItemID, int[] _ExclusionNumber = null)
    {
        Open();
        m_CurrentTargetType = Item.E_TYPE.NONE;
        SortItemRefresh(_ItemID, _ExclusionNumber);
    }

    public void Refresh()
    {
        if (m_Grid == null) m_Grid = transform.Find("MaskField/Grid");

        List<Item> list = m_CurrentTargetType == Item.E_TYPE.NONE ? InventoryManager.Instance.GetItemList() : InventoryManager.Instance.GetTypeItemList(m_CurrentTargetType);
        if (list == null)
        {
            for (int i = 0; i < m_Grid.childCount; ++i)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
            return;
        }

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
            m_ListInventorySlot[i].Initialize(list[i],InventorySlot.E_USETYPE.DEFAULT,  m_LobbyCanvasUI);
        }
    }

    public void SortItemRefresh(int _ItemID, int[] _ExclusionNumber)
    {
        if (m_Grid == null) m_Grid = transform.Find("MaskField/Grid");

        List<Item> list = InventoryManager.Instance.GetTypeItemList(_ItemID);
        if (list == null)
        {
            for (int i = 0; i < m_Grid.childCount; ++i)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
            return;
        }
        else
        {
            if (_ExclusionNumber != null)
            {
                int count = _ExclusionNumber.Length;
                while (count > 0)
                {
                    int number = 0;
                    for (int i = 0; i < list.Count - number; ++i)
                    {
                        if (list[i].m_UniqueID == _ExclusionNumber[number])
                        {
                            list.RemoveAt(i);
                            --count;
                            ++number;
                        }
                    }
                }
            }
        }

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
            m_ListInventorySlot[i].Initialize(list[i], InventorySlot.E_USETYPE.REINFORCE, m_LobbyCanvasUI);
        }
    }

    public void RefreshSlots()
    {
        if (!gameObject.activeSelf) return;

        for (int i = 0; i < m_CurrentInventoryCount; ++i)
        {
            m_ListInventorySlot[i].Refresh();
        }
    }
}

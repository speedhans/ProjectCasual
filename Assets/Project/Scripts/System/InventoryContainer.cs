using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryContainer : MonoBehaviour
{
    Dictionary<int, List<Item>> m_DicItemData = new Dictionary<int, List<Item>>();

    public void InsertItem(Item _Item)
    {
        List<Item> list;
        if (m_DicItemData.TryGetValue(_Item.m_ItemID, out list))
        {
            if (_Item.m_IsStockable)
            {
                for (int i = 0; i < list.Count; ++i)
                {
                    if (_Item.m_StockCount < 1) return;

                    if (list[i].m_StockCount < list[i].m_MaxStockCount)
                    {
                        list[i].m_StockCount += _Item.m_StockCount;
                        if (list[i].m_StockCount > list[i].m_MaxStockCount)
                        {
                            _Item.m_StockCount = list[i].m_MaxStockCount - list[i].m_StockCount;
                            list[i].m_StockCount = list[i].m_MaxStockCount;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                list.Add(_Item);
                _Item.transform.SetParent(transform);
            }
        }
        else
        {
            list = new List<Item>();
            list.Add(_Item);
            m_DicItemData.Add(_Item.m_ItemID, list);
            _Item.transform.SetParent(transform);
        }
    }

    public void DeleteItem(int _ItemID, int _UniqueID)
    {
        List<Item> list;
        if (m_DicItemData.TryGetValue(_ItemID, out list))
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].m_UniqueID == _UniqueID)
                {
                    Item item = list[i];
                    list.RemoveAt(i);
                    item.IsDestoryed = true;
                    Destroy(item.gameObject);
                    return;
                }
            }
        }
    }

    public void DeleteItem(Item _Item)
    {
        List<Item> list;
        if (m_DicItemData.TryGetValue(_Item.m_ItemID, out list))
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].m_UniqueID == _Item.m_UniqueID)
                {
                    Item item = list[i];
                    list.RemoveAt(i);
                    item.IsDestoryed = true;
                    Destroy(item.gameObject);
                    return;
                }
            }
        }
    }

    public Item FindItem(int _ItemID, int _UniqueID)
    {
        List<Item> list;
        if (m_DicItemData.TryGetValue(_ItemID, out list))
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].m_UniqueID == _UniqueID)
                {
                    return list[i];
                }
            }
        }

        return null;
    }

    public List<Item> FindItemsToID(int _ItemID)
    {
        List<Item> list;
        if (m_DicItemData.TryGetValue(_ItemID, out list))
        {
            List<Item> req = new List<Item>();
            for (int i = 0; i < list.Count; ++i)
            {
                req.Add(list[i]);
            }
            return req;
        }

        return null;
    }

    public List<Item> GetItemList()
    {
        List<Item> list = new List<Item>();

        foreach (List<Item> l in m_DicItemData.Values)
        {
            for (int i = 0; i < l.Count; ++i)
            {
                list.Add(l[i]);
            }
        }

        return list;
    }

    public List<Item> GetTypeItemList(Item.E_TYPE _Type)
    {
        List<Item> list = new List<Item>();

        foreach(List<Item> l in m_DicItemData.Values)
        {
            if (l[0].GetItemType() == _Type)
            {
                for (int i = 0; i < l.Count; ++i)
                {
                    list.Add(l[i]);
                }
            }
        }

        return list;
    }

    public int GetAllItemCount()
    {
        int count = 0;
        foreach(List<Item> l in m_DicItemData.Values)
        {
            count += l.Count;
        }

        return count;
    }
}

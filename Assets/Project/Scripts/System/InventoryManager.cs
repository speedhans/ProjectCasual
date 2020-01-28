using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    static GameObject _gameobject = null;
    static InventoryManager single = null;
    public static InventoryManager Instance
    {
        get
        {
            if (!single)
            {
                _gameobject = new GameObject("InventoryManager");
                single = _gameobject.AddComponent<InventoryManager>();
                single.Initialize();
            }

            return single;
        }

        private set { }
    }

    InventoryContainer m_InventoryContainer;

    void Initialize()
    {
        DontDestroyOnLoad(gameObject);

        GameObject g = new GameObject("InventoryContainer");
        g.transform.SetParent(transform);
        m_InventoryContainer = g.AddComponent<InventoryContainer>();
        g.SetActive(false);
    }

    EquipmentItem[] m_EquipItem = new EquipmentItem[3];
    string m_PlayerModel = "UnityChan/UnityChan";

    public void LoadItemList()
    {
        GameObject o = Instantiate(Resources.Load<GameObject>("BroadSword"));
        m_EquipItem[0] = o.GetComponent<EquipmentItem>();

        o = Instantiate(Resources.Load<GameObject>("FireBlade"));
        m_EquipItem[1] = o.GetComponent<EquipmentItem>();

        o = Instantiate(Resources.Load<GameObject>("FrameMagicBook"));
        m_EquipItem[2] = o.GetComponent<EquipmentItem>();

        for (int i = 0; i < m_EquipItem.Length; ++i)
        {
            m_EquipItem[i].SetEquip(true, i);
            InserItem(m_EquipItem[i]);
        }

        o = Instantiate(Resources.Load<GameObject>("Boots"));
        InserItem(o.GetComponent<Item>());
    }

    public EquipmentItem GetEquippedItem(int _Number) { return m_EquipItem[_Number]; }
    public void SetEquipmentItem(int _Number, EquipmentItem _Item)
    {
        m_EquipItem[_Number] = _Item;
        _Item.SetEquip(true, _Number);
    }

    public void EquipmentOff(int _Number)
    {
        if (m_EquipItem[_Number])
        {
            m_EquipItem[_Number].SetEquip(false);
            m_EquipItem[_Number] = null;
        }
    }

    public int EquipmentSlotCount() { return m_EquipItem.Length; }
    public string GetPlayerModel() { return m_PlayerModel; }

    public List<Item> GetItemList()
    {
        return m_InventoryContainer.GetItemList();
    }
    public List<Item> GetTypeItemList(Item.E_TYPE _Type)
    {
        return m_InventoryContainer.GetTypeItemList(_Type);
    }
    public List<Item> GetTypeItemList(int _ItemID)
    {
        return m_InventoryContainer.FindItemsToID(_ItemID);
    }

    public void InserItem(Item _Item)
    {
        m_InventoryContainer.InsertItem(_Item);
    }
}

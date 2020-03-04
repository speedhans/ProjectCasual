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
    CharacterItem m_EquipCharacterItem;

    public void LoadItemList()
    {
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

    public void SetEquipmentCharacterItem(CharacterItem _Item)
    {
        m_EquipCharacterItem = _Item;
        _Item.SetEquip(true);
    }

    public void EquipmentOffCharacterItem()
    {
        if (m_EquipCharacterItem)
        {
            m_EquipCharacterItem.SetEquip(false);
            m_EquipCharacterItem = null;
        }
    }

    public int EquipmentSlotCount() { return m_EquipItem.Length; }
    public CharacterItem GetEquipmentCharacterData() { return m_EquipCharacterItem; }
    public string GetPlayerModelName()
    {
        if (m_EquipCharacterItem)
        {
            return m_EquipCharacterItem.GetModelName();
        }

        return ""; 
    }

    public List<Item> GetItemList()
    {
        return m_InventoryContainer.GetItemList();
    }
    public List<Item> GetTypeItemList(Item.E_TYPE _Type)
    {
        return m_InventoryContainer.GetTypeItemList(_Type);
    }
    public List<Item> GetTypeItemList(Item.E_TYPE _Type, int _ItemID)
    {
        return m_InventoryContainer.FindItemsToID(_Type, _ItemID);
    }

    public void InserItem(Item _Item)
    {
        m_InventoryContainer.InsertItem(_Item);
    }

    public void InserItem(Item[] _Items)
    {
        for (int i = 0; i < _Items.Length; ++i)
        {
            m_InventoryContainer.InsertItem(_Items[i]);
        }
    }

    public void InserItem(List<Item> _Items)
    {
        for (int i = 0; i < _Items.Count; ++i)
        {
            m_InventoryContainer.InsertItem(_Items[i]);
        }
    }

    public void DestroyItem(Item _Item)
    {
        m_InventoryContainer.DeleteItem(_Item);
    }

    public void DestroyItem(Item.E_TYPE _Type, int _ItemID, int _UniqueID)
    {
        m_InventoryContainer.DeleteItem(_Type, _ItemID, _UniqueID);
    }
}

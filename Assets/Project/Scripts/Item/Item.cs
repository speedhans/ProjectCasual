using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum E_TYPE
    {
        NONE,
        EQUIPMENT,
        CONSUME,
    }

    protected E_TYPE m_Type;

    public struct S_EQUIP
    {
        public bool IsEquip;
        public int SlotNumber;
    }

    public S_EQUIP m_EquipState;

    public int m_ItemID { get; private set; }
    public int m_UniqueID { get; private set; }
    public string m_ItemName;
    public Sprite m_ItemImage;

    public bool m_IsStockable = false;
    public int m_UsageCount = 1;
    public int m_MaxUsageCount = 1;

    [TextArea]
    public string m_DefaultManual;

    System.Action m_UseAction;

    protected virtual void Awake()
    {
        m_Type = E_TYPE.NONE;
    }

    protected void AddUseAction(System.Action _UseAction) { m_UseAction += _UseAction; }
    public void UseItem() { m_UseAction?.Invoke(); }
    public E_TYPE GetItemType() { return m_Type; }
}

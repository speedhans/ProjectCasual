using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public enum E_USETYPE
    {
        DEFAULT,
        REINFORCE,
    }

    E_USETYPE m_UseType = E_USETYPE.DEFAULT;

    [SerializeField]
    Sprite m_OutlineBlack;
    [SerializeField]
    Sprite m_OutlineYellow;
    [SerializeField]
    Sprite m_StarBlack;
    [SerializeField]
    Sprite m_StarGold;

    UnityEngine.UI.Image[] m_Stars;
    [SerializeField]
    TMPro.TMP_Text m_StockCount;

    LobbyCanvasUI m_LobbyCanvasUI;

    Item m_Item;
    UnityEngine.UI.Image m_ItemImage;
    UnityEngine.UI.Image m_OutlineBoxImage;

    public void Initialize(Item _Item, E_USETYPE _UseType, LobbyCanvasUI _LobbyCanvasUI)
    {
        m_UseType = _UseType;
        gameObject.SetActive(true);
        m_Item = _Item;
        m_LobbyCanvasUI = _LobbyCanvasUI;

        if (!m_ItemImage) m_ItemImage = transform.Find("ItemImage").GetComponent<UnityEngine.UI.Image>();
        if (!m_OutlineBoxImage) m_OutlineBoxImage = transform.Find("OutLineImage").GetComponent<UnityEngine.UI.Image>();

        m_Stars = new UnityEngine.UI.Image[Common.MAXREINFORECEVALUE];
        for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
        {
            m_Stars[i] = transform.Find("Star " + (i + 1).ToString()).GetComponent<UnityEngine.UI.Image>();
        }

        m_ItemImage.sprite = _Item.m_ItemImage;

        if (m_Item.GetItemType() == Item.E_TYPE.EQUIPMENT)
        {
            EquipmentItem equip = _Item as EquipmentItem;

            if (equip.m_EquipState.IsEquip)
                m_OutlineBoxImage.sprite = m_OutlineYellow;
            else
                m_OutlineBoxImage.sprite = m_OutlineBlack;

            for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
            {
                m_Stars[i].gameObject.SetActive(true);
                if (equip.GetReinforceCount() > i)
                    m_Stars[i].sprite = m_StarGold;
                else
                    m_Stars[i].sprite = m_StarBlack;
            }

            m_StockCount.gameObject.SetActive(false);
        }
        else if (m_Item.GetItemType() == Item.E_TYPE.CONSUME)
        {
            for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
            {
                m_Stars[i].gameObject.SetActive(false);
            }

            m_StockCount.gameObject.SetActive(true);
            m_StockCount.text = m_Item.m_StockCount.ToString();
        }
    }

    public void SlotClickEvent()
    {
        if (m_UseType == E_USETYPE.REINFORCE)
        {
            MessageBox.CreateTwoButtonType("강화 하시겠습니까?", "YES", ReinforceItem, "NO");
        }
        else
        {
            m_LobbyCanvasUI.GetItemDataViewer().SetData(m_Item);
            m_LobbyCanvasUI.GetItemDataViewer().Open();
        }
    }

    public void Refresh()
    {
        if (m_Item == null || m_Item.IsDestoryed) gameObject.SetActive(false);

        if (m_Item.GetItemType() == Item.E_TYPE.EQUIPMENT)
        {
            EquipmentItem equip = m_Item as EquipmentItem;

            if (equip.m_EquipState.IsEquip)
                m_OutlineBoxImage.sprite = m_OutlineYellow;
            else
                m_OutlineBoxImage.sprite = m_OutlineBlack;

            for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
            {
                if (equip.GetReinforceCount() > i)
                    m_Stars[i].sprite = m_StarGold;
                else
                    m_Stars[i].sprite = m_StarBlack;
            }
        }
        else if (m_Item.GetItemType() == Item.E_TYPE.CONSUME)
        {
            m_StockCount.text = m_Item.m_StockCount.ToString();
        }
    }

    void ReinforceItem()
    {
        EquipmentItem item = m_Item as EquipmentItem;
        EquipmentItem titem = ItemDataViewer.DefaultInstance.GetCurrentTargetItem() as EquipmentItem;

        if (!item || !titem) return;

        if (item.m_EquipState.IsEquip)
        {
            MessageBox.CreateOneButtonType("장착중인 아이템은 강화재료로 사용 할 수 없습니다");
            return;
        }

        if (item.GetReinforceCount() > 0)
        {
            MessageBox.CreateTwoButtonType("이 아이템은 강화가 되어있습니다. 정말로 재료로 사용하시겠습니까?", "YES", ReinforceItemSuccess, "NO");
            return;
        }

        ReinforceItemSuccess();
    }

    void ReinforceItemSuccess()
    {
        EquipmentItem item = m_Item as EquipmentItem;
        EquipmentItem titem = ItemDataViewer.DefaultInstance.GetCurrentTargetItem() as EquipmentItem;

        if (!item || !titem) return;

        titem.IncreaseReinforceCount(item.GetReinforceCount() + 1);
        InventoryManager.Instance.DestroyItem(m_Item);
        InventoryViewer.RefreshAllSlots();
        ItemDataViewer.DefaultInstance.Refresh();
    }
}

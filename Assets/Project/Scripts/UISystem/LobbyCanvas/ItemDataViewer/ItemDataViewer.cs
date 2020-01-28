using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDataViewer : LobbyUI
{
    [SerializeField]
    TMP_Text m_NameText;
    [SerializeField]
    UnityEngine.UI.Image m_IconImage;
    [SerializeField]
    TMP_Text m_StockText;
    [SerializeField]
    TMP_Text m_DefaultManualText;
    [SerializeField]
    TMP_Text m_EffectManualText;

    [SerializeField]
    GameObject m_EquipmentButton;
    [SerializeField]
    GameObject m_EquipmentOffButton;

    [SerializeField]
    Sprite m_StarBlack;
    [SerializeField]
    Sprite m_StarGold;
    UnityEngine.UI.Image[] m_Stars;
    [SerializeField]
    InventoryViewer m_InventoryViewer;

    Item m_Item;

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_Stars = new UnityEngine.UI.Image[Common.MAXREINFORECEVALUE];
        for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
        {
            m_Stars[i] = transform.Find("Background/Star " + (i + 1).ToString()).GetComponent<UnityEngine.UI.Image>();
        }

        m_InventoryViewer.Initialize(_LobbyCanvasUI);
    }

    public void SetData(Item _Item)
    {
        m_Item = _Item;

        m_NameText.text = m_Item.m_ItemName;
        m_IconImage.sprite = m_Item.m_ItemImage;
        if (m_Item.m_IsStockable)
        {
            m_StockText.gameObject.SetActive(true);
            m_StockText.text = "보유수: " + m_Item.m_StockCount.ToString();
        }
        else
        {
            m_StockText.gameObject.SetActive(false);
        }

        if (m_Item.GetItemType() == Item.E_TYPE.EQUIPMENT)
        {
            EquipmentItem eitem = m_Item as EquipmentItem;
            if (eitem)
            {
                for (int i = 0; i < Common.MAXREINFORECEVALUE; ++i)
                {
                    if (eitem.GetReinforceCount() > i)
                        m_Stars[i].sprite = m_StarGold;
                    else
                        m_Stars[i].sprite = m_StarBlack;
                }

                if (eitem.GetState().IsEquip)
                {
                    m_EquipmentButton.SetActive(false);
                    m_EquipmentOffButton.SetActive(true);
                }
                else
                {
                    m_EquipmentButton.SetActive(true);
                    m_EquipmentOffButton.SetActive(false);
                }

                string defualtmanual = m_Item.m_DefaultManual;

                for (int i = 0; i < (int)EquipmentItem.E_ITEMSTATE.MAX; ++i)
                {
                    defualtmanual = defualtmanual.Replace("&" + i.ToString() + "&", ((int)(eitem.GetEquipmentItemState((EquipmentItem.E_ITEMSTATE)i))).ToString());
                }
                m_DefaultManualText.text = defualtmanual;
                m_EffectManualText.text = eitem.GetActiveSkillManualText();

                string[] strAry = eitem.GetPassiveSkillManualText();
                if (strAry != null && strAry.Length > 0)
                {
                    for (int i = 0; i < strAry.Length; ++i)
                    {
                        m_EffectManualText.text += "\n" + strAry[i];
                    }
                }
            }
            else
            {
                m_DefaultManualText.text = m_Item.m_DefaultManual;
                m_EffectManualText.text = "";
            }
        }
        else
        {
            m_EquipmentButton.SetActive(false);
            m_EquipmentOffButton.SetActive(false);

            m_DefaultManualText.text = m_Item.m_DefaultManual;
            m_EffectManualText.text = "";
        }
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

    public void EquipmentButton()
    {
        EquipmentItem equip = m_Item as EquipmentItem;
        if (!equip) return;

        bool complete = false;
        for (int i = 0; i < InventoryManager.Instance.EquipmentSlotCount(); ++i)
        {
            if (!InventoryManager.Instance.GetEquippedItem(i))
            {
                InventoryManager.Instance.SetEquipmentItem(i, equip);
                m_EquipmentButton.SetActive(false);
                m_EquipmentOffButton.SetActive(true);
                complete = true;
                break;
            }
        }

        if (!complete)
        {
            InventoryManager.Instance.EquipmentOff(InventoryManager.Instance.EquipmentSlotCount() - 1);
            InventoryManager.Instance.SetEquipmentItem(InventoryManager.Instance.EquipmentSlotCount() - 1, equip);
            m_EquipmentButton.SetActive(false);
            m_EquipmentOffButton.SetActive(true);
        }

        m_LobbyCanvasUI.GetStatusUI().RefreshSlotDatas();
        m_LobbyCanvasUI.GetInventoryUI().RefreshInventorySlotData();
    }

    public void EquipmentOffButton()
    {
        EquipmentItem equip = m_Item as EquipmentItem;
        if (equip)
        {
            InventoryManager.Instance.EquipmentOff(equip.GetState().SlotNumber);
            m_EquipmentButton.SetActive(true);
            m_EquipmentOffButton.SetActive(false);
        }

        m_LobbyCanvasUI.GetStatusUI().RefreshSlotDatas();
        m_LobbyCanvasUI.GetInventoryUI().RefreshInventorySlotData();
    }

    public void ReinforceMenuButton()
    {
        m_InventoryViewer.InventoryOpenTheTypeToID(m_Item.m_ItemID, new int[] { m_Item.m_UniqueID });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestInformationBox : MonoBehaviour
{
    static GameObject m_QuestInformationBoxPoolObject = null;

    [SerializeField]
    UnityEngine.UI.Image m_QuestIcon;
    [SerializeField]
    TMP_Text m_QuestNameText;
    [SerializeField]
    TMP_Text m_StaminaValueText;
    [SerializeField]
    UnityEngine.UI.Image[] m_IconList;

    static public void Create(QuestData _Data)
    {
        GameObject box = null;

        if (m_QuestInformationBoxPoolObject && m_QuestInformationBoxPoolObject.transform.childCount > 0)
        {
            box = m_QuestInformationBoxPoolObject.transform.GetChild(0).gameObject;
        }
        else
        {
            box = Instantiate(Resources.Load<GameObject>("QuestInformationBox"));
        }

        if (box)
        {
            QuestInformationBox qb = box.GetComponent<QuestInformationBox>();
            if (qb)
            {
                qb.Initialzie(_Data);
            }
        }
    }

    void Initialzie(QuestData _Data)
    {
        gameObject.SetActive(true);

        m_QuestIcon.sprite = _Data.m_Icon;
        m_QuestNameText.text = _Data.m_Name;
        m_StaminaValueText.text = _Data.m_StaminaValue.ToString();

        for (int i = 0; i < m_IconList.Length; ++i)
        {
            if (i >= _Data.m_ListDropItem.Count)
                m_IconList[i].gameObject.SetActive(false);
            else
            {
                m_IconList[i].gameObject.SetActive(true);
                m_IconList[i].sprite = _Data.m_ListDropItem[i].m_Item.m_ItemImage;
            }
        }
    }

    public void CloseButton()
    {
        PushToPool();
    }

    void PushToPool()
    {
        if (m_QuestInformationBoxPoolObject == null)
        {
            m_QuestInformationBoxPoolObject = new GameObject("QuestInformationBoxPool");
        }

        transform.SetParent(m_QuestInformationBoxPoolObject.transform);
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSlot : MonoBehaviour
{
    public enum E_QUESTLEVEL
    {
        NORMAL,
        HARD,
        VERYHARD,
    }

    UnityEngine.UI.Image m_Icon;
    TMPro.TMP_Text m_NameText;
    TMPro.TMP_Text m_LevelText;

    public void Initialize(QuestData _Data)
    {
        if (!m_Icon) m_Icon = transform.Find("Icon").GetComponent<UnityEngine.UI.Image>();
        if (!m_NameText) m_NameText = transform.Find("Name_Text").GetComponent<TMPro.TMP_Text>();
        if (!m_LevelText) m_LevelText = transform.Find("Level_Text").GetComponent<TMPro.TMP_Text>();

        gameObject.SetActive(true);

        m_Icon.sprite = _Data.m_Icon;
        m_NameText.text = _Data.m_Name;
        m_LevelText.text = _Data.m_Level.ToString();
    }

    public void QuestSelect()
    {
        Debug.Log(m_NameText.text);
    }

    public void QuestDropItemView()
    {
        Debug.Log(m_NameText.text + " : itemlist" );
    }
}

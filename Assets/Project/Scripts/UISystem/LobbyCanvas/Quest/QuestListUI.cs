using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListUI : LobbyUI
{
    [SerializeField]
    GameObject m_QuestSlotPrefab;

    Transform m_Grid;
    List<QuestSlot> m_ListQuestSlot = new List<QuestSlot>();

    public override void Initialize(LobbyCanvasUI _LobbyCanvasUI)
    {
        base.Initialize(_LobbyCanvasUI);

        m_Grid = transform.Find("MaskField/Grid");
    }

    public void QuestListOpen(List<QuestData> _DataList)
    {
        gameObject.SetActive(true);
        m_LobbyCanvasUI.ResetUIDepth();
        m_LobbyCanvasUI.AddUIDepth(this);

        if (m_Grid.childCount < _DataList.Count)
        {
            int interval = _DataList.Count - m_Grid.childCount;

            for (int i = 0; i < interval; ++i)
            {
                GameObject g = Instantiate(m_QuestSlotPrefab, m_Grid.transform);
                m_ListQuestSlot.Add(g.GetComponent<QuestSlot>());
            }
        }
        else if (m_Grid.childCount > _DataList.Count)
        {
            for (int i = _DataList.Count - 1; i < m_Grid.childCount; ++i)
            {
                m_Grid.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < _DataList.Count; ++i)
        {
            m_ListQuestSlot[i].Initialize(_DataList[i]);
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField]
    Transform m_Grid;
    List<ResultItemSlot> m_ListSlot = new List<ResultItemSlot>();

    int m_Score;
    Item[] m_RewardItems;

    Coroutine m_Coroutine;

    public void Initialize(int _Score, List<Item> _RewardItems)
    {
        m_Score = _Score;
        m_RewardItems = _RewardItems.ToArray();

        for (int i = 0; i < m_Grid.childCount; ++i)
        {
            m_ListSlot.Add(m_Grid.GetChild(i).GetComponent<ResultItemSlot>());
        }

        for (int i = 0; i < m_ListSlot.Count; ++i)
        {
            if (i < m_RewardItems.Length)
            {
                m_ListSlot[i].gameObject.SetActive(true);
                m_ListSlot[i].Initialize(m_RewardItems[i]);
            }
            else
            {
                m_ListSlot[i].gameObject.SetActive(false);
            }
        }
    }

    public void StarSlotsAnimation()
    {
        m_Coroutine = StartCoroutine(C_StartSlotAnimation());
    }

    IEnumerator C_StartSlotAnimation()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        int count = 0;
        while(count < m_RewardItems.Length)
        {
            m_ListSlot[count].StartAnimation();
            ++count;
            yield return wait;
        }
    }

    public void DirectResult()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
        }

        for (int i = 0; i < m_RewardItems.Length; ++i)
        {
            m_ListSlot[i].DirectResult();
        }
    }
}

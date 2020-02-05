using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField]
    Transform m_Grid;
    List<ResultItemSlot> m_ListSlot = new List<ResultItemSlot>();

    [SerializeField]
    TMPro.TMP_Text m_ScoreText;
    int m_Score;
    Item[] m_RewardItems;
    Coroutine m_Coroutine;

    [HideInInspector]
    public int m_Phase = 0;
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

    public void StartScoreAnimation()
    {
        if (m_Coroutine != null) StopCoroutine(m_Coroutine);
        m_Coroutine = StartCoroutine(C_StartScoreAnimation());
        m_Phase = 1;
    }

    IEnumerator C_StartScoreAnimation()
    {
        float progressvalue = 0.0f;
        while (true)
        {
            progressvalue += Time.deltaTime * m_Score * 0.5f;
            if (progressvalue >= m_Score)
            {
                m_ScoreText.text = m_Score.ToString();
                yield return new WaitForSeconds(1.0f);
                StarSlotsAnimation();
                yield break;
            }
            else
                m_ScoreText.text = ((int)progressvalue).ToString();
            yield return null;
        }
    }

    public void StarSlotsAnimation()
    {
        if (m_Coroutine != null) StopCoroutine(m_Coroutine);
        m_Coroutine = StartCoroutine(C_StartSlotAnimation());
        m_Phase = 2;
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

        m_Phase = 3;
    }

    public void DirectResult()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
        }

        m_ScoreText.text = m_Score.ToString();

        for (int i = 0; i < m_RewardItems.Length; ++i)
        {
            m_ListSlot[i].DirectResult();
        }

        m_Phase = 3;
    }
}

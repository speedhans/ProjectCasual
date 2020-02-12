using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoEffectCanvas : MonoBehaviour
{
    Transform m_List;
    Animator m_Animator;
    List<RectTransform> m_ListLines = new List<RectTransform>();

    public void Initialize()
    {
        m_List = transform.Find("List");
        m_Animator = m_List.GetComponent<Animator>();
        for (int i = 0; i < m_List.childCount; ++i)
        {
            m_ListLines.Add(m_List.GetChild(i).GetComponent<RectTransform>());
        }

        for (int i = 0; i < m_ListLines.Count; ++i)
        {
            Vector3 pos = m_ListLines[i].position;

            if (i % 2 == 0)
                pos.x = -1100.0f;
            else
                pos.x = 1100.0f;
            m_ListLines[i].position = pos;
        }
    }

    public void CurtainClose()
    {
        m_Animator.SetTrigger("Close");
    }

    public void CurtainOpen()
    {
        m_Animator.SetTrigger("Open");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : DefaultUI
{
    [SerializeField]
    Transform m_HpBar;
    [SerializeField]
    GameObject m_BuffViewer;
    List<SpriteRenderer> m_ListBuffIcon = new List<SpriteRenderer>();

    [SerializeField]
    float m_UpdateDelay = 0.5f;
    float m_UpdateTimer;

    Character m_Character;

    private void Awake()
    {
        for (int i = 0; i < m_BuffViewer.transform.childCount; ++i)
        {
            m_ListBuffIcon.Add(m_BuffViewer.transform.GetChild(i).GetComponent<SpriteRenderer>());
            m_ListBuffIcon[i].gameObject.SetActive(false);
        }
    }

    public void Initialize(Character _Self)
    {
        m_Character = _Self;
        transform.SetParent(m_Character.transform);
        transform.position = m_Character.transform.position + new Vector3(0.0f, 0.5f, -1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_Character) return;

        float RotX = VerticalFollowCamera.GetTransform().eulerAngles.x;

        Quaternion rot = Quaternion.Euler(RotX, 0.0f, 0.0f);
        Vector3 pos = m_Character.transform.position + new Vector3(0.0f, 0.5f, -1.0f);
        transform.SetPositionAndRotation(pos, rot);

        m_HpBar.localScale = new Vector3(Mathf.Clamp01(m_Character.m_Health / m_Character.m_MaxHealth), 1.0f, 1.0f);
        float deltatime = Time.deltaTime;
        m_UpdateTimer -= deltatime;
        if (m_UpdateTimer <= 0.0f)
        {
            m_UpdateTimer = m_UpdateDelay;
            BuffIconUpdate();
        }
    }

    void BuffIconUpdate()
    {
        List<Buff> list = m_Character.GetBuffList();

        for (int i = 0; i < m_BuffViewer.transform.childCount; ++i)
        {
            if (list.Count <= i)
            {
                m_ListBuffIcon[i].gameObject.SetActive(false);
                continue;
            }

            m_ListBuffIcon[i].sprite = list[i].m_BuffIcon;
            m_ListBuffIcon[i].gameObject.SetActive(true);
        }
    }
}

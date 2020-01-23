using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchEventSkillButton : CustomTouchEvent
{
    [SerializeField]
    int m_ButtonNumber;

    UnityEngine.UI.Image m_SkillIamge;
    UnityEngine.UI.Image m_CooldownVisualize;

    ActiveSkill m_TargetSkill;

    private void Awake()
    {
        m_SkillIamge = GetComponent<UnityEngine.UI.Image>();
        m_CooldownVisualize = transform.Find("CooldownVisualize").GetComponent<UnityEngine.UI.Image>();
        m_CooldownVisualize.fillAmount = 0.0f;
    }

    IEnumerator Start()
    {
        while(!m_TargetSkill)
        {
            PlayerCharacter c = GameManager.Instance.m_MyCharacter;
            if (c)
            {
                while (!c.m_IsPlayerCharacterInitializeComplete) yield return null;

                if (c.m_ListActiveSkill.Count < m_ButtonNumber + 1)
                {
                    gameObject.SetActive(false);
                }
                else if (c.m_ListActiveSkill[m_ButtonNumber] != null)
                {
                    m_TargetSkill = c.m_ListActiveSkill[m_ButtonNumber];
                    m_SkillIamge.sprite = m_TargetSkill.m_Image;
                    m_CooldownVisualize.sprite = m_TargetSkill.m_Image;
                }
            }
            yield return null;
        }
    }

    private void Update()
    {
        if (!m_TargetSkill) return;

        m_CooldownVisualize.fillAmount = (m_TargetSkill.m_CurrentCooldown / m_TargetSkill.m_MaxCooldown);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        m_TargetSkill.UseSkill();
    }
}

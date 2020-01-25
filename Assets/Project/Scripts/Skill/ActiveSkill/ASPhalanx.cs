using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASPhalanx : ActiveSkill
{
    public enum E_PHALANXTYPE
    {
        ADDTYPE, // Add
        MULTIPLETYPE, // Multi
    }

    [SerializeField]
    float m_Radius;
    [SerializeField]
    E_PHALANXTYPE m_Type;
    [SerializeField]
    float m_Duration;
    [SerializeField]
    [Range(0, 100)]
    float m_DefenceBonus;
    [SerializeField]
    E_DAMAGETYPE m_DefenceType;
    [SerializeField]
    GameObject m_Effect;
    [SerializeField]
    Character.E_ATTACHPOINT m_EffectAttachPoint;

    protected override void Awake()
    {
        base.Awake();

        AddUseAction(UseSkillEvent);
    }

    protected override void Update()
    {
        base.Update();
    }

    void UseSkillEvent()
    {
        StartCoroutine(C_SkillPlay(1.0f));
    }

    IEnumerator C_SkillPlay(float _Wait) // 지속시간, 방어력 증분
    {
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, _Wait, false, true);
        yield return new WaitForSeconds(_Wait);

        m_CurrentCooldown = m_MaxCooldown;

        List<Character> list = Character.FindTeamAllArea(m_Caster, m_Caster.m_Team, m_Caster.transform.position, m_Radius, false);

        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].AddBuff(m_Type == E_PHALANXTYPE.ADDTYPE ? E_BUFF.DEFENCEUP : E_BUFF.DEFENCEMULTI, new object[] { m_Duration, m_DefenceBonus, m_DefenceType, "Effect/" + m_Effect.name, m_EffectAttachPoint });
            }
        }
    }

    public override void AutoPlayLogic()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSHealthHigherDamage : PassiveSkill
{
    [Header("배율 1.0 증가분 50% ~ 0.01%")]
    [SerializeField]
    float m_DamageMultiply = 1.0f;
    float m_IncreaseDamage = 0.0f;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!m_Caster) return;

        int type = (int)m_Caster.m_AttackType;

        m_Caster.m_AddAttackDamage[type] -= m_IncreaseDamage;
        float damage = (m_Caster.m_AttackDamage + m_Caster.m_AddAttackDamage[type]);
        float hpper = (m_Caster.m_Health / m_Caster.m_MaxHealth) * 1.5f * m_DamageMultiply;

        m_IncreaseDamage = damage * hpper;
        m_Caster.m_AddAttackDamage[type] += m_IncreaseDamage;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        m_Caster.m_AddAttackDamage[(int)m_Caster.m_AttackType] -= m_IncreaseDamage;
    }
}

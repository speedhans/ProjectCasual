using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//    "&D& = Damage &DM& = DamageMultiply, &T& = Duration, &R& = Radius, &F& = Defence, &FM& = DefenceMultiply" +
//        "&AS& = AttackSpeed, &MS& = MovementSpeed, &DT& = DamageType"

public class ActiveSkill : Skill
{
    public enum E_SKILLTYPE
    {
        ATTACK,
        BUFF,
    }

    public E_SKILLTYPE m_SkillType = E_SKILLTYPE.ATTACK;
    public E_DAMAGETYPE m_DamageType;
    public float m_Damage;
    public float m_DamageMultiply = 0.0f;
    public float m_DefenceBonus;
    public float m_DefenceBonusMultiply;
    public float m_Radius = 0.0f;
    public float m_AttackSpeedMultiply;
    public float m_MovementSpeedMultiply;
    public float m_Duration;

    protected override void Awake()
    {
        base.Awake();
        m_ActuationType = E_ACTUATIONTYPE.ACTIVE;
    }

    public override void UseSkill()
    {
        base.UseSkill();
    }

    public override string GetManualText()
    {
        string manual = base.GetManualText();

        manual = manual.Replace("&DT&", m_DamageType.ToString());
        manual = manual.Replace("&D&", ((int)m_Damage).ToString());
        if (m_SkillType == E_SKILLTYPE.ATTACK)
            manual = manual.Replace("&DM&", m_DamageMultiply.ToString("N1"));
        else
            manual = manual.Replace("&DM&", ((int)(m_DamageMultiply * 100.0f - 100.0f)).ToString());
        manual = manual.Replace("&F&", ((int)m_DefenceBonus).ToString());
        manual = manual.Replace("&FM&", (m_DefenceBonusMultiply * 100.0f - 100.0f).ToString());
        manual = manual.Replace("&R&", m_Radius.ToString("N1"));
        manual = manual.Replace("&AS&", ((int)(m_AttackSpeedMultiply * 100.0f - 100.0f)).ToString());
        manual = manual.Replace("&MS&", ((int)(m_MovementSpeedMultiply * 100.0f - 100.0f)).ToString());
        manual = manual.Replace("&T&", ((int)m_Duration).ToString());

        return manual;
    }
}

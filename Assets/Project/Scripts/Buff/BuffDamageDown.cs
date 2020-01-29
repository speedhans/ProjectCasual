using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDamageDown : Buff
{
    float m_DecreaseDamage = 0.0f;
    float m_SubDamage;
    E_DAMAGETYPE m_Type;
    string m_EffectPath;
    Character.E_ATTACHPOINT m_EffectAttachPoint;

    GameObject m_Effect;

    public BuffDamageDown(Object _Self, string _BuffName, int _BuffID, Sprite _BuffIcon, float _LifeTime, float _AddDamage, E_DAMAGETYPE _Type, string _EffectPath, Character.E_ATTACHPOINT _Point) :
        base(_Self, _BuffName, _BuffID, _BuffIcon, _LifeTime)
    {
        m_SubDamage = _AddDamage;
        m_Type = _Type;
        m_EffectPath = _EffectPath;
        m_EffectAttachPoint = _Point;
        if (m_EffectPath != "")
        {
            m_Effect = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(m_EffectPath), (m_ParentObject as Character).GetAttachPoint(m_EffectAttachPoint));
            m_Effect.transform.localPosition = Vector3.zero;
            m_Effect.transform.localRotation = Quaternion.identity;
        }

        DamageDown();
    }

    public override void Destroy()
    {
        base.Destroy();

        Character c = m_ParentObject as Character;

        c.m_AddAttackDamage[(int)m_Type] -= m_DecreaseDamage;
        UnityEngine.Object.Destroy(m_Effect);
    }

    public override void DataUpdate(object[] _Value)
    {
        base.DataUpdate(_Value);

        Character c = m_ParentObject as Character;
        if (c)
        {
            c.m_AddAttackDamage[(int)m_Type] -= m_DecreaseDamage;
            m_DecreaseDamage = 0;
        }
        m_SubDamage = (float)_Value[1];
        m_Type = (E_DAMAGETYPE)_Value[2];
        m_EffectPath = (string)_Value[3];
        m_EffectAttachPoint = (Character.E_ATTACHPOINT)_Value[4];
        UnityEngine.Object.Destroy(m_Effect);

        if (m_EffectPath != "")
        {
            m_Effect = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(m_EffectPath), (m_ParentObject as Character).GetAttachPoint(m_EffectAttachPoint));
            m_Effect.transform.localPosition = Vector3.zero;
            m_Effect.transform.localRotation = Quaternion.identity;
        }

        DamageDown();
    }

    void DamageDown()
    {
        Character c = m_ParentObject as Character;
        if (c == null)
        {
            m_LifeTime = 0;
            return;
        }

        c.m_AddAttackDamage[(int)m_Type] -= m_DecreaseDamage;
        float fixeddmg = c.m_AddAttackDamage[(int)m_Type] - m_SubDamage;
        m_DecreaseDamage = fixeddmg - c.m_AddAttackDamage[(int)m_Type];
        c.m_AddAttackDamage[(int)m_Type] = fixeddmg;
    }
}

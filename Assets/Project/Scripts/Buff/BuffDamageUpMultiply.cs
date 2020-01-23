using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDamageUpMultiply : Buff
{
    float m_IncreaseDamage = 0.0f;
    float m_MultiplyDamage;
    E_DAMAGETYPE m_Type;
    string m_EffectPath;
    Character.E_ATTACHPOINT m_EffectAttachPoint;

    GameObject m_Effect;

    public BuffDamageUpMultiply(Object _Self, string _BuffName, int _BuffID, float _LifeTime, float _AddDamage, E_DAMAGETYPE _Type, string _EffectPath, Character.E_ATTACHPOINT _Point) :
        base(_Self, _BuffName, _BuffID, _LifeTime)
    {
        m_MultiplyDamage = _AddDamage;
        m_Type = _Type;
        m_EffectPath = _EffectPath;
        m_EffectAttachPoint = _Point;
        if (m_EffectPath != "")
        {
            m_Effect = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(m_EffectPath), (m_ParentObject as Character).GetAttachPoint(m_EffectAttachPoint));
            m_Effect.transform.localPosition = Vector3.zero;
            m_Effect.transform.localRotation = Quaternion.identity;
        }

        DamageUp();
    }

    protected override void Action(float _DeltaTime)
    {

    }

    public override void Destroy()
    {
        base.Destroy();

        Character c = m_ParentObject as Character;

        c.m_AddAttackDamage[(int)m_Type] -= m_IncreaseDamage;
        UnityEngine.Object.Destroy(m_Effect);
    }

    public override void DataUpdateEvent(object[] _Value)
    {
        m_MultiplyDamage = (float)_Value[1];
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

        DamageUp();
    }

    void DamageUp()
    {
        Character c = m_ParentObject as Character;
        if (c == null)
        {
            m_LifeTime = 0;
            return;
        }

        c.m_AttackDamage -= m_IncreaseDamage;

        float typedamage = c.m_AttackDamage + c.m_AddAttackDamage[(int)m_Type];

        float fixeddmg = typedamage * m_MultiplyDamage;

        m_IncreaseDamage = fixeddmg - typedamage;

        c.m_AttackDamage = fixeddmg;
    }
}

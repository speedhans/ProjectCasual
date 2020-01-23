using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHaste : Buff
{
    float m_IncreaseMoveSpeed = 0.0f;
    float m_IncreaseAttackSpeed = 0.0f;
    float m_MultiplySpeed;
    public BuffHaste(Object _Self, string _BuffName, int _BuffID, float _LifeTime, float _MultiplySpeed) :
    base(_Self, _BuffName, _BuffID, _LifeTime)
    {
        AddDataUpdateAction(DataUpdateEvent);
        m_MultiplySpeed = _MultiplySpeed;

        Haste();
    }

    protected override void Action(float _DeltaTime)
    {
        
    }

    public override void Destroy()
    {
        base.Destroy();

        Character c = m_ParentObject as Character;

        c.m_AttackSpeed -= m_IncreaseAttackSpeed;
        c.m_MovePerSpeed -= m_IncreaseMoveSpeed;
    }

    public override void DataUpdateEvent(object[] _Value)
    {
        m_MultiplySpeed = (float)_Value[1];
        Haste();
    }

    void Haste()
    {
        Character c = m_ParentObject as Character;
        if (c == null)
        {
            m_LifeTime = 0;
            return;
        }

        c.m_AttackSpeed -= m_IncreaseAttackSpeed;
        c.m_MovePerSpeed -= m_IncreaseMoveSpeed;

        float fixedatt = c.m_AttackSpeed * m_MultiplySpeed;
        float fixedmove = c.m_MovePerSpeed * m_MultiplySpeed;

        m_IncreaseAttackSpeed = fixedatt - c.m_AttackSpeed;
        m_IncreaseMoveSpeed = fixedmove - c.m_MovePerSpeed;

        c.m_AttackSpeed = fixedatt;
        c.m_MovePerSpeed = fixedmove;
    }
}

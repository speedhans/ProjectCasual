using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffHaste : Buff
{
    float m_IncreaseMoveSpeed = 0.0f;
    float m_IncreaseAttackSpeed = 0.0f;
    float m_MultiplyAttackSpeed;
    float m_MultiplyMoveSpeed;
    public BuffHaste(Object _Self, string _BuffName, int _BuffID, Sprite _BuffIcon, float _LifeTime, float _MultiplyAttackSpeed, float _MultiplyMoveSpeed) :
    base(_Self, _BuffName, _BuffID, _BuffIcon, _LifeTime)
    {
        m_MultiplyAttackSpeed = _MultiplyAttackSpeed;
        m_MultiplyMoveSpeed = _MultiplyMoveSpeed;
        Haste();
    }


    public override void Destroy()
    {
        base.Destroy();

        Character c = m_ParentObject as Character;

        c.m_AttackSpeed -= m_IncreaseAttackSpeed;
        c.m_MovePerSpeed -= m_IncreaseMoveSpeed;
    }

    public override void DataUpdate(object[] _Value)
    {
        base.DataUpdate(_Value);

        m_MultiplyAttackSpeed = (float)_Value[1];
        m_MultiplyMoveSpeed = (float)_Value[2];
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

        float fixedatt = c.m_AttackSpeed * m_MultiplyAttackSpeed;
        float fixedmove = c.m_MovePerSpeed * m_MultiplyMoveSpeed;

        m_IncreaseAttackSpeed = fixedatt - c.m_AttackSpeed;
        m_IncreaseMoveSpeed = fixedmove - c.m_MovePerSpeed;

        c.m_AttackSpeed = fixedatt;
        c.m_MovePerSpeed = fixedmove;
    }
}

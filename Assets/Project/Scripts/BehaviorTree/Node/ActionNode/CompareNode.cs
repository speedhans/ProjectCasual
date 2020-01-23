using System;
using System.Collections.Generic;
using UnityEngine;

public class CompareNode : ActionNode
{
    public enum E_COMPARE
    {
        LESS,
        LEQUAL,
        EQUAL,
        GEQUAL,
        GREATE,
    }

    public enum E_TYPE
    {
        SelfHealth,
        TargetHealth,
    }

    [Serializable]
    public struct S_COMPARE
    {
        public E_TYPE Type;
        public E_COMPARE Compare;
        public float Value;
    }

    public S_COMPARE[] m_Compare;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        for (int i = 0; i < m_Compare.Length; ++i)
        {
            if (!CompareCheck(_Self, i)) return false;
        }

        return true;       
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }

    bool CompareCheck(Character _Self, int _TableNumber)
    {
        float src = 0.0f;
        switch(m_Compare[_TableNumber].Type)
        {
            case E_TYPE.SelfHealth:
                src = _Self.m_Health;
                break;
            case E_TYPE.TargetHealth:
                src = m_BehaviorTree.m_TargetObject.m_Health;
                break;
        }
        
        switch(m_Compare[_TableNumber].Compare)
        {
            case E_COMPARE.LESS:
                {
                    if (src < m_Compare[_TableNumber].Value) return true;
                }
                break;
            case E_COMPARE.LEQUAL:
                {
                    if (src <= m_Compare[_TableNumber].Value) return true;
                }
                break;
            case E_COMPARE.EQUAL:
                {
                    if (src == m_Compare[_TableNumber].Value) return true;
                }
                break;
            case E_COMPARE.GEQUAL:
                {
                    if (src >= m_Compare[_TableNumber].Value) return true;
                }
                break;
            case E_COMPARE.GREATE:
                {
                    if (src > m_Compare[_TableNumber].Value) return true;
                }
                break;
        }

        return false;
    }
}

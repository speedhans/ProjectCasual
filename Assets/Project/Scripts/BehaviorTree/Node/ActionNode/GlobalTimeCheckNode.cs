using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimeCheckNode : ActionNode
{
    public enum E_COMPARE
    {
        LESS,
        LEQUAL,
        EQUAL,
        GEQUAL,
        GREATE,
    }

    [Serializable]
    public struct S_COMPARE
    {
        public int day;
        public int hour;
        public int minute;
        public E_COMPARE Compare;
    }

    public S_COMPARE[] m_Compare;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        for (int i = 0; i < m_Compare.Length; ++i)
        {
            if (!TimeCheck(i)) return false;
        }
        
        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }

    bool TimeCheck(int _TableNumber)
    {
        switch(m_Compare[_TableNumber].Compare)
        {
            case E_COMPARE.LESS:
                {
                    if (m_Compare[_TableNumber].day < GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.DAY))
                        return true;
                    else if (m_Compare[_TableNumber].hour < GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.HOUR))
                        return true;
                    else if (m_Compare[_TableNumber].minute < GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.MINUTE))
                        return true;
                }
                break;
            case E_COMPARE.LEQUAL:
                {
                    if (m_Compare[_TableNumber].day <= GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.DAY))
                        return true;
                    else if (m_Compare[_TableNumber].hour <= GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.HOUR))
                        return true;
                    else if (m_Compare[_TableNumber].minute <= GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.MINUTE))
                        return true;
                }
                break;
            case E_COMPARE.EQUAL:
                {
                    if ((m_Compare[_TableNumber].day == GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.DAY)) &&
                        (m_Compare[_TableNumber].hour == GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.HOUR)) &&
                        (m_Compare[_TableNumber].minute == GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.MINUTE)))
                        return true;
                }
                break;
            case E_COMPARE.GEQUAL:
                {
                    if (m_Compare[_TableNumber].day >= GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.DAY))
                        return true;
                    else if (m_Compare[_TableNumber].hour >= GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.HOUR))
                        return true;
                    else if (m_Compare[_TableNumber].minute >= GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.MINUTE))
                        return true;
                }
                break;
            case E_COMPARE.GREATE:
                {
                    if (m_Compare[_TableNumber].day > GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.DAY))
                        return true;
                    else if (m_Compare[_TableNumber].hour > GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.HOUR))
                        return true;
                    else if (m_Compare[_TableNumber].minute > GameManager.Instance.GetWorldTime(GameManager.E_TIMETYPE.MINUTE))
                        return true;
                }
                break;
        }

        return false;
    }
}

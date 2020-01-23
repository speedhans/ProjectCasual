using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitForTimeNode : ActionNode
{
    [SerializeField]
    bool m_GlobalTick = true;

    public float m_WaitTime;
    float m_Timer;

    float m_BeforeTime;

    private void OnEnable()
    {
        m_Timer = m_WaitTime;
    }

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        if (!m_GlobalTick)
        {
            float time = Time.time;
            m_Timer -= (time - m_BeforeTime);
            m_BeforeTime = time;
        }

        if (m_Timer <= 0.0f)
        {
            m_Timer = m_WaitTime;
            return true;
        }
        else
            return false;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
        if (m_GlobalTick)
        {
            float time = Time.time;
            m_Timer -= (time - m_BeforeTime);
            m_BeforeTime = time;
        }
    }

    public override void Initialize(Character _Self, BehaviorTree _SelfTree)
    {
        base.Initialize(_Self, _SelfTree);

        m_BeforeTime = Time.time;
    }
}

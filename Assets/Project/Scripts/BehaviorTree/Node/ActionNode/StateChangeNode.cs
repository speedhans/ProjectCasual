using System;
using System.Collections.Generic;
using UnityEngine;

public class StateChangeNode : ActionNode
{
    [SerializeField]
    Character.E_STATE m_ChangeState;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        _Self.m_State = m_ChangeState;
        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }
}

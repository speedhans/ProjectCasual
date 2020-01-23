using System;
using System.Collections.Generic;
using UnityEngine;

public class StateCheckNode : ActionNode
{
    public Character.E_STATE m_State;
    public bool m_Equal = true;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        if ((m_State == _Self.m_State) == m_Equal) return true;

        return false;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }
}

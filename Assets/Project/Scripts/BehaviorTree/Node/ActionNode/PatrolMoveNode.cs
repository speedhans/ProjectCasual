using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMoveNode : ActionNode
{
    [SerializeField]
    List<Vector3> m_ListLocation = new List<Vector3>();
    [SerializeField]
    float m_ArrivalDetectionRange;
    int m_Number;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        if (m_ListLocation.Count < 1) return false;

        if ((_Self.transform.position - m_ListLocation[m_Number]).magnitude < m_ArrivalDetectionRange)
        {
            m_Number++;
            if (m_ListLocation.Count - 1 < m_Number)
                m_Number = 0;
        }

        _Self.MoveToLocation(m_ListLocation[m_Number]);

        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }
}

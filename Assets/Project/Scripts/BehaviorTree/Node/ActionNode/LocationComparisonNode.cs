using System;
using System.Collections.Generic;
using UnityEngine;

public class LocationComparisonNode : ActionNode
{
    public enum E_TYPE
    {
        NavMeshTarget,
        ObjectTarget
    }

    [SerializeField]
    public E_TYPE m_TargetType;

    [SerializeField]
    
    [Range(0.0f, 99999.0f)]
    public float m_Distance;
    [SerializeField]
    bool m_Greater = false;
    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        Vector3 target = Vector3.zero;

        switch(m_TargetType)
        {
            case E_TYPE.NavMeshTarget:
                target = _Self.GetLastTargetLocation();
                break;
            case E_TYPE.ObjectTarget:
                {
                    if (!m_BehaviorTree.m_TargetObject) return false;
                    target = m_BehaviorTree.m_TargetObject.transform.position;
                }
                break;
        }

        if ((Vector3.Distance(_Self.transform.position, target) > m_Distance) == m_Greater)
            return true;

        return false;
    }

    public override void Initialize(Character _Self, BehaviorTree _SelfTree)
    {
        base.Initialize(_Self, _SelfTree);
        // 초기정보 받아오기

        
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }
}

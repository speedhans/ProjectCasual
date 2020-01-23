using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveToLocationNode : ActionNode
{
    public enum E_TYPE
    {
        StaticLocation,
        TargetTransform
    }

    [SerializeField]
    public E_TYPE m_LocationAcquisitionMethod;
    [SerializeField]
    public Vector3 m_Location;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        switch(m_LocationAcquisitionMethod)
        {
            case E_TYPE.StaticLocation:
                {
                    if (m_Location != _Self.GetLastTargetLocation() && (_Self.transform.position - m_Location).magnitude > 0.01f)
                        _Self.MoveToLocation(new Vector3(m_Location.x, m_Location.y, m_Location.z));
                }
                break;
            case E_TYPE.TargetTransform:
                {
                    if ((_Self.transform.position - m_BehaviorTree.m_MoveTargetTransform.position).magnitude > 0.01f)
                        _Self.MoveToLocation(m_BehaviorTree.m_MoveTargetTransform.position);
                }
                break;
        }

        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
        
    }
}

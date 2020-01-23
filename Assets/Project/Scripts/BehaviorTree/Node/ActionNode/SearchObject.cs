using System;
using System.Collections.Generic;
using UnityEngine;

public class SearchObject : ActionNode
{
    [SerializeField]
    LayerMask m_TargetLayerMask;
    [SerializeField]
    float m_SearchAreaRadius;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        float distacne = 9999999.0f;
        Collider[] c = Physics.OverlapSphere(_Self.transform.position, m_SearchAreaRadius, 1 << m_TargetLayerMask);

        if (c.Length < 1)
        {
            m_BehaviorTree.m_TargetObject = null;
            return false;
        }

        for (int i = 0; i < c.Length; ++i)
        {
            float sqrdistance = (_Self.transform.position - c[i].transform.position).sqrMagnitude;
            if (sqrdistance < distacne)
            {
                Object o = c[i].transform.GetComponent<Object>();
                if (o)
                {
                    if (o != _Self)
                    {
                        m_BehaviorTree.m_TargetObject = o;
                        distacne = sqrdistance;
                    }
                }
            }
        }

        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }
}

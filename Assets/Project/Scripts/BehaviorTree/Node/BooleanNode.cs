using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanNode : BranchNode
{
    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        if (m_ChildNode[0] == null) return false;
        if (m_ChildNode[0].BehaviorUpdate(_Self, _DeltaTime))
        {
            if (m_ChildNode.Count < 2 || m_ChildNode[1] == null) return false;
            m_ChildNode[1].BehaviorUpdate(_Self, _DeltaTime);
        }
        else
        {
            if (m_ChildNode.Count < 3 || m_ChildNode[2] == null) return false;
            m_ChildNode[2].BehaviorUpdate(_Self, _DeltaTime);
        }

        return true;
    }
}
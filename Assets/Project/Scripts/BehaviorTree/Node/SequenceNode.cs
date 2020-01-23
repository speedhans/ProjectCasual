using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전부 True 일때만 True
/// </summary>
public class SequenceNode : BranchNode
{
    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        for (int i = 0; i < m_ChildNode.Count; ++i)
        {
            if (m_ChildNode[i] != null)
            {
                if (!m_ChildNode[i].BehaviorUpdate(_Self, _DeltaTime))
                    return false;
            }
        }

        return true;
    }
}

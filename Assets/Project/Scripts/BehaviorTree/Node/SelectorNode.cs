using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하나라도 True 면 True
/// </summary>
public class SelectorNode : BranchNode
{
    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        bool b = false;
        for (int i = 0; i < m_ChildNode.Count; ++i)
        {
            if (m_ChildNode[i] != null)
            {
                if (m_ChildNode[i].BehaviorUpdate(_Self, _DeltaTime))
                {
                    b = true;
                }
            }
        }

        return b;
    }
}

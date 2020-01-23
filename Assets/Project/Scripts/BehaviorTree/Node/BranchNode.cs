using System.Collections.Generic;
using UnityEngine;

public class BranchNode : BehaviorTreeNode
{
    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        return false;
    }

    public void AddChildNode(BranchNode _Node)
    {
        for (int i = 0; i < m_ChildNode.Count; ++i)
        {
            if (m_ChildNode[i] == null) m_ChildNode.RemoveAt(i);
        }

        m_ChildNode.Add(_Node);
    }

    public void AddActionNode(ActionNode _Node)
    {
        for (int i = 0; i < m_ChildNode.Count; ++i)
        {
            if (m_ChildNode[i] == null) m_ChildNode.RemoveAt(i);
        }

        m_ChildNode.Add(_Node);
    }
}

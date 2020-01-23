using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "BehaviorTreeNode", menuName = "BehaviorTree/RootNode", order = int.MaxValue)]
public class RootNode : BranchNode
{
    [HideInInspector]
    public List<ActionNode> m_ListActionNode = new List<ActionNode>();

    public void Initialzie(Character _Self, BehaviorTree _SelfTree)
    {
        for (int i = 0; i < m_ListActionNode.Count; ++i)
        {
            if (m_ListActionNode[i] == null) 
                m_ListActionNode.RemoveAt(i);
            else
                m_ListActionNode[i].Initialize(_Self, _SelfTree);
        }
    }

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        foreach (ActionNode a in m_ListActionNode)
        {
            a.TickUpdate(_Self, _DeltaTime);
        }

        for (int i = 0; i < m_ChildNode.Count; ++i)
        {
            if (m_ChildNode[i] != null)
                m_ChildNode[i].BehaviorUpdate(_Self, _DeltaTime);
        }

        return true;
    }
}

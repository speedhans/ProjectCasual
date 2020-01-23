using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode : BehaviorTreeNode
{
    protected BehaviorTree m_BehaviorTree;

    public virtual void Initialize(Character _Self, BehaviorTree _SelfTree) { m_BehaviorTree = _SelfTree; }
    public abstract void TickUpdate(Character _Self, float _DeltaTime);
}

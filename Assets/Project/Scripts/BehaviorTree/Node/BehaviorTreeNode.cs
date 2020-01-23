using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorTreeNode : ScriptableObject, NodeInterface
{
    [HideInInspector]
    public List<BehaviorTreeNode> m_ChildNode = new List<BehaviorTreeNode>();
    [SerializeField]
    public RootNode m_RootNode;
    public abstract bool BehaviorUpdate(Character _Self, float _DeltaTime);
}

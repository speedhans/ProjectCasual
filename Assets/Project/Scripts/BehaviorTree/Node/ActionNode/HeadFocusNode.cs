using System;
using System.Collections.Generic;
using UnityEngine;

public class HeadFocusNode : ActionNode
{
    [SerializeField]
    bool m_TargetIsHead;
    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        if (m_TargetIsHead)
        {
            HumanoidCharacter humanoid = m_BehaviorTree.m_TargetObject as HumanoidCharacter;
            if (humanoid)
            {
                _Self.m_Animator.SetLookAtWeight(1.0f);
                _Self.m_Animator.SetLookAtPosition(humanoid.m_HeadAxis.position);
            }
            else
                return false;
        }
        else
        {
            _Self.m_Animator.SetLookAtWeight(1.0f);
            _Self.m_Animator.SetLookAtPosition(m_BehaviorTree.m_TargetObject.transform.position);
        }

        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
        if (m_BehaviorTree.m_TargetObject == null)
            _Self.m_Animator.SetLookAtWeight(0.0f);
    }
}

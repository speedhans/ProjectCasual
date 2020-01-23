using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayNode : ActionNode
{
    public string m_AnimationKey;
    public float m_FadeDuration;

    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        _Self.m_Animator.CrossFade(m_AnimationKey, m_FadeDuration);
        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }
}

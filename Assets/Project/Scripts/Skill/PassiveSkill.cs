using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkill : Skill
{
    protected override void Awake()
    {
        base.Awake();
        m_SkillType = E_TYPE.PASSIVE;
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void OnDestroy()
    {
        
    }
}

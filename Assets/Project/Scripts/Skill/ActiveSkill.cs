using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : Skill
{
    protected override void Awake()
    {
        base.Awake();
        m_SkillType = E_TYPE.ACTIVE;
    }

    public override void UseSkill()
    {
        base.UseSkill();
    }
}

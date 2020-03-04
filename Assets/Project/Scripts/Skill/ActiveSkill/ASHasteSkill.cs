using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ASHasteSkill : ActiveSkill
{
    protected override void Awake()
    {
        base.Awake();

        AddUseAction(UseSkillEvent);
    }

    protected override void Update()
    {
        base.Update();
    }

    void UseSkillEvent()
    {
        StartCoroutine(C_SkillPlay(1.0f));
    }

    IEnumerator C_SkillPlay(float _Wait)
    {
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, _Wait, false);
        yield return new WaitForSeconds(_Wait);

        m_CurrentCooldown = m_MaxCooldown;

        m_Caster.AddBuff(E_BUFF.HASTE, new object[] { m_Duration, m_AttackSpeedMultiply, m_MovementSpeedMultiply });
    }

    public override bool AutoPlayLogic()
    {
        return false;
    }
}

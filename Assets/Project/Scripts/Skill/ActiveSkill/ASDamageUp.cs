using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASDamageUp : ActiveSkill
{
    [Header("속성 데미지 증가")]
    [SerializeField]
    GameObject m_Effect;
    [SerializeField]
    Character.E_ATTACHPOINT m_EffectAttachPoint;

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
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, _Wait, false, true);
        yield return new WaitForSeconds(_Wait);

        m_CurrentCooldown = m_MaxCooldown;

        m_Caster.AddBuff(E_BUFF.DAMAGEUP, new object[] { m_Duration, m_Damage, m_DamageType, "Effect/" + m_Effect.name, m_EffectAttachPoint });
    }

    public override void AutoPlayLogic()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASDamageUp : ActiveSkill
{
    [SerializeField]
    float m_Duration;
    [Header("속성 데미지 증가")]
    [SerializeField]
    float m_AddDamage;
    [SerializeField]
    E_DAMAGETYPE m_DamageType;
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

    IEnumerator C_SkillPlay(float _Wait) // 지속시간, 공격력 증분
    {
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, _Wait, false, true);
        yield return new WaitForSeconds(_Wait);

        m_CurrentCooldown = m_MaxCooldown;

        m_Caster.AddBuff(1, new object[] { m_Duration, m_AddDamage, m_DamageType, "Effect/" + m_Effect.name, m_EffectAttachPoint });
    }

    public override void AutoPlayLogic()
    {

    }
}

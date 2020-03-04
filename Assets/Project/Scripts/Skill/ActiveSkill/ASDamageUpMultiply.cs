using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASDamageUpMultiply : ActiveSkill
{
    [Tooltip("계산식: (기본 공격력 + 해당 속성 공격력) * 스킬변수")]
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
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, _Wait, false);
        yield return new WaitForSeconds(_Wait);

        m_CurrentCooldown = m_MaxCooldown;
        if (!m_PhotonView.IsMine) yield break;
        m_Caster.AddBuff(E_BUFF.DAMAGEUPMULTI, new object[] { m_Duration, m_DamageMultiply, m_DamageType, "Effect/" + m_Effect.name, m_EffectAttachPoint });
    }

    public override bool AutoPlayLogic()
    {
        return false;
    }
}

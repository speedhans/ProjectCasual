using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASPhalanx : ActiveSkill
{
    public enum E_PHALANXTYPE
    {
        ADDTYPE, // Add
        MULTIPLETYPE, // Multi
    }

    [SerializeField]
    E_PHALANXTYPE m_Type;
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

        List<Character> list = Character.FindTeamAllArea(m_Caster, m_Caster.m_Team, m_Caster.transform.position, m_Radius, false);

        if (list != null)
        {
            E_BUFF buff = m_Type == E_PHALANXTYPE.ADDTYPE ? E_BUFF.DEFENCEUP : E_BUFF.DEFENCEMULTI;
            object[] datas = null;
            if (m_Type == E_PHALANXTYPE.ADDTYPE)
                datas = new object[] { m_Duration, m_DefenceBonus, m_DamageType, "Effect/" + m_Effect.name, m_EffectAttachPoint };
            else
                datas = new object[] { m_Duration, m_DefenceBonusMultiply, m_DamageType, "Effect/" + m_Effect.name, m_EffectAttachPoint };

            for (int i = 0; i < list.Count; ++i)
            {
                list[i].AddBuff(buff, datas);
            }
        }
    }

    public override bool AutoPlayLogic()
    {
        return false;
    }
}

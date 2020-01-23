using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ASHasteSkill : ActiveSkill
{
    [SerializeField]
    float m_Duration;
    [SerializeField]
    float m_MultiplySpeed;

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

    IEnumerator C_SkillPlay(float _Wait) // 지속시간, 공,이속증분
    {
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, _Wait, false, true);
        yield return new WaitForSeconds(_Wait);

        m_CurrentCooldown = m_MaxCooldown;

        m_Caster.AddBuff(0, new object[] { m_Duration, m_MultiplySpeed });
    }

    public override void AutoPlayLogic()
    {

    }
}

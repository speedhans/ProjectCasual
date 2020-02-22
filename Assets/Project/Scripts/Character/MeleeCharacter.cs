using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MeleeCharacter : Character
{
    protected override void Awake()
    {
        base.Awake();
        m_CharacterType = E_CHARACTERTYPE.NORMALNPC;

        SetAutoPlayLogic(AutoPlayLogic);
    }

    protected override void AutoPlayLogic()
    {
        if (m_AttackTarget != null)
        {
            if (m_AttackTarget.m_Character.m_Live == E_LIVE.DEAD)
            {
                m_AttackTarget.m_Hate = 0.0f;
                m_AttackTarget = null;
                return;
            }

            float distance = (m_AttackTarget.m_Character.transform.position - transform.position).magnitude;
            if (m_AttackRange >= distance) // 공격
            {
                if (m_NavMeshController.IsUpdate())
                    m_NavMeshController.ClearPath();

                transform.forward = m_AttackTarget.m_Character.transform.position - transform.position;

                SetStateAndAnimation(E_ANIMATION.ATTACK, 0.25f, 1.0f, 0.0f);
            }
            else // 추격
            {
                if (m_State != E_STATE.MOVE)
                    SetStateAndAnimation(E_ANIMATION.RUN, 0.25f, 1.0f, 0.0f);
                MoveToLocation(m_AttackTarget.m_Character.transform.position);
            }
        }
        else // 대기
        {
            if (m_State != E_STATE.IDLE && !m_NavMeshController.IsUpdate())
            {
                SetStateAndAnimation(E_ANIMATION.IDLE, 0.25f, 1.0f, 0.0f);
            }

            Character c = FindEnemyArea(this, this.transform.position, m_AutoSearchRadius);
            if (c)
            {
                HateTarget t = new HateTarget(c, 100.0f);
                m_DicHateTarget.Add(t.m_Character.gameObject.GetInstanceID(), t);
                m_AttackTarget = t;
            }
        }
    }

    public override void AttackMomentEvent()
    {
        if (m_Live == E_LIVE.DEAD) return;
        base.AttackMomentEvent();
        if (m_AttackTarget == null) return;

        m_AttackTarget.m_Character.GiveToDamage(GetDamages(), this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASFiveSwingAttack : ActiveSkill
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
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL2, 0.25f, 1.0f, 3.0f);
        StartCoroutine(C_Progress());
    }

    IEnumerator C_Progress()
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        yield return wait;
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL3, 0.25f, 1.0f, 1.0f);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL4, 0.25f, 1.0f, 1.0f);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL3, 0.25f, 1.0f, 1.0f);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL4, 0.25f, 1.0f, 1.0f);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL3, 0.25f, 1.0f, 1.0f);
        Damage();
    }

    void Damage()
    {
        List<Character> list = Character.FindEnemyAllShape(m_Caster, m_Caster.transform.position, m_Caster.transform.forward, m_Radius, 30.0f);

        if (list != null)
        {
            float[] damage = new float[] { CalculateSkillDamage(m_Caster, m_DamageType, m_DamageMultiply) };
            int[] type = new int[] { (int)m_DamageType };
            foreach (Character c in list)
            {
                c.GiveToDamage(damage, type, m_Caster);
            }
        }
    }

    public override void AutoPlayLogic()
    {
        Character.HateTarget target = m_Caster.AttackTarget;
        if (target == null) return;

        Vector3 dir = target.m_Character.transform.position - m_Caster.transform.position;
        float distance = dir.magnitude;
        if (distance < m_Radius * 0.9f)
        {
            UseSkill();
        }
    }
}

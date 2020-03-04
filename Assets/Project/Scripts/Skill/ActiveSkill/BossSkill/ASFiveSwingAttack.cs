using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASFiveSwingAttack : ActiveSkill
{
    [SerializeField]
    AudioClip m_SwingSound1;
    [SerializeField]
    AudioClip m_SwingSound2;
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
        StartCoroutine(C_Progress());
    }

    IEnumerator C_Progress()
    {
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL2, 0.25f, 1.0f, 3.0f);
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        yield return wait;
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL3, 0.25f, 1.0f, 1.0f);
        SoundManager.Instance.PlayEffectSound(m_SwingSound1);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL4, 0.25f, 1.0f, 1.0f);
        SoundManager.Instance.PlayEffectSound(m_SwingSound2);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL3, 0.25f, 1.0f, 1.0f);
        SoundManager.Instance.PlayEffectSound(m_SwingSound1);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL4, 0.25f, 1.0f, 1.0f);
        SoundManager.Instance.PlayEffectSound(m_SwingSound2);
        Damage();
        yield return wait;
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL3, 0.25f, 1.0f, 1.0f);
        SoundManager.Instance.PlayEffectSound(m_SwingSound1);
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

    public override bool AutoPlayLogic()
    {
        Character.HateTarget target = m_Caster.AttackTarget;
        if (target == null) return false;
        if (target.m_Character.m_Live == Object.E_LIVE.DEAD) return false;

        Vector3 dir = target.m_Character.transform.position - m_Caster.transform.position;
        float distance = dir.magnitude;
        if (distance < m_Radius * 0.9f)
        {
            return UseSkill();
        }
        return false;
    }
}

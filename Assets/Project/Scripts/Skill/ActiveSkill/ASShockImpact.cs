using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ASShockImpact : ActiveSkill
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
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL2, 0.25f, 2.5f, 0.5f, false, true);

        List<Character> list = Character.FindEnemyAllArea(m_Caster, m_Caster.transform.position, m_Radius);
        if (list != null)
        {
            
            float[] damage = new float[] { CalculrateSkillDamage(m_Caster, m_DamageType, m_DamageMultiply) };
            int[] type = new int[] { (int)m_DamageType };
            foreach (Character c in list)
            {
                c.GiveToDamage(damage, type, m_Caster);
            }
        }
    }

    public override void AutoPlayLogic()
    {

    }
}
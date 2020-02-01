using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ASChargeBlast : ActiveSkill
{
    [SerializeField]
    GameObject m_ChargeRibbonEffect;
    [SerializeField]
    GameObject m_ChargeBallEffect;
    [SerializeField]
    Vector3 m_ChargeEffectLocalLocation;
    [SerializeField]
    GameObject m_BlastEffect;

    [SerializeField]
    float m_ChargeTime;

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
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, 0.0f, true);

        StartCoroutine(C_Progress());
    }

    IEnumerator C_Progress()
    {
        float timer = m_ChargeTime;
        
        Vector3 effectpos = m_Caster.transform.position + (m_Caster.transform.rotation * m_ChargeEffectLocalLocation);
        LifeTimerWithObjectPool ribbon = ObjectPool.GetObject<LifeTimerWithObjectPool>(m_ChargeRibbonEffect.name);
        if (ribbon)
        {
            ribbon.Initialize();
            ribbon.transform.position = effectpos;
            ribbon.gameObject.SetActive(true);
        }
        LifeTimerWithObjectPool ball = ObjectPool.GetObject<LifeTimerWithObjectPool>(m_ChargeBallEffect.name);
        if (ball)
        {
            ball.Initialize();
            ball.transform.localScale = Vector3.one;
            ball.transform.position = effectpos;
            ball.gameObject.SetActive(true);
        }

        while (timer > 0.0f)
        {
            if (!m_Caster.IsCharging())
            {
                ObjectPool.PushObject(ribbon.gameObject);
                ObjectPool.PushObject(ball.gameObject);
                yield break;
            }
            float deltatime = Time.deltaTime;
            timer -= deltatime;

            ball.transform.localScale += Vector3.one * deltatime;
            yield return null;
        }

        ObjectPool.PushObject(ribbon.gameObject);
        ObjectPool.PushObject(ball.gameObject);
        m_Caster.SetCharging(false);

        LifeTimerWithObjectPool blast = ObjectPool.GetObject<LifeTimerWithObjectPool>(m_BlastEffect.name);
        if (blast)
        {
            blast.Initialize();
            blast.transform.position = m_Caster.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
            blast.gameObject.SetActive(true);
        }

        List<Character> list = Character.FindEnemyAllArea(m_Caster, m_Caster.transform.position, m_Radius);
        if (list != null)
        {
            float[] damage = new float[] { CalculrateSkillDamage(m_Caster, m_DamageType, m_DamageMultiply) };
            int[] type = new int[] { (int)m_DamageType };

            for (int i = 0; i < list.Count; ++i)
            {
                list[i].GiveToDamage(damage, type, m_Caster);
            }
        }
    }
    
    public override void AutoPlayLogic()
    {
        if (m_CurrentCooldown > 0.0f) return;
        
        if (Character.ScopeEnemyCheck(m_Caster, m_Caster.transform.position, m_Radius * 0.75f) > 0)
        {
            UseSkill();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : SkyEnvironment
{
    [SerializeField]
    float m_EffectDelay;
    float m_EffectDelayTimer;

    [SerializeField]
    float m_IceDamage;
    int[] m_DamageType = new int[1] { (int)E_DAMAGETYPE.ICE };

    protected override void Awake()
    {
        GameManager.Instance.AddSkyEnvironmentEvent(EnvironmentEffect);

        m_EffectDelayTimer = m_EffectDelay;
    }

    protected override void Update()
    {
        base.Update();

        if (m_EffectDelayTimer > 0.0f)
        {
            m_EffectDelayTimer -= Time.deltaTime;
            if (m_EffectDelayTimer <= 0.0f)
            {
                m_EffectDelayTimer = m_EffectDelay;
            }
        }
    }

    public override void EnvironmentEffect(Object _Self)
    {
        if (m_EffectDelayTimer == m_EffectDelay)
        {
            float[] damage = new float[1] { m_IceDamage };

            _Self.GiveToDamage(damage, m_DamageType, null);
        }
    }

    protected override void OnDestroy()
    {
        GameManager.Instance.SubSkyEnvironmentEvent(EnvironmentEffect);
    }
}

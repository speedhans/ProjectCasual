using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkeletonKing : MeleeBossCharacter
{
    [SerializeField]
    GameObject m_AttackImpactEffect;
    protected override void Awake()
    {
        base.Awake();

        StartCoroutine(C_Initialize());

        SetAutoPlayLogic(AutoPlayLogic);
        AddDamageEvent(DamageEvent_S);
    }

    IEnumerator C_Initialize()
    {
        while(true)
        {
            if (GameManager.Instance.m_Main)
            {
                if (GameManager.Instance.m_Main.IsLoadingComplete)
                {
                    BossCharacterUI.Instance.InsertUI(this);
                    yield break;
                }
            }
            yield return null;
        }
    }

    protected override void AutoPlayLogic()
    {
        base.AutoPlayLogic();


    }

    protected void DamageEvent_S(float[] _Damage, float[] _CalculateDamage, int[] _DamageType, float _CalculateFullDamage, bool _Critical, Character _Attacker)
    {
        
    }

    public override void AttackMomentEvent()
    {
        if (m_Live == E_LIVE.DEAD) return;
        base.AttackMomentEvent();

        Vector3 location = transform.position + (transform.forward * m_AttackRange) + new Vector3(0.0f, 0.01f, 0.0f);
        LifeTimerWithObjectPool life = ObjectPool.GetObject<LifeTimerWithObjectPool>(m_AttackImpactEffect.name);
        if (life)
        {
            life.Initialize();
            life.transform.position = location;
            life.gameObject.SetActive(true);
        }
        List<Character> list = FindEnemyAllArea(this, location, 2.0f);
        if (list != null)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].GiveToDamage(GetDamages(), this);
            }
        }
    }
}

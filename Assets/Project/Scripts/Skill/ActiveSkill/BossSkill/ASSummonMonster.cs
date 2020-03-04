using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class ASSummonMonster : ActiveSkill
{
    [SerializeField]
    string m_SummonMonsterPath;
    [SerializeField]
    GameObject m_SummonMonsterPrefab;
    [SerializeField]
    Vector3[] m_SpawnLocation;
    [SerializeField]
    GameObject m_SummonEffectPrefab;
    [SerializeField]
    float[] m_HPTrigger;
    bool[] m_Trigger;
    int m_TriggerNumber;

    protected override void Awake()
    {
        base.Awake();
        m_Trigger = new bool[m_HPTrigger.Length];
        NetworkManager.Instance.m_MasterClientSwitchedCallback += MasterClientSwitched;
        AddUseAction(UseSkillEvent);
    }
    void UseSkillEvent()
    {
        m_Caster.SetStateAndAnimationLocal(E_ANIMATION.SPECIAL1, 0.25f, 1.0f, 1.5f);
        if (!m_PhotonView.IsMine) return;

        for (int i = 0; i < m_SpawnLocation.Length; ++i)
        {
            Vector3 spawnpos = m_Caster.transform.position + m_SpawnLocation[i] + new Vector3(0.0f, 0.01f, 0.0f);

            LifeTimerWithObjectPool effect = ObjectPool.GetObject<LifeTimerWithObjectPool>(m_SummonEffectPrefab.name);
            if (effect)
            {
                effect.Initialize();
                effect.transform.position = spawnpos;
                effect.gameObject.SetActive(true);
            }
            if (!m_PhotonView.IsMine) continue;
            GameObject g = PhotonNetwork.InstantiateSceneObject(m_SummonMonsterPath + "/" + m_SummonMonsterPrefab.name, spawnpos, Quaternion.identity);
            if (g)
            {
                Character c = g.GetComponent<Character>();
                if (c)
                {
                    c.SetFreeze(1.0f, true);
                }
            }
        }
    }

    public override bool AutoPlayLogic()
    {
        if (m_HPTrigger.Length < 1) return false;
        if (m_TriggerNumber >= m_Trigger.Length) return false;

        if (!m_Trigger[m_TriggerNumber] && m_Caster.m_Health / m_Caster.m_MaxHealth <= m_HPTrigger[m_TriggerNumber] * 0.01f)
        {
            m_Trigger[m_TriggerNumber] = true;
            ++m_TriggerNumber;
            return UseSkill();
        }

        return false;
    }

    void MasterClientSwitched(Player _NewMaster)
    {
        object[] datas = m_PhotonView.InstantiationData;
        if (datas != null)
        {
            m_TriggerNumber = (int)datas[0];

            for (int i = 0; i < m_TriggerNumber; ++i)
            {
                m_Trigger[i] = true;
            }
        }
    }

    private void OnDestroy()
    {
        NetworkManager.Instance.m_MasterClientSwitchedCallback -= MasterClientSwitched;
    }
}

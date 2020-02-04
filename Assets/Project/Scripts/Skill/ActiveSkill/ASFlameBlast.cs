using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ASFlameBlast : ActiveSkill
{
    [SerializeField]
    GameObject m_SubPrefab;
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
        m_Caster.SetStateAndAnimation(E_ANIMATION.SPECIAL2, 0.25f, 2.5f, 0.75f, false, true);

        if (!m_PhotonView.IsMine) return;

        GameObject g = PhotonNetwork.Instantiate("SubPrefabs/" + m_SubPrefab.name, m_Caster.transform.position + Vector3.up * 0.5f, Quaternion.LookRotation(m_Caster.transform.forward, Vector3.up));
        if (g)
        {
            ASFlameBlastSub f = g.GetComponent<ASFlameBlastSub>();
            if (f)
            {
                f.Initialize(m_Caster, CalculateSkillDamage(m_Caster, E_DAMAGETYPE.FIRE, m_DamageMultiply), E_DAMAGETYPE.FIRE);
            }
        }
    }

    public override void AutoPlayLogic()
    {

    }
}

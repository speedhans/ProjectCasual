using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Skill : MonoBehaviour
{
    public enum E_TYPE
    {
        ACTIVE,
        PASSIVE,
    }

    protected E_TYPE m_SkillType;

    public string m_SkillName;
    public Sprite m_Image;
    public float m_MaxCooldown;
    public float m_CurrentCooldown;
    protected Character m_Caster;
    protected PhotonView m_PhotonView;
    System.Action m_UseSkillAction;
    [SerializeField]
    protected GameObject m_UseEffect;
    [SerializeField]
    protected Vector3 m_EffectLocalPosition;
    [SerializeField]
    [TextArea]
    protected string m_SkillManual;

    protected virtual void Awake()
    {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public virtual void Initialize(Character _Caster, bool _Insert = false)
    {
        if (PhotonNetwork.IsConnectedAndReady && m_PhotonView.IsMine)
            m_PhotonView.RPC("Skill_Initialize_RPC", RpcTarget.AllBuffered, _Caster.m_PhotonView.ViewID, _Insert);
        else
        {
            SetSkill(_Caster, _Insert);
        }
    }

    [PunRPC]
    public void Skill_Initialize_RPC(int _PhotonViewID, bool _Insert)
    {
        Character c = NetworkManager.Instance.FindObjectWithPhotonViewID<Character>(_PhotonViewID);
        if (c)
        {
            SetSkill(c, _Insert);
        }
    }

    void SetSkill(Character _Caster, bool _Insert)
    {
        m_Caster = _Caster;
        transform.SetParent(m_Caster.transform);
        transform.localPosition = Vector3.zero;

        if (_Insert && m_SkillType == E_TYPE.ACTIVE)
            m_Caster.m_ListActiveSkill.Add(this as ActiveSkill);
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.m_GameStop) return;
        if (m_CurrentCooldown <= 0.0f) return;

        m_CurrentCooldown -= Time.deltaTime;
    }

    protected void AddUseAction(System.Action _Event) { m_UseSkillAction += _Event; }
    protected void SubUseAction(System.Action _Event) { m_UseSkillAction = _Event; }

    public virtual void UseSkill()
    {
        if (!m_PhotonView.IsMine) return;
        if (m_CurrentCooldown > 0.0f || m_Caster.IsFreeze() || m_Caster.m_Live == Object.E_LIVE.DEAD) return;
        m_CurrentCooldown = m_MaxCooldown;
        m_PhotonView.RPC("UseSkill_RPC", RpcTarget.AllViaServer);
    }

    [PunRPC]
    public void UseSkill_RPC()
    {
        m_CurrentCooldown = m_MaxCooldown;
        if (m_UseEffect != null)
        {
            LifeTimerWithObjectPool effect = ObjectPool.GetObject<LifeTimerWithObjectPool>(m_UseEffect.name);
            if (effect)
            {
                effect.transform.position = transform.position + m_EffectLocalPosition;
                effect.Initialize();
                effect.gameObject.SetActive(true);
            }
        }
        m_UseSkillAction?.Invoke();
    }

    public virtual void AutoPlayLogic() { }
    public E_TYPE GetSkillType() { return m_SkillType; }
    static protected float CalculrateSkillDamage(Character _Caster, E_DAMAGETYPE _DamageType, float _SkillDamageMultiply)
    {
        if (_Caster.m_AttackType == _DamageType)
            return (_Caster.m_AttackDamage + _Caster.m_AddAttackDamage[(int)_DamageType]) * _SkillDamageMultiply; // 타입이 같을때 공식
        else
            return (_Caster.m_AttackDamage + _Caster.m_AddAttackDamage[(int)_Caster.m_AttackType] + _Caster.m_AddAttackDamage[(int)_DamageType]) * _SkillDamageMultiply; // 타입이 다른 공격일때 공식
    }

    public string GetManualText() { return m_SkillManual; }
}

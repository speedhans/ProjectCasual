using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : Item
{
    public EquipmentItem(int _ItemID, int _UniqueID, string _ItemName, Sprite _ItemImage, bool _IsStockable, int _UsageCount, int _MaxUsageCount) : base(_ItemID, _UniqueID, _ItemName, _ItemImage, _IsStockable, _UsageCount, _MaxUsageCount)
    {
    }

    [SerializeField]
    GameObject m_ActiveSkillPrefab;
    [SerializeField]
    GameObject[] m_PassiveSkillPrefabs;
    [SerializeField]
    GameObject m_WeaponPrefab;

    public E_DAMAGETYPE m_ItemElement = E_DAMAGETYPE.BLUNT;
    [Space(2)]
    [Header("Fire, Ice, Electric, Cut, Blunt, Pierce")]
    public float m_FireDamage;
    public float m_IceDamage;
    public float m_ElectricDamage;
    public float m_CutDamage;
    public float m_BluntDamage;
    public float m_PierceDamage;
    [Space(2)]
    [Range(1.0f, 10.0f)]
    public float m_AttackSpeed;
    public float m_AttackRange;
    public float m_CriticalChance;
    public float m_CriticalMuliply;
    public float m_Health;
    public float m_HealthRegeneration;

    protected override void Awake()
    {
        base.Awake();
        m_Type = E_TYPE.EQUIPMENT;
    }

    public virtual void EquipAction(Character _Character)
    {
        _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.FIRE] += m_FireDamage;
        _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.ICE] += m_IceDamage;
        _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.ELECTRIC] += m_ElectricDamage;
        _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.CUT] += m_CutDamage;
        _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.BLUNT] += m_BluntDamage;
        _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.PIERCE] += m_PierceDamage;

        _Character.m_AttackSpeed *= m_AttackSpeed;
        _Character.m_AttackRange += m_AttackRange;
        _Character.m_CriticalChance += m_CriticalChance;
        _Character.m_CriticalMuliply += m_CriticalMuliply;
        _Character.m_MaxHealth += m_Health;
        _Character.m_Health += m_Health;
        _Character.m_PerSecondHealthRegeneration += m_HealthRegeneration;


    }

    public ActiveSkill GetActiveSkill() { return Photon.Pun.PhotonNetwork.Instantiate("Active/" + m_ActiveSkillPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<ActiveSkill>(); }
    public List<PassiveSkill> GetPassiveSkills()
    {
        if (m_PassiveSkillPrefabs != null && m_PassiveSkillPrefabs.Length > 0)
        {
            List<PassiveSkill> list = new List<PassiveSkill>();

            for (int i = 0; i < m_PassiveSkillPrefabs.Length; ++i)
            {
                list.Add(Photon.Pun.PhotonNetwork.Instantiate("Passive/" + m_PassiveSkillPrefabs[i].name, Vector3.zero, Quaternion.identity).GetComponent<PassiveSkill>());
            }

            return list;
        }

        return null;
    }

    public GameObject GetWeaponModel() { return m_WeaponPrefab; }
    public S_EQUIP GetState() { return m_EquipState; }
    public void SetEquip(bool _Equip, int _SlotNumber) { m_EquipState.IsEquip = _Equip; m_EquipState.SlotNumber = _SlotNumber; }
}

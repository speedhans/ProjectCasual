using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentItem : Item
{
    public enum E_ITEMSTATE
    {
        FIREDAMAGE,
        ICEDAMAGE,
        ELECTRICDAMAGE,
        WINDDAMAGE,
        LIGHTDAMAGE,
        DARKDAMAGE,
        ATTACKSPEED,
        MOVEMENTSPEED,
        ATTACKRANGE,
        CRITICALCHANCE,
        CRITICALDAMAGE,
        HEALTH,
        HEALTHREGENERATION,
        MAX
    }

    [SerializeField]
    int m_ReinforceCount;
    [SerializeField]
    GameObject m_ActiveSkillPrefab;
    [SerializeField]
    GameObject[] m_PassiveSkillPrefabs;
    [SerializeField]
    GameObject m_WeaponPrefab;
    [SerializeField]
    E_WEAPONTYPE m_WeaponType;

    public E_DAMAGETYPE m_ItemElement = E_DAMAGETYPE.WIND;
    [Space(2)]
    [Header("Fire, Ice, Electric, Cut, Blunt, Pierce")]
    [Tooltip("&0&")]
    public float m_FireDamage; // &0&
    public float m_FireDamageReinforceBonus;
    [Tooltip("&1&")]
    public float m_IceDamage; // &1&
    public float m_IceDamageReinforceBonus;
    [Tooltip("&2&")]
    public float m_ElectricDamage; // &2&
    public float m_ElectricDamageReinforceBonus;
    [Tooltip("&3&")]
    public float m_WindDamage; // &3&
    public float m_WindDamageReinforceBonus;
    [Tooltip("&4&")]
    public float m_LightDamage; // &4&
    public float m_LightDamageReinforceBonus;
    [Tooltip("&5&")]
    public float m_DarkDamage; // &5&
    public float m_DarkDamageReinforceBonus;
    [Space(2)]
    [Range(1.0f, 10.0f)]
    [Tooltip("&6&")]
    public float m_AttackSpeed; // &6&
    public float m_AttackSpeedReinforceBonus;
    [Range(1.0f, 10.0f)]
    [Tooltip("&7&")]
    public float m_MovementSpeed; // &7&
    public float m_MovementSpeedReinforceBonus;
    [Tooltip("&8&")]
    public float m_AttackRange; // &8&
    public float m_AttackRangeReinforceBonus;
    [Tooltip("&9&")]
    public float m_CriticalChance; // &9&
    public float m_CriticalChanceReinforceBonus;
    [Tooltip("&10&")]
    public float m_CriticalDamage; // &10&
    public float m_CriticalDamageReinforceBonus;
    [Tooltip("&11&")]
    public float m_Health; // &11&
    public float m_HealthReinforceBonus;
    [Tooltip("&12&")]
    public float m_HealthRegeneration; // &12&
    public float m_HealthRegenerationReinforceBonus;

    protected override void Awake()
    {
        base.Awake();
        m_Type = E_TYPE.EQUIPMENT;
    }

    public virtual void EquipAction(Character _Character)
    {
        float[] stats = new float[(int)Character.E_STATS.MAX];

        stats[0] = _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.FIRE] + m_FireDamage + (m_ReinforceCount * m_FireDamageReinforceBonus);
        stats[1] = _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.ICE] + m_IceDamage + (m_ReinforceCount * m_IceDamageReinforceBonus);
        stats[2] = _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.ELECTRIC] + m_ElectricDamage + (m_ReinforceCount * m_ElectricDamageReinforceBonus);
        stats[3] = _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.WIND] + m_WindDamage + (m_ReinforceCount * m_WindDamageReinforceBonus);
        stats[4] = _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.LIGHT] + m_LightDamage + (m_ReinforceCount * m_LightDamageReinforceBonus);
        stats[5] = _Character.m_AddAttackDamage[(int)E_DAMAGETYPE.DARK] + m_DarkDamage + (m_ReinforceCount * m_DarkDamageReinforceBonus);

        stats[6] = _Character.m_AttackSpeed * m_AttackSpeed + (m_ReinforceCount * m_AttackSpeedReinforceBonus);
        stats[7] = _Character.m_MovePerSpeed * m_MovementSpeed + (m_ReinforceCount * m_MovementSpeedReinforceBonus);
        stats[8] = _Character.m_AttackRange + m_AttackRange + (m_ReinforceCount * m_AttackRangeReinforceBonus);
        stats[9] = _Character.m_CriticalChance + m_CriticalChance + (m_ReinforceCount * m_CriticalChanceReinforceBonus);
        stats[10] = _Character.m_CriticalMuliply + (m_CriticalDamage + (m_ReinforceCount * m_CriticalDamageReinforceBonus)) * 0.01f;
        stats[11] = _Character.m_MaxHealth + m_Health + (m_ReinforceCount * m_HealthReinforceBonus);
        stats[12] = _Character.m_Health + m_Health + (m_ReinforceCount * m_HealthReinforceBonus);
        stats[13] = _Character.m_PerSecondHealthRegeneration + m_HealthRegeneration + (m_ReinforceCount * m_HealthRegenerationReinforceBonus);

        _Character.SetStats(stats);
    }

    public ActiveSkill GetActiveSkill() 
    {
        if (!m_ActiveSkillPrefab) return null;

        return Photon.Pun.PhotonNetwork.Instantiate("Active/" + m_ActiveSkillPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<ActiveSkill>();
    }
    public string GetActiveSkillManualText()
    {
        if (!m_ActiveSkillPrefab) return "";

        return m_ActiveSkillPrefab.GetComponent<Skill>().GetManualText(); 
    }
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
    public string[] GetPassiveSkillManualText()
    {
        if (m_PassiveSkillPrefabs != null && m_PassiveSkillPrefabs.Length > 0)
        {
            string[] strAry = new string[m_PassiveSkillPrefabs.Length];

            for (int i = 0; i < m_PassiveSkillPrefabs.Length; ++i)
            {
                strAry[i] = m_PassiveSkillPrefabs[i].GetComponent<Skill>().GetManualText();
            }

            return strAry;
        }
        return null;
    }

    public GameObject GetWeaponModel() { return m_WeaponPrefab; }
    public S_EQUIP GetState() { return m_EquipState; }
    public void SetEquip(bool _Equip, int _SlotNumber) { m_EquipState.IsEquip = _Equip; m_EquipState.SlotNumber = _SlotNumber; }
    public void SetEquip(bool _Equip) { m_EquipState.IsEquip = _Equip; m_EquipState.SlotNumber = -1; }

    public E_WEAPONTYPE GetWeaponType() { return m_WeaponType; }

    public int GetReinforceCount() { return m_ReinforceCount; }
    public void IncreaseReinforceCount(int _Added = 1) 
    {
        m_ReinforceCount += _Added;
        m_ReinforceCount = Mathf.Clamp(m_ReinforceCount, 0, Common.MAXREINFORECEVALUE);
    }

    public float GetEquipmentItemState(E_ITEMSTATE _State)
    {
        switch (_State)
        {
            case E_ITEMSTATE.FIREDAMAGE:
                return m_FireDamage + (m_ReinforceCount * m_FireDamageReinforceBonus);
            case E_ITEMSTATE.ICEDAMAGE:
                return m_IceDamage + (m_ReinforceCount * m_IceDamageReinforceBonus);
            case E_ITEMSTATE.ELECTRICDAMAGE:
                return m_ElectricDamage + (m_ReinforceCount * m_ElectricDamageReinforceBonus);
            case E_ITEMSTATE.WINDDAMAGE:
                return m_WindDamage + (m_ReinforceCount * m_WindDamageReinforceBonus);
            case E_ITEMSTATE.LIGHTDAMAGE:
                return m_LightDamage + (m_ReinforceCount * m_LightDamageReinforceBonus);
            case E_ITEMSTATE.DARKDAMAGE:
                return m_DarkDamage + (m_ReinforceCount * m_DarkDamageReinforceBonus);
            case E_ITEMSTATE.ATTACKSPEED:
                return (m_AttackSpeed + (m_ReinforceCount * m_AttackSpeedReinforceBonus)) * 100.0f - 100.0f;
            case E_ITEMSTATE.MOVEMENTSPEED:
                return (m_MovementSpeed + (m_ReinforceCount * m_MovementSpeedReinforceBonus)) * 100.0f - 100.0f;
            case E_ITEMSTATE.ATTACKRANGE:
                return m_AttackRange + (m_ReinforceCount * m_AttackRangeReinforceBonus);
            case E_ITEMSTATE.CRITICALCHANCE:
                return m_CriticalChance + (m_ReinforceCount * m_CriticalChanceReinforceBonus);
            case E_ITEMSTATE.CRITICALDAMAGE:
                return m_CriticalDamage + (m_ReinforceCount * m_CriticalDamageReinforceBonus);
            case E_ITEMSTATE.HEALTH:
                return m_Health + (m_ReinforceCount * m_HealthReinforceBonus);
            case E_ITEMSTATE.HEALTHREGENERATION:
                return m_HealthRegeneration + (m_ReinforceCount * m_HealthRegenerationReinforceBonus);
            default:
                break;
        }

        return 0;
    }
}

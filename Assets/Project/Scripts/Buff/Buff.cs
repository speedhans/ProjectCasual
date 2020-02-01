using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum E_BUFF
{
    NONE = -1,
    HASTE,
    DAMAGEUP,
    DAMAGEUPMULTI,
    DEFENCEUP,
    DEFENCEMULTI,
    DAMAGEDOWN,
    DAMAGEDOWNMULTI,
    DEFENCEDOWN,
    DEFENCEDOWNMULTI,
}

public abstract class Buff
{
    protected Object m_ParentObject;
    public string m_BuffName;
    public int m_BuffID;
    public List<GameObject> m_ListVisualEffect;
    public float m_LifeTime;
    float m_MaxLifeTime;
    public Sprite m_BuffIcon { get; protected set; }

    public Buff(Object _Self, string _BuffName, int _BuffID, Sprite _BuffIcon, float _LifeTime)
    {
        m_ParentObject = _Self;
        m_BuffName = _BuffName;
        m_BuffID = _BuffID;
        m_LifeTime = _LifeTime;
        m_MaxLifeTime = m_LifeTime;
        m_BuffIcon = _BuffIcon;
    }

    public virtual void Update(float _DeltaTime)
    {
        m_LifeTime -= _DeltaTime;
    }

    public void ResetLifeTime() { m_LifeTime = m_MaxLifeTime; }

    public virtual void DataUpdate(object[] _Value)
    {
        m_LifeTime = (float)_Value[0];
    }

    public virtual void Destroy()
    {
        if (m_ListVisualEffect != null)
        {
            foreach (GameObject g in m_ListVisualEffect)
                UnityEngine.Object.Destroy(g);

            m_ListVisualEffect.Clear();
        }
    }


    //---------- Buff List ------------//
    #region Buff List

    static public Buff CreateBuff(int _BuffID, Object _Target, object[] _BuffData = null)
    {
        switch ((E_BUFF)_BuffID)
        {
            case E_BUFF.HASTE:
                return new BuffHaste(_Target, "Haste", _BuffID, Resources.Load<Sprite>("IconSprite/T_18_next2_"),  (float)_BuffData[0], (float)_BuffData[1], (float)_BuffData[2]);
            case E_BUFF.DAMAGEUP:
                return new BuffDamageUp(_Target, "DamageUp", _BuffID, Resources.Load<Sprite>("IconSprite/T_2_sword45_"), (float)_BuffData[0], (float)_BuffData[1], (E_DAMAGETYPE)_BuffData[2], (string)_BuffData[3], (Character.E_ATTACHPOINT)_BuffData[4]);
            case E_BUFF.DAMAGEUPMULTI:
                return new BuffDamageUpMultiply(_Target, "DamageUpMultiply", _BuffID, Resources.Load<Sprite>("IconSprite/T_1_sword90_"), (float)_BuffData[0], (float)_BuffData[1], (E_DAMAGETYPE)_BuffData[2], (string)_BuffData[3], (Character.E_ATTACHPOINT)_BuffData[4]);
            case E_BUFF.DEFENCEUP:
                return new BuffDefenceUp(_Target, "BuffDefenceUp", _BuffID, Resources.Load<Sprite>("IconSprite/T_1_shield_"), (float)_BuffData[0], (float)_BuffData[1], (E_DAMAGETYPE)_BuffData[2], (string)_BuffData[3], (Character.E_ATTACHPOINT)_BuffData[4]);
            case E_BUFF.DEFENCEMULTI:
                return new BuffDefenceUpMultiply(_Target, "BuffDefenceUpMultiply", _BuffID, Resources.Load<Sprite>("IconSprite/T_4_shield_bezel_"), (float)_BuffData[0], (float)_BuffData[1], (E_DAMAGETYPE)_BuffData[2], (string)_BuffData[3], (Character.E_ATTACHPOINT)_BuffData[4]);
            default:
                return null;
        }
    }

    #endregion Buff List
}


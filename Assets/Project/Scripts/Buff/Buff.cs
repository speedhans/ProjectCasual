using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum E_BUFF
{
    HASTE,
    DAMAGEUP,
    DAMAGEUPMULTI,
    DEFENCEUP,
    DEFENCEMULTI,
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
    System.Action<float> m_BuffAction;
    System.Action<object[]> m_DataUpdateEvent;

    public Buff(Object _Self, string _BuffName, int _BuffID, Sprite _BuffIcon, float _LifeTime)
    {
        m_ParentObject = _Self;
        m_BuffName = _BuffName;
        m_BuffID = _BuffID;
        m_LifeTime = _LifeTime;
        m_MaxLifeTime = m_LifeTime;
        m_BuffIcon = _BuffIcon;
    }

    public void Update(float _DeltaTime)
    {
        m_LifeTime -= _DeltaTime;
        m_BuffAction?.Invoke(_DeltaTime);
    }

    /// <param name="_Action"> input value is deltatime </param>
    protected void AddAction(System.Action<float> _Action)
    {
        m_BuffAction += _Action;
    }

    public void ResetLifeTime() { m_LifeTime = m_MaxLifeTime; }

    protected abstract void Action(float _DeltaTime);
    protected void AddDataUpdateAction(System.Action<object[]> _Function) { m_DataUpdateEvent += _Function; }

    public void DataUpdate(object[] _Value)
    {
        m_LifeTime = (float)_Value[0];
        m_DataUpdateEvent?.Invoke(_Value);
    }

    public abstract void DataUpdateEvent(object[] _Value);

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
                return new BuffHaste(_Target, "Haste", _BuffID, Resources.Load<Sprite>("IconSprite/T_18_next2_"),  (float)_BuffData[0], (float)_BuffData[1]);
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


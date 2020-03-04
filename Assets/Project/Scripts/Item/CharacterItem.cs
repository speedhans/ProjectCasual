using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterItem : Item
{
    [SerializeField]
    Sprite m_PosterImage;
    [SerializeField]
    string m_ModelName;
    [SerializeField]
    int m_ReinforceCount;

    protected override void Awake()
    {
        base.Awake();
        m_Type = E_TYPE.CHARACTER;
    }

    public Sprite GetPosterImage() { return m_PosterImage; }
    public string GetModelName() { return m_ModelName; }
    public S_EQUIP GetState() { return m_EquipState; }
    public void SetEquip(bool _Equip) { m_EquipState.IsEquip = _Equip; m_EquipState.SlotNumber = 0; }
    public int GetReinforceCount() { return m_ReinforceCount; }
    public void IncreaseReinforceCount(int _Added = 1)
    {
        m_ReinforceCount += _Added;
        m_ReinforceCount = Mathf.Clamp(m_ReinforceCount, 0, Common.MAXREINFORECEVALUE);
    }
}

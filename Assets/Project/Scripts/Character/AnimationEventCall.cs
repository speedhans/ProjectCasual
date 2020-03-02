using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCall : MonoBehaviour
{
    Character m_Character;

    private void Awake()
    {
        m_Character = GetComponentInParent<Character>();
    }

    public void AttackMomentEvent()
    {
        SoundManager.Instance.PlayAttackMomentSound(m_Character.m_AttackSound);
        m_Character.AttackMomentEvent();
    }

    public void FootEvent(int _Value)
    {

    }
}

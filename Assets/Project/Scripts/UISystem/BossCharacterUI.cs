using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacterUI : DefaultUI
{
    class BossUIData
    {
        List<SpriteRenderer> m_ListBuffIcon = new List<SpriteRenderer>();
        Character m_Character;

        public void SetCharacter(Character _Target)
        {
            m_Character = _Target;
        }
    }


    [SerializeField]
    float m_UpdateDelay = 0.5f;
    float m_UpdateTimer;

}

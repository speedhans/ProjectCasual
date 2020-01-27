using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBossCharacter : RangeCharacter
{
    protected override void Awake()
    {
        base.Awake();
        m_CharacterType = E_CHARACTERTYPE.BOSSNPC;

        SetAutoPlayLogic(AutoPlayLogic);
    }

    protected override void AutoPlayLogic()
    {
        base.AutoPlayLogic();


    }
}

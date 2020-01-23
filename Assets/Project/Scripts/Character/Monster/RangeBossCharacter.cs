using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBossCharacter : RangeCharacter
{
    protected override void Awake()
    {
        base.Awake();

        SetAutoPlayLogic(AutoPlayLogic);
    }

    protected override void AutoPlayLogic()
    {
        base.AutoPlayLogic();


    }
}

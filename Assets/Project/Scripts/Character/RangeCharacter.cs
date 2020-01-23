using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RangeCharacter : Character
{
    protected override void Awake()
    {
        base.Awake();

        SetAutoPlayLogic(AutoPlayLogic);
    }

    protected override void AutoPlayLogic()
    {

    }
}

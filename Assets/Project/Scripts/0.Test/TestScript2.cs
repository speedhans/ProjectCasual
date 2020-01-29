using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : TestScript1
{
    public override void TestCall()
    {
        base.TestCall();

        Debug.Log("Child");
    }
}

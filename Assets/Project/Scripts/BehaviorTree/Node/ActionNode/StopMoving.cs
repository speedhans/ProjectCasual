using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StopMoving : ActionNode
{
    public override bool BehaviorUpdate(Character _Self, float _DeltaTime)
    {
        _Self.StopMoving();
        return true;
    }

    public override void TickUpdate(Character _Self, float _DeltaTime)
    {
    }
}

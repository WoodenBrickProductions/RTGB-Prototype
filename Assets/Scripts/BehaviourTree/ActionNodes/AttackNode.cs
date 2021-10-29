using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNode : ActionNode
{
    public string targetName;
    
    private IDealsDamage attacker;
    private TileObject target;
    private State returnState = State.Running;
    
    protected override void OnStart()
    {
        if (targetName != null && tileObject is IDealsDamage)
        {
            blackboard.objects.TryGetValue(targetName, out target);
            attacker = (IDealsDamage) tileObject;
        }
        else
        {
            returnState = State.Failure;
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (returnState == State.Failure || !(target is IAttackable))
        {
            return State.Failure;
        }

        bool result = attacker.Attack((IAttackable) target);
        return result ? State.Success : State.Failure;
    }
}

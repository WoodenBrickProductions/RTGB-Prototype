using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteOnceNode : DecoratorNode
{
    public bool executed = false;
    
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (!executed)
        {
            State newState = child.Update();
            if (newState != State.Running)
            {
                executed = true;
            }

            return newState;
        }
        else
        {
            return State.Failure;
        }
    }
}

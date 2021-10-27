using System;
using UnityEngine;

public enum Operation
{
    EQUALS,
    MORETHAN,
    LESSTHAN,
    NOTEQUALS,
    NOTMORETHAN,
    NOTLESSTHAN
}

public enum Value
{
    BOOL,
    FLOAT
}

public class ConditionNode : ActionNode
{
    public string conditionName;
    public Operation operationType;
    public Value valueType;
    public bool conditionValue = true;
    public float floatValue;
    
    private State returnState = State.Running;
    protected override void OnStart()
    {
        if (conditionName.Length == 0)
        {
            Debug.Log("Condition is not defined");
            returnState = State.Failure;
        }

        bool v;
        // if (valueType == Value.FLOAT)
        // {
        //     switch (operationType)
        //     {
        //         case Operation.EQUALS:
        //             break;
        //         case Operation.MORETHAN:
        //             break;
        //         case Operation.LESSTHAN:
        //             break;
        //         case Operation.NOTEQUALS:
        //             break;
        //         case Operation.NOTMORETHAN:
        //             break;
        //         case Operation.NOTLESSTHAN:
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        //
        //     returnState = v == conditionValue ? State.Success : State.Failure;
        // }
        // else
        {
            blackboard.conditions.TryGetValue(conditionName, out v);
            returnState = v == conditionValue? State.Success : State.Failure;
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return returnState;
    }
}

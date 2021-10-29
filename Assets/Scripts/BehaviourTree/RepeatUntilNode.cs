using UnityEngine;
using UnityEngine.UI;

public class RepeatUntilNode : DecoratorNode
{
    public enum Repeat
    {
        Times,
        Duration
    }
    
    /// <summary>
    /// How many times to repeat (-1 if infinite)
    /// </summary>
    public Repeat repeatType = Repeat.Times;
    public int times = -1;

    public float duration = 1;
    public State untilState = State.Success;
    
    private int current;
    private float startTime;
    
    protected override void OnStart()
    {
        current = times;
        startTime = Time.time;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (repeatType == Repeat.Times)
        {
            if (current == 0)
            {
                return State.Failure;
            }

            if (current > 0)
            {
                current--;
            }

            State newState = child.Update();
            return newState == untilState ? State.Success : State.Running;
            
        }
        else
        {
            State newState = child.Update();
            if (Time.time - startTime <= duration)
            {
                return newState == untilState ? State.Success : State.Running;
            }
            else
            {
                if (newState == State.Running)
                {
                    return State.Running;
                }
                
                return newState == untilState ? State.Success : State.Failure;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HierarchicalStateMachine
{
    // Start is called before the first frame update
    private Stack<State> root;
    private State previousState;
    private State currentState;
    private State newState;

    private Dictionary<string, object> blackboard;
    void Start()
    {
        root.Push(new State());
        currentState = root.Peek();
        blackboard = new Dictionary<string, object>();
        blackboard.Add("CHECK", new Dictionary<string, object>());
    }

    // Update is called once per frame
    void GetResult()
    {
        if (currentState.Check())
        {
            Result result = currentState.Execute();
            switch (result)
            {
                case Result.Success:
                    newState = currentState.OnSuccess();
                    break;
                case Result.Failure:
                    newState = currentState.OnFailure();
                    break;
                case Result.Running:
                    return;
            }
            
            if (newState != null)
            {
                previousState = currentState;
                currentState = newState;
            }
            else
            {
                previousState = root.Pop();
                currentState = root.Peek();
            }
        }
    }
}

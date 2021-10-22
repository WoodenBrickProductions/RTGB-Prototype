using UnityEngine;

public sealed class PlaceholderNode : CompositeNode
{
    public State returnState = State.Success;
    protected override void OnStart()
    {
        Debug.Log("Placeholder node started: " + description);
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        return returnState;
    }
}

public class IsObjectInRangeNode : ActionNode
{
    public TileObject target;
    public float range;

    private State returnState;
    
    protected override void OnStart()
    {
        if (target == null && tileObject == null)
        {
            returnState = State.Failure;
            return;
        }

        UnitController unit = (UnitController) tileObject;
        returnState = unit.IsObjectWithinRange(target, range) ? State.Success : State.Failure;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        return returnState;
    }
}

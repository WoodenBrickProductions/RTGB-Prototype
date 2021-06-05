public class MovingState : IState
{
    private bool[] _legalActions = {false, false, true, true};
    public void Entry()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public bool CanDoAction(int actionIndex)
    {
        return _legalActions[actionIndex];
    }
}

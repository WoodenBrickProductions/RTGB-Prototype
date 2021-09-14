public enum Result {Running, Failure, Success}

public class State
{
    public virtual bool Check()
    {
        return true;
    }
    
    public virtual Result Execute()
    {
        return Result.Failure;
    }

    public virtual State OnFailure()
    {
        return null;
    }

    public virtual State OnSuccess()
    {
        return null;
    }
}

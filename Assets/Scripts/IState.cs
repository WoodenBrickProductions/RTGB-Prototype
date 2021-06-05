using System;

enum GlobalStateActions
{
    CanMove,
    CanAttack,
    CanInteract,
    CanCast
}

public interface IState
{
    void Entry();

    void Update();

    void Exit();

    bool CanDoAction(int actionIndex);
}

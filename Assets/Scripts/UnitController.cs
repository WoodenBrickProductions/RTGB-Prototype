using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States
{
    Idle,
    Moving,
    Attacking,
    Disabled
}

public class UnitController : MonoBehaviour
{
    private States _state;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO switch to event and Invoke based system
    private void SwitchState(States newState)
    {

        CustomExit(_state);
        CustomAwake(newState);
        _state = newState;
    }
    
    private void CustomAwake(States state)
    {
        switch (state)
        {
            case(States.Idle):
                Idle_Awake();
                break;
            case(States.Moving):
                Moving_Awake();
                break;                
        }        
    }

    private void CustomUpdate(States state)
    {
        switch (state)
        {
            case(States.Idle):
                Idle_Update();
                break;
            case(States.Moving):
                Moving_Update();
                break;    
        }
    }
    
    private void CustomExit(States state)
    {
        switch (state)
        {
            case(States.Idle):
                Idle_Exit();
                break;
            case(States.Moving):
                Idle_Exit();
                break;    
        }
    }
    
    private void Idle_Awake()
    {
        
    }

    private void Idle_Update()
    {
        
    }

    private void Idle_Exit()
    {
        
    }

    private void Moving_Awake()
    {
        
    }

    private void Moving_Update()
    {
        
    }

    private void Moving_Exit()
    {
        
        
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_StateMachine_FSMRBS : MonoBehaviour
{
    private Dictionary<Type, ST_Base_FSMRBS> states;

    public ST_Base_FSMRBS currentState;

    public ST_Base_FSMRBS CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            currentState = value;
        }
    }

    public void SetStates(Dictionary<Type, ST_Base_FSMRBS> states)
    {
        this.states = states;
    }

    void Update()
    {
        if (CurrentState == null)
        {
            //Debug.Log(states.Values.First());
            if (states.Values.First() != null)
            {
                CurrentState = states.Values.First();
                CurrentState.EnterState();
            }
            
        }
        else
        {
            var nextState = CurrentState.StateLogic();

            if (nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState);
            }
        }
    }

    void SwitchToState(Type nextState)
    {
        CurrentState.LeaveState();
        CurrentState = states[nextState];
        CurrentState.EnterState();
    }
}

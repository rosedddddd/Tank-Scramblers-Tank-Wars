using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public State curState;
    public void ChangeState<T>(List<T> param, string stateID) where T : State
    {
        bool foundState = false;
        // leaving the current state
        if (curState != null)
        {
            curState.LeaveState();
        }

            // search through the list of states until you find a matching stateID
            for (int i = 0; i < param.Count; i++)
        {
            if (param[i].stateID == stateID)
            {
                foundState = true;
                curState = param[i];
            }
        }

        if ( foundState == false) // check if there was a succesfull change
        {
            Debug.LogError("No ''" + stateID + "'' state exists inside of this controllers possible states");
            return;
        }


        //entering new state
        curState.EnterState();
    }


    public virtual void ControllerUpdate()
    {

        curState.FixedStateLogic();
    }

    public virtual void ControllerStart()
    {

    }


    private void FixedUpdate()
    {
        ControllerUpdate();
    }

    private void Start()
    {
        ControllerStart();
    }
}

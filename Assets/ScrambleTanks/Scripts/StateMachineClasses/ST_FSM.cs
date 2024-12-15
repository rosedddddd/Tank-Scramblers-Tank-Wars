using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_FSM : MonoBehaviour
{
    public ST_Smart_Tank tank;
    public Type curState; //current state
    public Dictionary<Type, ST_BaseTankState> states;

    public bool AttemptStateChange(Type newState)
    {
        if (newState == curState || newState == null) return false;


        //checking if the LeaveState() function returns a different state to imediately jump to
        if (AttemptStateChange(states[curState].LeaveState())) return false;

        //checking if the EnterState() function returns a different state to imediately jump to
        if (AttemptStateChange(states[newState].EnterState())) return false;

        //finaly actually changing the state once no ovverides are detected.
        curState = newState;
        Debug.LogError(curState);


        return true;
        // do not place overlaping state conditions inside the EnterState() or LeaveState() function of multiple states
        // it could cause an infinite loop if for example:
        /*
         * 
         * ChaseState{
         * 
         *      Type EnterState()
         *      {
         *           (distance to enemmy < 15) return RoamState
         *      {
         * }
         * 
         * RoamState{
         * 
         *      Type EnterState()
         *      {
         *           (distance to enemmy > 10) return ChaseState
         *      {
         * } 
         */
        // this would crash unity while the enemmy is between 10-15 units away

    }

    public virtual void ControllerUpdate()
    {
        AttemptStateChange(states[curState].StateLogic());

    }

    public virtual void ControllerStart()
    {
        Debug.Log(states.Count);
        if (curState == null)
        {
            curState = states.Keys.First();
            
        }
    }


}

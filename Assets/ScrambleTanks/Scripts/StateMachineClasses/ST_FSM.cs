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
        InitializeStates();

        if (curState == null)
        {
            curState = typeof(ST_Tank_Kiting);
        }
    }

    public virtual void InitializeStates()
    {

        states = new Dictionary<Type, ST_BaseTankState>();

        states.Add(typeof(ST_Tank_Search), new ST_Tank_Search());
        states.Add(typeof(ST_Tank_Chase), new ST_Tank_Chase());
        states.Add(typeof(ST_Tank_Attack), new ST_Tank_Attack());
        states.Add(typeof(ST_Tank_Retreat), new ST_Tank_Retreat());
        states.Add(typeof(ST_Tank_Kiting), new ST_Tank_Kiting());

        //linking the states to the controller
        ST_BaseTankState[] stateReferences = states.Values.ToArray();
        foreach (var stateReference in stateReferences)
        {
            stateReference.tank = this.tank;
        }
    }


}

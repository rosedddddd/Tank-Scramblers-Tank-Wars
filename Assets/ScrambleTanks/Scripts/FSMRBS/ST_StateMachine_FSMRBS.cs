using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_StateMachine_FSMRBS : MonoBehaviour
{
    public ST_SmartTankFSMRBS tank;
    public Type curState; //current state
    public Dictionary<Type, ST_Base_FSMRBS> states;

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
        InitializeStates_FSMRBS();

        if (curState == null)
        {
            curState = typeof(ST_Search_FSMRBS);

        }
    }

    public virtual void InitializeStates_FSMRBS()
    {

        //Dictionary<Type, ST_BaseTank_FSMRBS> states = new Dictionary<Type, ST_BaseTank_FSMRBS>();

        //states.Add(typeof(ST_Search_FSMRBS), new ST_Search_FSMRBS());
        //states.Add(typeof(ST_Attack_FSMRBS), new ST_Tank_Chase());
        //states.Add(typeof(ST_Chase_FSMRBS), new ST_Tank_Attack());
        //states.Add(typeof(ST_Kiting_FSMRBS), new ST_Tank_Retreat());
        //states.Add(typeof(ST_Retreat_FSMRBS), new ST_Tank_Kiting());

        ////linking the states to the controller
        //ST_BaseTank_FSMRBS[] stateReferences = states.Values.ToArray();
        //foreach (var stateReference in stateReferences)
        //{
        //    stateReference.tank = this.tank;
        //}
    }
}

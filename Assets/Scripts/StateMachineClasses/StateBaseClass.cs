using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class State : MonoBehaviour
{
    // this class works weith the Controller class, but you should make a custom variable
    // in any inherited classes so that it's easier to work with
    //public Controller controller;
    public string stateID = "empty";

    // setting up to enter the state
    public virtual Type EnterState()
    {
        Debug.Log(stateID);
        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public virtual void LeaveState()
    {

    }


    // a number of checks made to if you should enter the state
    public virtual void StateConditions()
    {

    }

    // logic that runs every frame
    public virtual void UpdateStateLogic()
    {

    }
    // logic that runs every physics update
    public virtual void FixedStateLogic()
    {
        StateConditions();
    }
}



using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public abstract class ST_State : MonoBehaviour
{

    // setting up to enter the state
    public abstract Type EnterState();

    // anything that needs to be reset or changed once the player leaves the stage
    public abstract Type LeaveState();

    // logic that runs every physics update inside of the controller
    public abstract Type StateLogic();
}



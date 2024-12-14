using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Guard : ST_BaseTankState
{

    // setting up to enter the state
    public override Type EnterState()
    {

        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public override Type LeaveState()
    {
        return null;
    }

    // logic that runs every physics update inside of the controller
    public override Type StateLogic()
    {

        tank.FollowPathToWorldPoint(tank.baseWaypoint.gameObject, 1);


        if (tank.VisibleEnemyTanks.Count > 0)
            return typeof(ST_Tank_Search); // switches to search state so it may figure out what the appropriate next move is
        

        if (Vector3.Distance(tank.baseWaypoint.position, tank.transform.position) < 3) 
            return typeof(ST_Tank_Search); // reached distination but no tank seen :(

        if (tank.lowFuel) 
            return typeof(ST_Tank_Search); // give up if you have little fuel

        return null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Retreat : ST_BaseTankState
{
    float retreatTime;

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
        //if low health and low ammo go into attack
        if (tank.lastStand) { return typeof(ST_Tank_Attack); }

        retreatTime += Time.deltaTime;

        //if health, ammo or fuel are low go look for consumables
        if (tank.lowHealth || tank.lowAmmo || tank.lowFuel)
        {
            float dinamicSpeed = tank.TankCurrentFuel;
            Debug.Log("i retreat");
            
            if (tank.VisibleConsumables.Count > 0)
            {
                Debug.Log("I see consumable");
                tank.FollowPathToWorldPoint(tank.VisibleConsumables.First().Key, dinamicSpeed, tank.heuristicMode);
            }
            else if (tank.VisibleConsumables.Count == 0)
            {
                tank.FollowPathToRandomWorldPoint(.7f, tank.heuristicMode);
            }
        }
        //if good condition go back to search
        if (!tank.lowHealth && !tank.lowAmmo && !tank.lowFuel)
        {
            Debug.Log("Good Health! Back to search");
            //return typeof(ST_Tank_Search);
        }
        else if(retreatTime > 20f) { Debug.Log("time ran out"); return typeof(ST_Tank_Search); }

        return null;
    }
}

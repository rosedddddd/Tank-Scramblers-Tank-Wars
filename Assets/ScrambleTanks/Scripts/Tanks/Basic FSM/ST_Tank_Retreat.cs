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
        retreatTime = 0f;  // Reset retreatTime when entering the retreat state
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
            float dynamicSpeed = Mathf.Max(tank.TankCurrentFuel / 30,.3f);  // note for whoever first wrote this line
                                                                            // the speed of the tank is a 0-1 range while tank fuel is 0-100
                                                                            // just doing speed = tank fuel like how you were doing previously
                                                                            // would have no effect until tank fuel goes bellow 1
                                                            
            if (tank.VisibleConsumables.Count > 0)
            {
                //see's consumable
                tank.FollowPathToWorldPoint(tank.VisibleConsumables.First().Key, dynamicSpeed, tank.heuristicMode);
            }
            else if (tank.VisibleConsumables.Count == 0)
            {
                //randomly wonder
                tank.FollowPathToRandomWorldPoint(dynamicSpeed, tank.heuristicMode);
            }
        }
        //if in good condition go back to search
        if (!tank.lowHealth && !tank.lowAmmo && !tank.lowFuel)
        {
            //Debug.Log("Good Health! Back to search");
            return typeof(ST_Tank_Search);
        }
        
        if(tank.tooCowardly) { Debug.Log("spent too much time running away"); return typeof(ST_Tank_Search); }



        tank.timeSpentInRetreatState += Time.deltaTime;

        return null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Search : ST_BaseTankState
{
    float newPathTime = 10; // the number of seconds the timer bellow has
    float newPathTimer; // time before changing to a different random path

    float scanDuration = 1.5f; // how long the tank pauses to scan it's surroundings in seconds
    float scanPause = 4; // how long to move the tank in-between scaning
    float timeAtScan; // used in scanning to make the turret always start out facing the front of the tank
    float scanTimer; // the timer that scanning is dictated by

    bool scanning { get { return scanTimer >= scanPause; } } // currently scanning, determined by if the scanTimer is above scanPause.
                                                             // The scanTimer resets once it's greater than scanPause + scanDuration

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

        if (tank.takenBackshot) scanTimer = scanPause; // begin scanning imediately if attacked while not seeing an enemmy


        if (tank.VisibleEnemyTanks.Count > 0)
        {
            if (!tank.hasKited) { tank.hasKited = true; return typeof(ST_Tank_Kiting); } // only go in to the kiting state the first time you see the enemmy

            //check smart tank for comments on these variables
            if (tank.lastStand) return typeof(ST_Tank_Chase);
            if (tank.lowHealth || tank.lowAmmo || tank.lowFuel) return typeof(ST_Tank_Retreat);
            return typeof(ST_Tank_Chase); // if all condingencies are not met after seeing the enemmy tank, just attack
        }

        if (tank.VisibleConsumables.Count > 0)
        {
            tank.FollowPathToWorldPoint(tank.VisibleConsumables.Keys.First(), 1);
            return null;
        }

        if (tank.VisibleEnemyBases.Count > 0)
        {
            if (!tank.lowAmmo && !tank.lowFuel) return typeof(ST_Tank_Attack);
        }

        // by using return inside of each of the above if statements I am making the tank prioritize enemmies over consumables and consumables over bases

        

        // scanning but otherwise moving
        if (scanning) {
            // using sin and cos to rotate a transform circularly around the tank and making the turret face it
            tank.calcTransform.position = tank.transform.position + (new Vector3(Mathf.Sin((Time.time - timeAtScan) * 5f), 0, Mathf.Cos((Time.time - timeAtScan) * 5f)) * 10);
            tank.TurretFaceWorldPoint(tank.calcTransform.gameObject);
        }else
        {
            tank.FollowPathToRandomWorldPoint(.3f, tank.heuristicMode);
        }


        #region timers

        if (scanning && Time.time - timeAtScan > scanPause) // true whenever a new scan begins
        {
            timeAtScan = Time.time;
        }

        scanTimer += Time.deltaTime;
        newPathTimer += Time.deltaTime;

        // reseting the scan timer if it has gone above the combined values
        if (scanTimer > scanDuration + scanPause)
        {
            scanTimer = 0;
        }

        // new path after a certain ammount of time
        newPathTimer += Time.deltaTime;
        if (newPathTimer > 10)
        {
            tank.GenerateNewRandomWorldPoint();
            newPathTimer = 0;
        }

        #endregion timers

        return null;
    }
}

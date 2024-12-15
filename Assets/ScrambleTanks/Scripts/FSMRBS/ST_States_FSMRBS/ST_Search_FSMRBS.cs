using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_Search_FSMRBS : ST_Base_FSMRBS
{
    private ST_SmartTankFSMRBS smartTank;

    float newPathTime = 10; // the number of seconds the timer bellow has
    float newPathTimer; // time before changing to a different random path

    float scanDuration = 1.5f; // how long the tank pauses to scan it's surroundings in seconds
    float scanPause = 4; // how long to move the tank in-between scaning
    float timeAtScan; // used in scanning to make the turret always start out facing the front of the tank
    float scanTimer; // the timer that scanning is dictated by

    bool scanning { get { return scanTimer >= scanPause; } } // currently scanning, determined by if the scanTimer is above scanPause.
                                                             // The scanTimer resets once it's greater than scanPause + scanDuration

    public ST_Search_FSMRBS(ST_SmartTankFSMRBS smartTank)
    {
        this.smartTank = smartTank;
    }

    // setting up to enter the state
    public override Type EnterState()
    {
        smartTank.stats["searchState_FSMRBS"] = true;
        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public override Type LeaveState()
    {
        smartTank.stats["searchState_FSMRBS"] = false;
        return null;
    }

    // logic that runs every physics update inside of the controller
    public override Type StateLogic()
    {
        //foreach (var item in smartTank.rules.GetRules)
        //{
        //    if (item.CheckRule(smartTank.stats) != null)
        //    {
        //        return item.CheckRule(smartTank.stats);
        //    }
        //}

        if (smartTank.takenBackshot) scanTimer = scanPause; // begin scanning imediately if attacked while not seeing an enemmy

        if (smartTank.VisibleConsumables.Count > 0)
        {
            //smartTank.FollowPathToWorldPoint(smartTank.VisibleConsumables.Keys.First(), 1f);
            return null;
        }

        // scanning but otherwise moving
        if (scanning)
        {
            // using sin and cos to rotate a transform circularly around the tank and making the turret face it
            smartTank.calcTransform.position = smartTank.transform.position + (new Vector3(Mathf.Sin((Time.time - timeAtScan) * 5f), 0, Mathf.Cos((Time.time - timeAtScan) * 5f)) * 10);
            smartTank.TurretFaceWorldPoint(smartTank.calcTransform.gameObject);
        }
        else
        {
            smartTank.FollowPathToRandomWorldPoint(.3f, smartTank.heuristicMode);
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
            smartTank.GenerateNewRandomWorldPoint();
            newPathTimer = 0;
        }

        #endregion timers

        return null;
    }
}

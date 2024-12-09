using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Search : ST_BaseTankState
{
    float newPathTimer;

    float scanDuration = 1.5f;
    float scanPause = 4;
    float scanTimer;
    float timeAtScan;
    bool scanning { get { return scanTimer >= scanPause; } }

    bool frameFlipper;
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
        frameFlipper = !frameFlipper;

        if (tank.takenBackshot) scanTimer = scanPause;



        if (tank.VisibleEnemyTanks.Count > 0)
        {
            if (tank.lastStand) return typeof(ST_Tank_Attack);
            if (tank.lowHealth || tank.lowAmmo || tank.lowFuel) return typeof(ST_Tank_Retreat);
            return typeof(ST_Tank_Attack);
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

        


        if (scanning) {
            tank.calcTransform.position = tank.transform.position + (new Vector3(Mathf.Sin((Time.time - timeAtScan) * 5f), 0, Mathf.Cos((Time.time - timeAtScan) * 5f)) * 10);
            tank.TurretFaceWorldPoint(tank.calcTransform.gameObject);
        }else
        {
            tank.FollowPathToRandomWorldPoint(.3f, tank.heuristicMode);
        }

        scanTimer += Time.deltaTime;
        if (scanTimer > scanDuration + scanPause)
        {
            scanTimer = 0;
        }

        if (scanning && Time.time - timeAtScan > scanPause)
        {
            timeAtScan = Time.time;
        }

        newPathTimer += Time.deltaTime;
        if (newPathTimer > 10)
        {
            tank.GenerateNewRandomWorldPoint();
            newPathTimer = 0;
        }

        return null;
    }
}

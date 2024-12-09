using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Search : ST_BaseTankState
{
    float t;
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

        if (tank.VisibleEnemyTanks.Count > 0)
        {
            tank.FollowPathToRandomWorldPoint(1, tank.heuristicMode);
            tank.TurretFaceWorldPoint(tank.VisibleEnemyTanks.Keys.First());
        }
        else
        {
            tank.FollowPathToRandomWorldPoint(.3f, tank.heuristicMode);
            tank.calcTransform.position = tank.transform.position + new Vector3(Mathf.Sin(Time.time * 5), 0, Mathf.Cos(Time.time * 5));
            tank.TurretFaceWorldPoint(tank.calcTransform.gameObject);
        }


        t += Time.deltaTime;
        if (t > 10)
        {
            tank.GenerateNewRandomWorldPoint();
            t = 0;
        }

        return null;
    }
}

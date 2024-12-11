using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ST_Tank_Kiting : ST_BaseTankState
{
    public float t;
    bool circlingClockwise = true;
    public float kitCount;

    // setting up to enter the state
    public override Type EnterState()
    {
        return null;
    }

    // anything that needs to be reset or changed once the player leaves the stage
    public override Type LeaveState()
    {
        tank.hasKited = true;
        return null;
    }

    // logic that runs every physics update inside of the controller
    public override Type StateLogic()
    {
        //if health dropped too much, retreat
        if (tank.TankCurrentHealth <= 75) { return typeof(ST_Tank_Retreat); }
        //time ran out, retreat
        if(t >= 15) { return typeof(ST_Tank_Attack);}


        if (tank.lastSeenTimer < 15) //enemy tank seen
        {
            tank.kitingDistance = 20f; //circle distance
            float dist = Vector3.Distance(tank.transform.position, tank.enemyLastSeen.position); //distance between tanks
            Vector3 normalized = (tank.transform.position - tank.enemyLastSeen.position).normalized; //normalized position
            float cirAngleClose = 40f; //circling angle
            cirAngleClose *= circlingClockwise ? -1 : 1; //check if going clock/counterclock wise
            normalized = Quaternion.AngleAxis(cirAngleClose, Vector3.up) * normalized; //normalized quaternion
            tank.calcTransform.position = tank.enemyLastSeen.position + normalized * tank.kitingDistance; //move tank to said position

            //use rays to check if there's an obstacle
            RaycastHit hit;
            if ( 
                Physics.Raycast(
                    new Ray(tank.transform.position, tank.calcTransform.position - tank.transform.position),
                    out hit,
                    Vector3.Distance(tank.transform.position, tank.calcTransform.position), 
                    tank.raycastLayers
                    )
                )
            {
                circlingClockwise = !circlingClockwise;
                Debug.DrawLine(tank.transform.position, hit.point, Color.red,10);
            }
            else
            {
                Debug.DrawLine(tank.transform.position, tank.transform.position + tank.calcTransform.position - tank.transform.position);
            }
            //generate new point to follow if can't traverse
            tank.GeneratePathToWorldPoint(tank.calcTransform.gameObject);
            tank.FollowPathToWorldPoint(tank.calcTransform.gameObject, 0.5f, tank.heuristicMode);

            if (dist < 25F) { t += Time.deltaTime; } //while tank is still within distance keep adding time
            
            tank.TurretFaceWorldPoint(tank.enemyLastSeen.gameObject);
        }


        return null;
    }
}
